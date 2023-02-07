using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[ViewInfo(UILayer.PopUpWindow, UILife.Permanent)]
public class UIFarmView : ViewBase<UIFarmViewModel>
{
    [SerializeField] private Text txtLevel;
    [SerializeField] private Button btnLevelUp;
    [SerializeField] private Button btnReap;
    [SerializeField] private Button btnTomato;
    [SerializeField] private Button btnPotato;
    [SerializeField] private Button btnCotton;
    [SerializeField] private Button btnClose;

    [SerializeField] private List<UIGrid> uiGridList = new List<UIGrid>();

    private void Awake()
    {
        btnLevelUp.onClick.AddListener(BtnOnClick_LevelUp);
        btnReap.onClick.AddListener(BtnOnClick_Reap);
        btnTomato.onClick.AddListener(BtnOnClick_Tomato);
        btnPotato.onClick.AddListener(BtnOnClick_Potato);
        btnCotton.onClick.AddListener(BtnOnClick_Cotton);
        btnClose.onClick.AddListener(BtnOnClick_Close);
    }

    protected override void OnShowStart(bool immediate)
    {
        base.OnShowStart(immediate);

        var goodDataDict = FarmSystem.Instance.GetFarmGoodsData();
        var farmChangeActionDict = FarmSystem.Instance.GetFarmChangeAction();
        for (int i = 0; i < uiGridList.Count; i++)
        {
            var uiGrid = uiGridList[i];
            uiGrid.SetId(i);
            var data = goodDataDict[i];
            UpdateFarmUI(data, uiGrid);
            UpdateAddictiveState(i);

            uiGrid.BtnAddictiveAction += LevelUpAddictive;
            farmChangeActionDict[i] += FarmChangeAction;
        }

        FarmSystem.Instance.FarmFreeAction += UpdateSowBtn;
    }

    protected override void OnHideFinish()
    {
        base.OnHideFinish();

        var farmChangeActionDict = FarmSystem.Instance.GetFarmChangeAction();
        for (int i = 0; i < uiGridList.Count; i++)
        {
            var uiGrid = uiGridList[i];
            uiGrid.BtnAddictiveAction -= LevelUpAddictive;
            farmChangeActionDict[i] -= FarmChangeAction;
        }

        FarmSystem.Instance.FarmFreeAction -= UpdateSowBtn;
    }

    private void LevelUpAddictive(int id)
    {
        FarmSystem.Instance.LevelUpAddictive(id);
        UpdateAddictiveState(id);
    }

    private void UpdateAddictiveState(int id)
    {
        StringBuilder tipsBuilder = new StringBuilder();
        var num =  FarmSystem.Instance.GetAddictiveNum(id);
        for (int i = 0; i <  FarmSystem.Instance.AddictiveLevelConfigArray.Length; i++)
        {
            var val = FarmSystem.Instance.AddictiveLevelConfigArray[i];
            tipsBuilder.Append(val);
            tipsBuilder.Append(num >= (i + 1) ? "(已建造) " : " ");
        }
        uiGridList[id].UpdateTips(tipsBuilder.ToString());
    }

    private void FarmChangeAction(int id, GoodsData data)
    {
        var uiGrid = uiGridList[id];
        UpdateFarmUI(data, uiGrid);
    }

    private void UpdateFarmUI(GoodsData data, UIGrid uiGrid)
    {
        if (data == null)
        {
            uiGrid.UpdateTitle("空");
            uiGrid.UpdateNum(-1);
        }
        else
        {
            uiGrid.UpdateTitle($"{data.type.ToString()} - {data.state.ToString()}");
            if (data.isRiped)
                uiGrid.UpdateNum(data.num);
            else
                uiGrid.UpdateNum(-1);
        }
    }

    private void UpdateSowBtn(bool canSow)
    {
        btnTomato.gameObject.SetActive(canSow);
        btnPotato.gameObject.SetActive(canSow);
        btnCotton.gameObject.SetActive(canSow);
    }

    private void BtnOnClick_LevelUp()
    {
        FarmSystem.Instance.LevelUp();
        txtLevel.text = "lv" + FarmSystem.Instance.Level.ToString();
    }

    private void BtnOnClick_Reap()
    {
        FarmSystem.Instance.Reap();
    }

    private void BtnOnClick_Tomato()
    {
        FarmSystem.Instance.Sow(Goods.Tomato);
    }

    private void BtnOnClick_Potato()
    {
        FarmSystem.Instance.Sow(Goods.Potato);
    }

    private void BtnOnClick_Cotton()
    {
        FarmSystem.Instance.Sow(Goods.Cotton);
    }

    private void BtnOnClick_Close()
    {
        UIManager.Instance.CloseView<UIFarmView>();
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[ViewInfo(UILayer.PopUpWindow, UILife.Permanent)]
public class UIStoreView : ViewBase<UIStoreViewModel>
{
    [SerializeField] private Text txtLevel;
    [SerializeField] private Button btnLevelUp;
    [SerializeField] private Button btnTomato;
    [SerializeField] private Button btnPotato;
    [SerializeField] private Button btnCotton;
    [SerializeField] private Button btnClose;

    [SerializeField] private List<UIGrid> uiGridList = new List<UIGrid>();

    private void Awake()
    {
        btnLevelUp.onClick.AddListener(BtnOnClick_LevelUp);
        btnTomato.onClick.AddListener(BtnOnClick_Tomato);
        btnPotato.onClick.AddListener(BtnOnClick_Potato);
        btnCotton.onClick.AddListener(BtnOnClick_Cotton);
        btnClose.onClick.AddListener(BtnOnClick_Close);
    }

    protected override void OnShowStart(bool immediate)
    {
        base.OnShowStart(immediate);

        var goodDataDict = StoreSystem.Instance.GetGridData();
        var gridChangeActionDict = StoreSystem.Instance.GetGridChangeAction();
        for (int i = 0; i < uiGridList.Count; i++)
        {
            var uiGrid = uiGridList[i];
            uiGrid.SetId(i);
            var data = goodDataDict[i];
            UpdateStoreUI(data, uiGrid);
            UpdateAddictiveState(i);
            
            uiGrid.BtnAddictiveAction += LevelUpAddictive;
            gridChangeActionDict[i] += StoreChangeAction;
        }
    }

    protected override void OnHideFinish()
    {
        base.OnHideFinish();

        var gridChangeActionDict = StoreSystem.Instance.GetGridChangeAction();
        for (int i = 0; i < uiGridList.Count; i++)
        {
            var uiGrid = uiGridList[i];
            uiGrid.BtnAddictiveAction -= LevelUpAddictive;
            gridChangeActionDict[i] -= StoreChangeAction;
        }
    }

    private void LevelUpAddictive(int id)
    {
        StoreSystem.Instance.LevelUpAddictive(id);
        UpdateAddictiveState(id);
    }

    private void UpdateAddictiveState(int id)
    {
        StringBuilder tipsBuilder = new StringBuilder();
        var num =  StoreSystem.Instance.GetAddictiveNum(id);
        for (int i = 0; i <  StoreSystem.Instance.AddictiveLevelConfigArray.Length; i++)
        {
            var val = StoreSystem.Instance.AddictiveLevelConfigArray[i];
            tipsBuilder.Append(val);
            tipsBuilder.Append(num >= (i + 1) ? "(已建造) " : " ");
        }
        uiGridList[id].UpdateTips(tipsBuilder.ToString());
    }
    
    private void StoreChangeAction(int id, GridData data)
    {
        var uiGrid = uiGridList[id];
        UpdateStoreUI(data, uiGrid);
    }

    private void UpdateStoreUI(GridData data, UIGrid uiGrid)
    {
        if (data == null)
        {
            uiGrid.UpdateTitle("空");
            uiGrid.UpdateNum(-1);
        }
        else
        {
            uiGrid.UpdateTitle($"{data.type.ToString()}");
            uiGrid.UpdateNum(data.num);
        }
    }

    private void BtnOnClick_LevelUp()
    {
        StoreSystem.Instance.LevelUp();
        txtLevel.text = "lv" + StoreSystem.Instance.Level.ToString();
    }
    
    private void BtnOnClick_Tomato()
    {
        StoreSystem.Instance.UnStore(Goods.Tomato);
    }

    private void BtnOnClick_Potato()
    {
        StoreSystem.Instance.UnStore(Goods.Potato);
    }

    private void BtnOnClick_Cotton()
    {
        StoreSystem.Instance.UnStore(Goods.Cotton);
    }

    private void BtnOnClick_Close()
    {
        UIManager.Instance.CloseView<UIStoreView>();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[ViewInfo(UILayer.PopUpWindow, UILife.Permanent)]
public class UISupermarketView : ViewBase<UISupermarketViewModel>
{
    [SerializeField] private Text txtLevel;
    [SerializeField] private Button btnLevelUp;
    [SerializeField] private Button btnClose;

    [SerializeField] private List<UIGrid> uiGridList = new List<UIGrid>();

    private void Awake()
    {
        btnLevelUp.onClick.AddListener(BtnOnClick_LevelUp);
        btnClose.onClick.AddListener(BtnOnClick_Close);
    }

    protected override void OnShowStart(bool immediate)
    {
        base.OnShowStart(immediate);

        var goodDataDict = SupermarketSystem.Instance.GetGridData();
        var gridChangeActionDict = SupermarketSystem.Instance.GetGridChangeAction();
        for (int i = 0; i < uiGridList.Count; i++)
        {
            var uiGrid = uiGridList[i];
            uiGrid.SetId(i);
            var data = goodDataDict[i];
            UpdateSupermarketUI(data, uiGrid);
            UpdateAddictiveState(i);

            uiGrid.BtnAddictiveAction += LevelUpAddictive;

            gridChangeActionDict[i] += SupermarketChangeAction;
        }
    }

    protected override void OnHideFinish()
    {
        base.OnHideFinish();

        var gridChangeActionDict = SupermarketSystem.Instance.GetGridChangeAction();
        for (int i = 0; i < uiGridList.Count; i++)
        {
            var uiGrid = uiGridList[i];
            uiGrid.BtnAddictiveAction -= LevelUpAddictive;
            gridChangeActionDict[i] -= SupermarketChangeAction;
        }
    }
    
    private void LevelUpAddictive(int id)
    {
        SupermarketSystem.Instance.LevelUpAddictive(id);
        UpdateAddictiveState(id);
    }

    private void UpdateAddictiveState(int id)
    {
        StringBuilder tipsBuilder = new StringBuilder();
        var num =  SupermarketSystem.Instance.GetAddictiveNum(id);
        for (int i = 0; i <  SupermarketSystem.Instance.AddictiveLevelConfigArray.Length; i++)
        {
            var val = SupermarketSystem.Instance.AddictiveLevelConfigArray[i];
            tipsBuilder.Append(val);
            tipsBuilder.Append(num >= (i + 1) ? "(已建造) " : " ");
        }
        uiGridList[id].UpdateTips(tipsBuilder.ToString());
    }

    private void SupermarketChangeAction(int id, GridData data)
    {
        var uiGrid = uiGridList[id];
        UpdateSupermarketUI(data, uiGrid);
    }

    private void UpdateSupermarketUI(GridData data, UIGrid uiGrid)
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
        SupermarketSystem.Instance.LevelUp();
        txtLevel.text = "lv" + SupermarketSystem.Instance.Level.ToString();
    }

    private void BtnOnClick_Close()
    {
        UIManager.Instance.CloseView<UISupermarketView>();
    }
}

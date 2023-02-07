using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ViewInfo(UILayer.Menu, UILife.Permanent)]
public class UIMainView : ViewBase<UIMainViewModel>
{
    [SerializeField] private Text coinText;
    [SerializeField] private Button btnFarm;
    [SerializeField] private Button btnStore;
    [SerializeField] private Button btnSupermarket;

    private void Awake()
    {
        btnFarm.onClick.AddListener(BtnOnClick_Farm);
        btnStore.onClick.AddListener(BtnOnClick_Store);
        btnSupermarket.onClick.AddListener(BtnOnClick_Supermarket);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Binder.Add<int>("CoinVal", OnChangedCoinVal);
    }

    protected override void OnShowStart(bool immediate)
    {
        base.OnShowStart(immediate);

        coinText.text = "金币: " + Context.CoinVal.ToString();
    }

    private void OnChangedCoinVal(int oldVal, int newVal)
    {
        coinText.text = "金币: " + newVal.ToString();
    }

    private void BtnOnClick_Farm()
    {
        UIManager.Instance.OpenView<UIFarmView>();
    }

    private void BtnOnClick_Store()
    {
        UIManager.Instance.OpenView<UIStoreView>();
    }

    private void BtnOnClick_Supermarket()
    {
        UIManager.Instance.OpenView<UISupermarketView>();
    }
}
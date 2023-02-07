using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainViewModel : ViewModelBase
{
    public BindableProperty<int> CoinVal = new BindableProperty<int>();

    public override void OnShowStart()
    {
        base.OnShowStart();

        CoinVal.Value = PlayerData.Instance.CurCoin;
        PlayerData.Instance.CoinChangeAction += CoinChange;
    }

    public override void OnHideFinish()
    {
        base.OnHideFinish();
        
        PlayerData.Instance.CoinChangeAction -= CoinChange;
    }

    private void CoinChange(int val)
    {
        CoinVal.Value = val;
    }
}

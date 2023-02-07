using System;
using Newtonsoft.Json;

public class PlayerData : IJsonData<PlayerData>
{
    public PlayerDataParam playerData;

    public PlayerData()
    {
        string jsonStr = InitData("PlayerData", true, false);
        playerData = JsonConvert.DeserializeObject<PlayerDataParam>(jsonStr);
    }

    public void SavePlayerData()
    {
        var jsonStr = JsonConvert.SerializeObject(playerData);
        SaveData(jsonStr);
    }

    public int CurCoin
    {
        get => playerData.Coin;
        set
        {
            playerData.Coin = value;
            CoinChangeAction?.Invoke(value);
        }
    }

    public Action<int> CoinChangeAction = null;
}

public class PlayerDataParam
{
    public int Coin = 0; //金币
}
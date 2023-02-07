using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FarmSystem : SingletonBase<FarmSystem>
{
    public const int FarmNum = 4;

    private const int LevelGrowUpNum = 5;
    private const int LevelFactor = 1;

    private int _level = 1;

    public int Level
    {
        get => _level;
    }

    private int _factor = 1;

    private Dictionary<int, GoodsData> _goodsDict = new Dictionary<int, GoodsData>();
    private Dictionary<int, Action<int, GoodsData>> _farmChangeDict = new Dictionary<int, Action<int, GoodsData>>();
    public Action<bool> FarmFreeAction = null; //农场可种植回调

    public string[] AddictiveLevelConfigArray =
    {
        "稻草人", "稻草人", "自动洒水器", "自动洒水器", "自动洒水器","自动洒水器"
    };

    private Dictionary<int, int> _addictiveLevelDict = new Dictionary<int, int>();

    public override void Init()
    {
        _goodsDict.Clear();
        _farmChangeDict.Clear();
        _addictiveLevelDict.Clear();
        for (int i = 0; i < FarmNum; i++)
        {
            _goodsDict.Add(i, null);
            _farmChangeDict.Add(i, null);
            _addictiveLevelDict.Add(i, 0);
        }
    }

    public void Update(float deltaTime)
    {
        foreach (var kv in _goodsDict)
        {
            var data = kv.Value;
            if (data == null) continue;

            if (data.remainTime > 0)
            {
                data.remainTime -= deltaTime;
                var newState = (GoodsState) (2 - Mathf.Clamp(Mathf.CeilToInt(data.remainTime) / 10, 0, 2));
                if (newState != data.state)
                {
                    data.state = newState;
                    _farmChangeDict[kv.Key]?.Invoke(kv.Key, kv.Value);
                }
            }
            else if (!data.isRiped)
            {
                //果实已经成熟 可以收割
                data.isRiped = true;
                data.num = Mathf.FloorToInt(GetGoodsNum() * (1 + 0.05f * _addictiveLevelDict[kv.Key]));
                _farmChangeDict[kv.Key]?.Invoke(kv.Key, kv.Value);
            }
        }
    }

    private int FindEmptyFarm()
    {
        foreach (var kv in _goodsDict)
        {
            if (kv.Value == null)
                return kv.Key;
        }

        return -1;
    }

    /// <summary>
    /// 播种
    /// </summary>
    /// <param name="goodsType"></param>
    /// <returns></returns>
    public bool Sow(Goods goodsType)
    {
        int emptyFarmId = FindEmptyFarm();
        if (emptyFarmId == -1) return false;

        GoodsData data = new GoodsData();
        data.type = goodsType;
        data.state = GoodsState.Sowed;
        data.remainTime = 30; //30秒后成熟
        data.num = 0;
        _goodsDict[emptyFarmId] = data;
        _farmChangeDict[emptyFarmId]?.Invoke(emptyFarmId, data);

        FarmFreeAction?.Invoke(FindEmptyFarm() != -1);
        return true;
    }

    /// <summary>
    /// 收割成熟果实
    /// </summary>
    /// <returns></returns>
    public void Reap()
    {
        var keyList = new List<int>(_goodsDict.Keys);
        for (int i = 0; i < keyList.Count; i++)
        {
            var key = keyList[i];
            var value = _goodsDict[key];

            var data = value;
            if (data == null || !data.isRiped) continue;

            var isAllStore = StoreSystem.Instance.Store(data);
            //如果可以全部储藏
            if (isAllStore)
            {
                _goodsDict[key] = null;
                _farmChangeDict[key]?.Invoke(key, null);

                FarmFreeAction?.Invoke(FindEmptyFarm() != -1);
            }
        }
    }

    public Dictionary<int, GoodsData> GetFarmGoodsData()
    {
        return _goodsDict;
    }

    public Dictionary<int, Action<int, GoodsData>> GetFarmChangeAction()
    {
        return _farmChangeDict;
    }

    public int GetGoodsNum()
    {
        return _level * LevelGrowUpNum * _factor;
    }

    public void LevelUp()
    {
        // int cost = _level * 100;
        // if(PlayerData.Instance.CurCoin<=cost) return;

        if (_level >= 200) return;
        // PlayerData.Instance.CurCoin -= cost;
        _level++;

        _factor = _level / 20 * LevelFactor + 1;
    }

    public void LevelUpAddictive(int id)
    {
        var count = _addictiveLevelDict[id];
        if (count >= AddictiveLevelConfigArray.Length) return;

        _addictiveLevelDict[id]++;
    }

    public int GetAddictiveNum(int id)
    {
        return _addictiveLevelDict[id];
    }
}
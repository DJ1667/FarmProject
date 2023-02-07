using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    public Goods type;
    public int num; //已经放置的数量
}

public class StoreSystem : SingletonBase<StoreSystem>
{
    public const int GridNum = 4;
    private const int LevelGrowUpNum = 10;
    private const int LevelFactor = 1;

    private int _level = 1;

    public int Level
    {
        get => _level;
    }

    private int _factor = 1;

    private Dictionary<int, GridData> _gridDict = new Dictionary<int, GridData>();
    private Dictionary<int, Action<int, GridData>> _gridChangeDict = new Dictionary<int, Action<int, GridData>>();

    public string[] AddictiveLevelConfigArray =
    {
        "风车", "风车", "烘干机", "烘干机", "烘干机","烘干机"
    };

    private Dictionary<int, int> _addictiveLevelDict = new Dictionary<int, int>();

    public override void Init()
    {
        _gridDict.Clear();
        _gridChangeDict.Clear();
        _addictiveLevelDict.Clear();

        for (int i = 0; i < GridNum; i++)
        {
            _gridDict.Add(i, null);
            _gridChangeDict.Add(i, null);
            _addictiveLevelDict.Add(i, 0);
        }
    }

    /// <summary>
    /// 储藏
    /// </summary>
    /// <param name="goodsData"></param>
    /// <returns>是否可以全部储藏进去</returns>
    public bool Store(GoodsData goodsData)
    {
        int capacity = 0;
        foreach (var kv in _gridDict)
        {
            capacity = GetSingleCapacity(kv.Key);
            var gridData = kv.Value;
            if (gridData == null) continue;

            if (gridData.type == goodsData.type)
            {
                if (gridData.num + goodsData.num <= capacity)
                {
                    gridData.num += goodsData.num;
                    return true;
                }
            }
        }

        //还有剩余空格子
        var emptyId = FindEmptyGrid();
        capacity = GetSingleCapacity(emptyId);
        if (emptyId != -1 && capacity > goodsData.num)
        {
            GridData gridData = new GridData();
            gridData.type = goodsData.type;
            gridData.num = goodsData.num;

            _gridDict[emptyId] = gridData;
            return true;
        }

        return false;
    }

    private int FindEmptyGrid()
    {
        foreach (var kv in _gridDict)
        {
            if (kv.Value == null)
                return kv.Key;
        }

        return -1;
    }

    public int GetCurrentCapacity()
    {
        return _level * LevelGrowUpNum * _factor;
    }

    public int GetSingleCapacity(int id)
    {
        return Mathf.FloorToInt(GetCurrentCapacity() * (1 + 0.05f * _addictiveLevelDict[id]));
    }

    public void UnStore(Goods type)
    {
        var keyList = new List<int>(_gridDict.Keys);
        for (int i = 0; i < keyList.Count; i++)
        {
            var key = keyList[i];
            var value = _gridDict[key];

            var data = value;
            if(data== null) continue;
            if (data.type == type)
            {
                var isAllStore = SupermarketSystem.Instance.Store(data);
                if (isAllStore)
                {
                    _gridDict[key] = null;
                    _gridChangeDict[key]?.Invoke(key, null);
                }
                else
                {
                    _gridChangeDict[key]?.Invoke(key, value);
                }
            }
        }
    }

    public Dictionary<int, GridData> GetGridData()
    {
        return _gridDict;
    }

    public Dictionary<int, Action<int, GridData>> GetGridChangeAction()
    {
        return _gridChangeDict;
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
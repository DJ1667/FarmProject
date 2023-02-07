using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupermarketSystem : SingletonBase<SupermarketSystem>
{
    public const int GridNum = 4;
    private const int LevelGrowUpNum = 2;
    private const int LevelFactor = 1;

    private const float CustomerSpan = 10;
    private float _customerTimer = 0;

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
        "绿植", "绿植", "展示货架", "展示货架", "展示货架","展示货架"
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

    public void Update(float delatTime)
    {
        _customerTimer += delatTime;
        if (_customerTimer >= CustomerSpan)
        {
            _customerTimer = 0;
            Buy();
        }
    }

    /// <summary>
    /// 储藏
    /// </summary>
    /// <param name="goodsData"></param>
    /// <returns>是否可以全部储藏进去</returns>
    public bool Store(GridData newData)
    {
        int capacity = 0;

        foreach (var kv in _gridDict)
        {
            capacity = GetSingleCapacity(kv.Key);
            var data = kv.Value;
            if(data==null) continue;
            
            if (data.type == newData.type)
            {
                if (data.num + newData.num <= capacity)
                {
                    data.num += newData.num;
                    return true;
                }
                else
                {
                    newData.num -= (capacity - data.num);
                    data.num = capacity;
                }
            }
        }

        //还有剩余空格子
        var emptyId = FindEmptyGrid();
        capacity = GetSingleCapacity(emptyId);
        if (emptyId != -1)
        {
            GridData gridData = new GridData();
            gridData.type = newData.type;

            _gridDict[emptyId] = gridData;
            if (capacity >= newData.num)
            {
                gridData.num = newData.num;
                return true;
            }
            else
            {
                gridData.num = capacity;
                newData.num -= capacity;
            }
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

    /// <summary>
    /// 顾客直接购买一个格子所有货物
    /// </summary>
    public void Buy()
    {
        var keyList = new List<int>(_gridDict.Keys);
        for (int i = 0; i < keyList.Count; i++)
        {
            var key = keyList[i];
            var value = _gridDict[key];

            var data = value;
            if (data == null) continue;

            int price = (int) (data.type + 1) * data.num;
            PlayerData.Instance.CurCoin += price;
            _gridDict[key] = null;
            _gridChangeDict[key]?.Invoke(key,null);
        }
    }

    public int GetCurrentCapacity()
    {
        return _level * LevelGrowUpNum * _factor;
    }
    
    
    public int GetSingleCapacity(int id)
    {
        return Mathf.FloorToInt(GetCurrentCapacity() * (1 + 0.05f * _addictiveLevelDict[id]));
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
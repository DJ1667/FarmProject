using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsData
{
    public Goods type;
    public GoodsState state;
    public float remainTime = 0;//收获倒计时
    public bool isRiped = false;//是否已经可收割
    public int num=0;//数量
}

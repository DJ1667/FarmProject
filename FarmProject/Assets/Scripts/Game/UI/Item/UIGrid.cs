using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGrid : MonoBehaviour
{
    private Text txtTitle;
    private Text txtNum;
    private Text txtTips;
    private Button btnAddictive;

    public Action<int> BtnAddictiveAction = null;

    private int _id = 1;

    private void Awake()
    {
        txtTitle = transform.Find("Title").GetComponent<Text>();
        txtNum = transform.Find("Num").GetComponent<Text>();
        txtTips = transform.Find("Tips").GetComponent<Text>();
        btnAddictive = transform.Find("AdditiveLevelUp").GetComponent<Button>();
        
        btnAddictive.onClick.AddListener(() =>
        {
            BtnAddictiveAction?.Invoke(_id);
        });
    }

    public void SetId(int id)
    {
        _id = id;
    }

    public void UpdateTitle(string title)
    {
        txtTitle.text = title;
    }

    public void UpdateNum(int num)
    {
        if (num == -1)
        {
            txtNum.gameObject.SetActive(false);
        }
        else
        {
            txtNum.gameObject.SetActive(true);
            txtNum.text = num.ToString();
        }
    }
    
    public void UpdateTips(string title)
    {
        txtTips.text = title;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private void Start()
    {
        FarmSystem.Instance.Init();
        StoreSystem.Instance.Init();
        SupermarketSystem.Instance.Init();
        
        UIManager.Instance.OpenView<UIMainView>();
    }

    private void Update()
    {
        FarmSystem.Instance.Update(Time.deltaTime);
        SupermarketSystem.Instance.Update(Time.deltaTime);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        PlayerData.Instance.SavePlayerData();
    }

    private void OnApplicationQuit()
    {
        PlayerData.Instance.SavePlayerData();
    }
}
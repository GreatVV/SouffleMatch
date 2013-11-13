﻿using System;
using UnityEngine;

public class Economy : MonoBehaviour
{
    public static Economy Instance;
    public int Money;

    public int CurrentMoney
    {
        get { return Money; }
        private set
        {
            Money = value;
            InvokeMoneyChanged();
        }
    }

    #region Events

    public event Action<int> MoneyChanged;

    #endregion

    #region Event Invokators

    protected virtual void InvokeMoneyChanged()
    {
        var handler = MoneyChanged;
        if (handler != null) handler(CurrentMoney);
    }

    #endregion

    public bool Spent(int amount)
    {
        if (amount <= CurrentMoney)
        {
            CurrentMoney -= amount;
            return true;
        }
        return false;
    }

    public void Add(int amount)
    {
        CurrentMoney += amount;
    }

    private void Awake()
    {
        Instance = this;
        //TODO load from files
    }

    public JSONObject Serialize()
    {
        var json = new JSONObject(JSONObject.Type.OBJECT);
        json.AddField("Money", CurrentMoney);
        return json;
    }

    public void Unserialize(JSONObject json)
    {
        CurrentMoney = (int) json.GetField("Money").n;
    }
}
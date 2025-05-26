using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    private void Awake()
    {
        if (Instance != null) return;
        Instance = this;
    }

    private readonly Dictionary<ItemType,int> _inventory = new Dictionary<ItemType, int>();
    public void AddItem(ItemType type)
    {
        if (_inventory.TryAdd(type, 0)) return;
        
        _inventory[type] += 1;
        Debug.Log(type + $" added to Inventory, You have : {_inventory[type] }");

    } 
}

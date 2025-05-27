using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public Action<Dictionary<ItemType, int>> OnInventoryChanged;
    private readonly Dictionary<ItemType,int> _inventory = new Dictionary<ItemType, int>();
    private void Awake()
    {
        if (Instance != null) return;
        Instance = this;
    }
    public void AddItem(ItemType type)
    {
        if (_inventory.TryAdd(type, 0)) return;
        
        _inventory[type] += 1;
        Debug.Log(type + $" added to Inventory, You have : {_inventory[type] }");
        OnInventoryChanged?.Invoke(_inventory);
    } 
    
    public Dictionary<ItemType, int> GetInventory()
    {
        return _inventory;
    }
}

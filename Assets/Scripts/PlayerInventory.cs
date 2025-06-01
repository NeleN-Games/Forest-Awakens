using System;
using System.Collections.Generic;
using Enums;
using Models;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    public Action<Dictionary<SourceType, int>> OnInventoryChanged;
    private readonly Dictionary<SourceType,int> _inventory = new Dictionary<SourceType, int>();
    private void Awake()
    {
        if (Instance != null) return;
        Instance = this;
    }
    public void AddItem(SourceType type)
    {
        _inventory.TryAdd(type, 0);
        _inventory[type] += 1;
        Debug.Log(type + $" added to Inventory, You have : {_inventory[type] }");
        OnInventoryChanged?.Invoke(_inventory);
    } 
    
    public Dictionary<SourceType, int> GetInventory()
    {
        return _inventory;
    }

    public bool HasEnoughSources(List<SourceRequirement> sources)
    {
        foreach (var source in sources)
        {
            if ( _inventory[source.sourceType]>=source.amount )
            {
              continue;
            }
            return false;
        }

        foreach (var source in sources)
        {
            _inventory[source.sourceType] -= source.amount;
        }
        OnInventoryChanged?.Invoke(_inventory);
        return true;
    }
}

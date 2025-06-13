using System;
using System.Collections.Generic;
using Enums;
using Hud;
using Interfaces;
using Managers;
using Models;
using Services;
using UnityEngine;

public class PlayerInventory : MonoBehaviour,IInitializable, IInventoryService
{
    public Action<Dictionary<SourceType, int>> OnInventoryChanged;
    public Action<SourceType, int> OnSourceAmountChanged;
    private readonly Dictionary<SourceType,int> _inventory = new Dictionary<SourceType, int>();
    public void Initialize()
    {
        OnInventoryChanged += ServiceLocator.Get<InventoryUI>().RefreshInventory;
        OnSourceAmountChanged += ServiceLocator.Get<CraftLookupManager>().OnSourceAmountChanged;
    }

    public void OnDestroy()
    {
        OnInventoryChanged -= ServiceLocator.Get<InventoryUI>().RefreshInventory;
        OnSourceAmountChanged -= ServiceLocator.Get<CraftLookupManager>().OnSourceAmountChanged;
    }
    public int GetSourceAmount(SourceType sourceType)
    {
        return _inventory.GetValueOrDefault(sourceType, 0);
    }

    public Dictionary<SourceType, int> GetInventory()
    {
        return _inventory;
    }
    public void AddItem(SourceType type,int amount)
    {
        _inventory.TryAdd(type, 0);
        _inventory[type] += amount;
        Debug.Log(type + $" added to Inventory, You have : {_inventory[type] }");
        OnInventoryChanged?.Invoke(_inventory);
        OnSourceAmountChanged?.Invoke(type,_inventory[type]);
    } 
    
   
    private void RemoveSource(SourceType type,int amount)
    {
        _inventory[type] -= amount;
        Debug.Log(type + $" removed from Inventory, You have : {_inventory[type] }");
        OnInventoryChanged?.Invoke(_inventory);
        OnSourceAmountChanged?.Invoke(type,_inventory[type]);
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
            RemoveSource(source.sourceType, source.amount);
        }
        OnInventoryChanged?.Invoke(_inventory);
        return true;
    }
    
}

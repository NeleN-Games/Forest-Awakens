using System.Collections.Generic;
using Enums;
using Models.Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;

    private static Dictionary<ItemType, ItemData> _itemDict;

    public static void Initialize(ItemDatabase db)
    {
        _itemDict = new Dictionary<ItemType, ItemData>();
        foreach (var item in db.items)
        {
            _itemDict[item.type] = item;
        }
    }

    public static ItemData GetItem(ItemType type)
    {
        if (_itemDict[type].IsValid()) return _itemDict[type];
        
        Debug.LogError($"Item of type {type} has missing or invalid data!");
        return null;

    }
}
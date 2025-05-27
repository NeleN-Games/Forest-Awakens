using System.Collections.Generic;
using Enums;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public ItemType type;
    public string itemName;
    public Sprite icon;
}

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
        return _itemDict[type];
    }
}
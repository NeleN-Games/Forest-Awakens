using System.Collections.Generic;
using Tech_Tree;
using UnityEngine;

public class TechTreeSystem : MonoBehaviour
{
    public TechTreeDatabase techDatabase;

    private Dictionary<string, TechItem> techLookup;
    public TextAsset jsonFile; // برای درگ‌کردن فایل JSON از Inspector

    void Awake()
    {
        techLookup = new Dictionary<string, TechItem>();
        foreach (var item in techDatabase.techItems)
        {
            techLookup[item.name] = item;
        }
    }
    public TechItem GetItem(string name)
    {
        return techLookup.ContainsKey(name) ? techLookup[name] : null;
    }

    public bool CanCraft(string name, Dictionary<string, int> inventory)
    {
        var item = GetItem(name);
        if (item == null) return false;

        // Check prerequisites
        foreach (var pre in item.prerequisites)
        {
            if (!inventory.ContainsKey(pre))
                return false;
        }

        // Check materials
        foreach (var mat in item.materials)
        {
            if (!inventory.ContainsKey(mat.materialName) || inventory[mat.materialName] < mat.quantity)
                return false;
        }

        return true;
    }

    public void Craft(string name, Dictionary<string, int> inventory)
    {
        if (!CanCraft(name, inventory)) return;

        var item = GetItem(name);
        foreach (var mat in item.materials)
        {
            inventory[mat.materialName] -= mat.quantity;
        }

        if (!inventory.ContainsKey(name)) inventory[name] = 0;
        inventory[name]++;
        Debug.Log($"Crafted: {name}");
    }

    [ContextMenu("Import JSON to ScriptableObject")]
    public void ImportFromJson()
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON file is missing!");
            return;
        }

        techDatabase.techItems = JsonUtilityWrapper.LoadTechItems(jsonFile.text);
        Debug.Log("Tech Tree imported.");
    }

}
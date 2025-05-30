using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace Tech_Tree
{
    public class JsonUtilityWrapper
    {
        public static List<RawTechItem> LoadRawTechItems(string json)
        {
            var list = new List<RawTechItem>();
            var arr = JSON.Parse(json).AsArray;
            foreach (var node in arr)
            {
                var obj = node.Value;
                var item = new RawTechItem();
                item.id = obj["id"].AsInt;
                item.name = obj["name"];
                item.category = obj["category"];
                item.type = obj["type"];
                item.description = obj["description"];
                item.effect = obj["effect"];
                // Parse materials as Dictionary<string, int>
                item.materials = new Dictionary<string, int>();
                var matNode = obj["materials"];
                if (matNode != null && matNode.IsObject)
                {
                    
                    foreach (var mat in matNode.Keys)
                    {
                        item.materials[mat] = matNode[mat.AsInt].AsInt;
                    }
                }
                // Parse prerequisites as List<string>
                item.prerequisites = new List<string>();
                var preNode = obj["prerequisites"];
                if (preNode != null && preNode.IsArray)
                {
                    foreach (var pre in preNode.AsArray)
                        item.prerequisites.Add(pre.Value);
                }
                list.Add(item);
            }
            return list;
        }

        public static List<TechItem> LoadTechItems(string json) {
            var rawItems = LoadRawTechItems(json);
            var finalList = new List<TechItem>();

            foreach (var raw in rawItems) {
                finalList.Add(raw.ToTechItem());
            }

            return finalList;
        }
    }

}

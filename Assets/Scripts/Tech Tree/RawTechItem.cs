using System;
using System.Collections.Generic;

namespace Tech_Tree
{
    [Serializable]
    public class RawTechItem {
        public int id;
        public string name;
        public string category;
        public string type;
        public string description;
        public string effect;
        public Dictionary<string, int> materials;
        public List<string> prerequisites;

        public TechItem ToTechItem() {
            return new TechItem {
                id = id,
                name = name,
                category = category,
                type = type,
                description = description,
                effect = effect,
                prerequisites = prerequisites,
                materials = ConvertMaterials()
            };
        }

        private List<MaterialRequirement> ConvertMaterials() {
            var list = new List<MaterialRequirement>();
            foreach (var kv in materials) {
                list.Add(new MaterialRequirement {
                    materialName = kv.Key,
                    quantity = kv.Value
                });
            }
            return list;
        }
    }
}

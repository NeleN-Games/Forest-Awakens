using System;
using System.Collections.Generic;

[Serializable]
public class TechItem {
    public int id;
    public string name;
    public string category;
    public string type; // نوع آیتم (مثلاً technology, building, resource, ...)
    public string description; // توضیح کوتاه
    public string effect; // اثر یا خروجی
    public List<MaterialRequirement> materials;
    public List<string> prerequisites;
}


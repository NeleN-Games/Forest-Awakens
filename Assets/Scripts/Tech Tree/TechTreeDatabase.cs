using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TechTreeDatabase", menuName = "TechTree/Create Database")]
public class TechTreeDatabase : ScriptableObject
{
    public List<TechItem> techItems;
}
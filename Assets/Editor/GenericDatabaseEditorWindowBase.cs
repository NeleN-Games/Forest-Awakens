using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public abstract class GenericDatabaseEditorWindowBase 
    {
        public abstract void OnGUIContents();
    }
}
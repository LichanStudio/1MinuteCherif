using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonstersRegistry))]
public class MonstersRegistryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MonstersRegistry registry = (MonstersRegistry)target;
        if (GUILayout.Button("Refresh"))
        {
            registry.RefreshInEditor();
        }
    }
}
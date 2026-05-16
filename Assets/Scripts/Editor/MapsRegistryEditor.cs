using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapsRegistry))]
public class MapsRegistryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapsRegistry registry = (MapsRegistry)target;
        if (GUILayout.Button("Refresh"))
        {
            registry.RefreshInEditor();
        }
    }
}
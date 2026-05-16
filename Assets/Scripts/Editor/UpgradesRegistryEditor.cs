using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpgradesRegistry))]
public class UpgradesRegistryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UpgradesRegistry registry = (UpgradesRegistry)target;
        if (GUILayout.Button("Refresh"))
        {
            registry.RefreshInEditor();
        }
    }
}
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileTexturesRegistry))]
public class TileTextresRegistryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileTexturesRegistry registry = (TileTexturesRegistry)target;
        if (GUILayout.Button("Refresh"))
        {
            registry.RefreshInEditor();
        }
    }
}
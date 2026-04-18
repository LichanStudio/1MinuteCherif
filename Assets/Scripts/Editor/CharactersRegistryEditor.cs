using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharactersRegistry))]
public class CharactersRegistryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CharactersRegistry registry = (CharactersRegistry)target;
        if (GUILayout.Button("Refresh"))
        {
            registry.RefreshInEditor();
        }
    }
}
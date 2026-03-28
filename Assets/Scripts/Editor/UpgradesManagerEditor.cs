using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpgradesManager))]
public class UpgradesManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UpgradesManager manager = (UpgradesManager)target;
        if (GUILayout.Button("Refresh List"))
        {
            manager.RefreshEntityList();
        }
    }
}
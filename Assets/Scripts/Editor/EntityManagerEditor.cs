using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityManager))]
public class EntityManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EntityManager manager = (EntityManager)target;
        if (GUILayout.Button("Refresh List"))
        {
            manager.RefreshEntityList();
        }
    }
}
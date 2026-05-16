using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "MapsRegistry", menuName = "ScriptableObjects/Registries/Maps")]
public class MapsRegistry : ScriptableObject
{
    public List<MapData> Maps = new();

    public void RefreshInEditor()
    {
#if UNITY_EDITOR
        Maps.Clear();

        string searchFilter = "t:MapData";
        string[] guids = AssetDatabase.FindAssets(searchFilter);

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MapData data = AssetDatabase.LoadAssetAtPath<MapData>(path);
            if (data != null) Maps.Add(data);
        }

        Debug.Log($"Total de maps ajoutés : {Maps.Count}");
        UnityEditor.EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
}
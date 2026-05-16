using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TileTexturesRegistry", menuName = "ScriptableObjects/Registries/TileTextures")]
public class TileTexturesRegistry : ScriptableObject
{
    public List<TileTexture> Textures = new();

    public void RefreshInEditor()
    {
#if UNITY_EDITOR
        Textures.Clear();

        string searchFilter = "t:TileTexture";
        string[] guids = AssetDatabase.FindAssets(searchFilter);

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TileTexture data = AssetDatabase.LoadAssetAtPath<TileTexture>(path);
            if (data != null) Textures.Add(data);
        }

        Debug.Log($"Tile texture data added : {Textures.Count}");
        UnityEditor.EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
}

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "CharactersRegistry", menuName = "ScriptableObjects/Registries/Characters")]
public class CharactersRegistry : ScriptableObject
{
    public List<CharacterData> characters = new();

    public void RefreshInEditor()
    {
#if UNITY_EDITOR
        characters.Clear();

        string searchFilter = "t:CharacterData";
        string[] guids = AssetDatabase.FindAssets(searchFilter);

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CharacterData data = AssetDatabase.LoadAssetAtPath<CharacterData>(path);
            if (data != null) characters.Add(data);
        }

        Debug.Log($"Characters data added : {characters.Count}");
        UnityEditor.EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
}
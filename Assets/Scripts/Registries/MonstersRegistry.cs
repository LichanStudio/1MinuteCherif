using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MonstersRegistry", menuName = "ScriptableObjects/Registries/Monsters")]
public class MonstersRegistry : ScriptableObject
{
    public List<MonsterData> entities = new();

    public void RefreshInEditor()
    {
        entities.Clear();

        string searchFilter = "t:MonsterData";
        string[] guids = AssetDatabase.FindAssets(searchFilter);

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MonsterData data = AssetDatabase.LoadAssetAtPath<MonsterData>(path);
            if (data != null) entities.Add(data);
        }

        Debug.Log($"Total de monstres ajoutés : {entities.Count}");
        UnityEditor.EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public MonsterData GetRandomMonster()
    {
        if (entities.Count == 0) return null;
        int randomIndex = Random.Range(0, entities.Count);
        return entities[randomIndex];
    }
}
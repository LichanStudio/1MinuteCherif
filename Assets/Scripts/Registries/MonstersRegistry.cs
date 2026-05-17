using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "MonstersRegistry", menuName = "ScriptableObjects/Registries/Monsters")]
public class MonstersRegistry : ScriptableObject
{
    public List<MonsterData> entities = new();

    public void RefreshInEditor()
    {
#if UNITY_EDITOR
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
#endif
    }

    public MonsterData GetRandomMonster(List<MapData.MonsterSpawnData> monstersSpawnData, float totalMonsterProb)
    {
        if (monstersSpawnData == null || monstersSpawnData.Count == 0) return null;

        float randomValue = Random.Range(0f, totalMonsterProb);
        MapData.MonsterSpawnData selectedData = monstersSpawnData[0];
        float cumulativeProb = 0f;
        Debug.Log($"Random Value: {randomValue}, Total Monster Prob: {totalMonsterProb}");
        foreach (MapData.MonsterSpawnData spawnData in monstersSpawnData)
        {
            if(cumulativeProb <= randomValue) selectedData = spawnData;
            else break;
            cumulativeProb += spawnData.GetActualSpawnChance();
        }

        return selectedData.Monster;
    }
}
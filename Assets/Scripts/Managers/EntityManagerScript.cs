using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityManager", menuName = "ScriptableObjects/Managers/Entity", order = 1)]
public class EntityManager : ScriptableObject
{
    [Header("Managers")]
    [SerializeField] PlayerDataManager _playerDataManager;

    [Header("Objects")]
    public List<Entity> EntityList = new();

    private readonly Dictionary<string, Entity> _entityMapper = new();
    private PlayerDataManager _enemyDataManager;

    public void OnEnable()
    {
        UpdateEntityList();
        _enemyDataManager = Instantiate(_playerDataManager);
        OnSessionStart();
    }

    public void OnDisable()
    {
        _entityMapper.Clear();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) RefreshEntityList();
#endif
    }

    public void UpdateEntityList()
    {
        _entityMapper.Clear();
        foreach (Entity entity in EntityList)
        {
            if (!_entityMapper.ContainsKey(entity.GetId())) _entityMapper.Add(entity.GetId(), entity);
        }
    }

    public void RefreshEntityList()
    {
#if UNITY_EDITOR
        EntityList.Clear();

        string[] foldersToSearch = { "Assets/Data" };
        string[] guids = AssetDatabase.FindAssets("t:Entity");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Entity entity = AssetDatabase.LoadAssetAtPath<Entity>(path);
            if (entity != null)
            {
                EntityList.Add(entity);
            }
        }

        UpdateEntityList();

        EditorUtility.SetDirty(this);
        if (!Application.isPlaying) AssetDatabase.SaveAssets();
#endif
    }

    public void OnSessionStart()
    {
        _enemyDataManager.ResetBaseValues();
        _enemyDataManager.ResetAdditionnalValues();
    }

    public void OnSelectUpgrade(CalculatedUpgradeClass playerUpgrade, CalculatedUpgradeClass upgrade)
    {
        _enemyDataManager.OnSelectUpgrade(upgrade);
    }

    public PlayerDataManager GetEnemyData()
    {
        return _enemyDataManager;
    }
}

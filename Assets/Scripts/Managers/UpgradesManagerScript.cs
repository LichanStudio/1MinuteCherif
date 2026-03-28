using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradesManager", menuName = "ScriptableObjects/Managers/Upgrades", order = 1)]
public class UpgradesManager : ScriptableObject
{
    [Header("Objects")]
    public List<Upgrade> UpgradesList = new();
    private List<Upgrade> _playerUpgrades = new();
    private List<Upgrade> _enemyUpgrades = new();

    private readonly Dictionary<string, Upgrade> _upgradesMapper = new();

    public void OnEnable()
    {
        UpdateEntityList();
    }

    public void OnDisable()
    {
        _upgradesMapper.Clear();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) RefreshEntityList();
#endif
    }

    public void UpdateEntityList()
    {
        _upgradesMapper.Clear();
        _playerUpgrades.Clear();
        _enemyUpgrades.Clear();
        foreach (Upgrade upgrade in UpgradesList)
        {
            if (!_upgradesMapper.ContainsKey(upgrade.GetId())) _upgradesMapper.Add(upgrade.GetId(), upgrade);
            if (upgrade.GetUpgradeData().IsEnemyUpgrade) _enemyUpgrades.Add(upgrade);
            else _playerUpgrades.Add(upgrade);
        }
    }

    public void RefreshEntityList()
    {
#if UNITY_EDITOR
        UpgradesList.Clear();

        string[] foldersToSearch = { "Assets/Data/Upgrades" };
        string[] guids = AssetDatabase.FindAssets("t:Upgrade");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Upgrade upgrade = AssetDatabase.LoadAssetAtPath<Upgrade>(path);
            if (upgrade != null)
            {
                UpgradesList.Add(upgrade);
            }
        }

        UpdateEntityList();

        EditorUtility.SetDirty(this);
        if (!Application.isPlaying) AssetDatabase.SaveAssets();
#endif
    }

    public List<Upgrade> GenerateLoots(bool goingWrong)
    {
        List<Upgrade> generatedLoots = new();
        int generatedUpgradesCount = 1;
        int _enemyUpgradesCount = 1;

        if (goingWrong)
        {
            generatedUpgradesCount += UnityEngine.Random.Range(1, 2);
            _enemyUpgradesCount += UnityEngine.Random.Range(1, 3);
        }
        else
        {
            generatedUpgradesCount += UnityEngine.Random.Range(1, 4);
            _enemyUpgradesCount += UnityEngine.Random.Range(0, 1);
        }
        for (int i = 0; i < generatedUpgradesCount; i++)
        {
            Upgrade upgrade = _playerUpgrades[UnityEngine.Random.Range(0, _playerUpgrades.Count)].Clone();
            generatedLoots.Add(upgrade);
        }
        for (int i = 0; i < _enemyUpgradesCount; i++)
        {
            Upgrade upgrade = _enemyUpgrades[UnityEngine.Random.Range(0, _enemyUpgrades.Count)].Clone();
            generatedLoots.Add(upgrade);
        }
        return generatedLoots;
    }

    public Upgrade GetRandomDefUpgrade()
    {
        Upgrade upgrade = _playerUpgrades[UnityEngine.Random.Range(0, _playerUpgrades.Count)].Clone();
        return upgrade;
    }
}

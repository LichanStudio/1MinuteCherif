using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "UpgradesRegistry", menuName = "ScriptableObjects/Registries/Upgrades")]
public class UpgradesRegistry : ScriptableObject
{
    public List<UpgradeData> upgrades = new();

    public class UpgradeConstraint
    {
        public bool isEnemyUpgrade = false;
        public bool bounceEnabled = false;
        public bool pierceEnabled = false;
        public bool multiShotEnabled = false;
        public bool multiHitEnabled = false;
    }

    public void RefreshInEditor()
    {
#if UNITY_EDITOR
        upgrades.Clear();

        string searchFilter = "t:UpgradeData";
        string[] guids = AssetDatabase.FindAssets(searchFilter);

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            UpgradeData data = AssetDatabase.LoadAssetAtPath<UpgradeData>(path);
            if (data != null) upgrades.Add(data);
        }

        Debug.Log($"Total d'upgrades ajoutés : {upgrades.Count}");
        UnityEditor.EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }

    public UpgradeData GetRandomUpgrade(UpgradeConstraint constraint)
    {
        if (upgrades.Count == 0) return null;
        Debug.Log(constraint.pierceEnabled);
        List<UpgradeData> filteredEntities = upgrades.FindAll(upgrade =>
            upgrade.GetUpgradeType() == UpgradeData.UpgradeType.Default ||
            (constraint.bounceEnabled && upgrade.GetUpgradeType() == UpgradeData.UpgradeType.Bounce) ||
            (constraint.pierceEnabled && upgrade.GetUpgradeType() == UpgradeData.UpgradeType.Piercing) ||
            (constraint.multiShotEnabled && upgrade.GetUpgradeType() == UpgradeData.UpgradeType.MultiShot) ||
            (constraint.multiHitEnabled && upgrade.GetUpgradeType() == UpgradeData.UpgradeType.MultiHit)
        );
        if (filteredEntities.Count == 0) return null;
        int randomIndex = Random.Range(0, filteredEntities.Count);
        return filteredEntities[randomIndex];
    }
}
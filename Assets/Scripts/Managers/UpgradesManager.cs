using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    public static UpgradesManager Instance { get; private set; }

    [Header("Informations")]
    [SerializeField] private UpgradesRegistry _upgradesRegistry;

    private const int UPGRADES_FOR_MONSTERS = 3;
    private const int UPGRADES_FOR_CHARACTERS = 4;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public Stats GetTemporaryUpgrades(bool enemy)
    {
        Stats result = new();
        if(enemy)
        {
            UpgradesRegistry.UpgradeConstraint constraint = new()
            {
                isEnemyUpgrade = true,
                bounceEnabled = false,
                pierceEnabled = false,
                multiShotEnabled = false,
                multiHitEnabled = false
            };
            for (int i = 0; i < UPGRADES_FOR_MONSTERS; i++)
            {
                UpgradeData upgradeData = _upgradesRegistry.GetRandomUpgrade(constraint);
                if (upgradeData != null) result += upgradeData.GetRandomStat();
            }
        }
        else if(CharacterManager.Instance.SelectedCharacter != null && CharacterManager.Instance.SelectedCharacter.WeaponData != null)
        {
            WeaponData weaponData = CharacterManager.Instance.SelectedCharacter.WeaponData;
            UpgradesRegistry.UpgradeConstraint constraint = new()
            {
                isEnemyUpgrade = false,
                bounceEnabled = weaponData.IsBouncingEnabled(),
                pierceEnabled = weaponData.IsPiercingEnabled(),
                multiShotEnabled = weaponData.IsMultiShotEnabled(),
                multiHitEnabled = weaponData.IsMultiHitEnabled()
            };
            for (int i = 0; i < UPGRADES_FOR_CHARACTERS; i++)
            {
                UpgradeData upgradeData = _upgradesRegistry.GetRandomUpgrade(constraint);
                if (upgradeData != null) result += upgradeData.GetRandomStat();
            }
        }
        return result;
    }
}

using UnityEngine;

public class GlobalUIScript : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private ChestLootsScript _upgradeUI;

    public void OnEnable()
    {
        ActionsManager.OnStartUpgradeSelection += ShowUpgradeSelection;
        ActionsManager.OnSelectUpgrade += HideUpgradeSelection;
    }

    public void OnDisable()
    {
        ActionsManager.OnStartUpgradeSelection -= ShowUpgradeSelection;
        ActionsManager.OnSelectUpgrade += HideUpgradeSelection;
    }

    private void ShowUpgradeSelection()
    {
        _upgradeUI.GenerateLoots();
    }

    private void HideUpgradeSelection(Stats playerUpgrades, Stats enemyUpgrades)
    {
        _upgradeUI.gameObject.SetActive(false);
    }
}

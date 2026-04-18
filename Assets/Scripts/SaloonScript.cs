using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SaloonScript : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private MovementManager _movementManager;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private UpgradesManager _upgradesManager;

    [Header("Settings")]
    [SerializeField] private TilesGeneratorScript _tilesGenerator;
    [SerializeField] private GameObject _spawnPoint;
    [SerializeField] private List<DefinitiveUpgradeScript> _definitiveUpgrades;

    public void OnEnable()
    {
        ActionsManager.OnTryBuyUpgrade += OnTryBuyUpgrade;
    }

    public void OnDisable()
    {
        ActionsManager.OnTryBuyUpgrade -= OnTryBuyUpgrade;
    }

    public void TeleportPlayerIn()
    {
        if (_spawnPoint == null || _movementManager == null) return;
        _movementManager.TeleportPlayer(_spawnPoint.transform.position);
        foreach(DefinitiveUpgradeScript defUpgrade in _definitiveUpgrades)
        {
            Upgrade newDefUpgrade = _upgradesManager.GetRandomDefUpgrade();
            newDefUpgrade.GetUpgradeData().CalculateDefValue();
            defUpgrade.SetUpgrade(newDefUpgrade);
            defUpgrade.gameObject.SetActive(true);
        }
    }

    public void OnTryBuyUpgrade(Upgrade upgrade)
    {
        /*if (_gameManager.GetGolds() > upgrade.GetUpgradeData().DefinitiveCost)
        {
            _gameManager.OnBuyDefinitiveUpgrade(upgrade);
            foreach (DefinitiveUpgradeScript defUpgrade in _definitiveUpgrades)
            {
                defUpgrade.gameObject.SetActive(false);
            }
        }*/
    }
}

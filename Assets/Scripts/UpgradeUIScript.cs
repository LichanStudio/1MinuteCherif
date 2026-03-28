using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeUIScript : MonoBehaviour, IPointerClickHandler
{
    [Header("Managers")]
    [SerializeField] private PlayerDataManager _playerDataManager;

    [Header("Upgrade UI Elements")]
    [SerializeField] private GameObject _upgradeLinePrefab;
    [SerializeField] private GameObject _playerUpgradesParent;
    [SerializeField] private GameObject _enemyUpgradesParent;

    private CalculatedUpgradeClass _playerUpgrades;
    private CalculatedUpgradeClass _enemyUpgrades;
    private bool _canClick = false;
    private List<Upgrade> _upgrades;

    public void OnEnable()
    {
        _canClick = false;
        StartCoroutine(DelayClickEvent());
    }

    public void SetUpgrades(List<Upgrade> upgrades, float playerEff, float enemyEff)
    {
        _upgrades = upgrades;
        UpdateUI(playerEff, enemyEff);
    }

    public void UpdateUI(float playerEff, float enemyEff)
    {
        _playerUpgrades = new();
        _enemyUpgrades = new();
        _playerUpgrades.CalculateUpgrade(_upgrades.Where(u => u.GetUpgradeData().IsEnemyUpgrade == false).ToList(), playerEff);
        _enemyUpgrades.CalculateUpgrade(_upgrades.Where(u => u.GetUpgradeData().IsEnemyUpgrade == true).ToList(), enemyEff);
        AddLines(_playerUpgrades, _playerUpgradesParent);
        AddLines(_enemyUpgrades, _enemyUpgradesParent);
    }

    private void AddLines(CalculatedUpgradeClass calculUp, GameObject parentContainer)
    {
        if (calculUp != null && parentContainer != null)
        {
            for (int i = 0; i < parentContainer.transform.childCount; i++)
            {
                Destroy(parentContainer.transform.GetChild(i).gameObject);
            }
            if (calculUp.DamageAdd != 0) AddLine(parentContainer, "Damage", calculUp.DamageAdd.ToString());
            if (calculUp.BulletsAdd != 0) AddLine(parentContainer, "Bullets", calculUp.BulletsAdd.ToString());
            if (calculUp.BouncesAdd != 0) AddLine(parentContainer, "Bounce", calculUp.BouncesAdd.ToString());
            if (calculUp.MultiCount != 0) AddLine(parentContainer, "Split bullets", calculUp.MultiCount.ToString());
            if (calculUp.MultiChance != 0) AddLine(parentContainer, "Chance of split", calculUp.MultiChance.ToString() + "%");
            if (calculUp.HPAdd != 0) AddLine(parentContainer, "Increase HP", calculUp.HPAdd.ToString());
            if (calculUp.HPRecovery != 0) AddLine(parentContainer, "HP Recovery", calculUp.HPAdd.ToString() + "%");
            if (calculUp.LifeSteal != 0) AddLine(parentContainer, "Life steal", calculUp.LifeSteal.ToString() + "%");
            if (calculUp.MoveSpeed != 0) AddLine(parentContainer, "Move speed", calculUp.MoveSpeed.ToString() + "%");
        }
    }

    private void AddLine(GameObject parent, string label, string value)
    {
        GameObject line = Instantiate(_upgradeLinePrefab, parent.transform);
        if(line.TryGetComponent(out UpgradeLineUIScript lineScript))
        {
            lineScript.SetUpgradeLine(label, value);
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_canClick && eventData.button == PointerEventData.InputButton.Left)
        {
            _playerDataManager.OnSelectUpgrade(_playerUpgrades);
            ActionsManager.OnSelectUpgrade?.Invoke(_playerUpgrades, _enemyUpgrades);
        }
    }

    private IEnumerator DelayClickEvent()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        _canClick = true;
    }
}

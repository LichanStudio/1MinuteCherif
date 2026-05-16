using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeUIScript : MonoBehaviour, IPointerClickHandler
{
    [Header("Upgrade UI Elements")]
    [SerializeField] private GameObject _upgradeLinePrefab;
    [SerializeField] private GameObject _playerUpgradesParent;
    [SerializeField] private GameObject _enemyUpgradesParent;

    private Stats _characterUpgrades;
    private Stats _enemiesUpgrades;
    private Stats _characterCalculatedUpgrades;
    private Stats _enemiesCalculatedUpgrades;
    private bool _canClick = false;

    public void OnEnable()
    {
        _canClick = false;
        StartCoroutine(DelayClickEvent());
    }

    public void SetUpgrades(Stats charactersUpgrades, Stats enemiesUpgrades, float playerEff, float enemyEff)
    {
        Debug.Log("Setting up upgrades with player efficiency: " + playerEff + " and enemy efficiency: " + enemyEff);
        _characterUpgrades = charactersUpgrades;
        _enemiesUpgrades = enemiesUpgrades;
        UpdateUI(playerEff, enemyEff);
    }

    public void UpdateUI(float playerEff, float enemyEff)
    {
        _characterCalculatedUpgrades = _characterUpgrades;
        _enemiesCalculatedUpgrades = _enemiesUpgrades;
        AddLines(_characterCalculatedUpgrades, _playerUpgradesParent);
        AddLines(_enemiesCalculatedUpgrades, _enemyUpgradesParent);
    }

    private void AddLines(Stats stats, GameObject parentContainer)
    {
        if (stats != null && parentContainer != null)
        {
            for (int i = 0; i < parentContainer.transform.childCount; i++)
            {
                Destroy(parentContainer.transform.GetChild(i).gameObject);
            }
            if (stats.Damage != 0) AddLine(parentContainer, "Damage", stats.Damage.ToString() + "%");
            if (stats.MultishotChance != 0) AddLine(parentContainer, "Multishot chance", stats.MultishotChance.ToString() + "%");
            if (stats.MultihitChance != 0) AddLine(parentContainer, "Multihit chance", stats.MultihitChance.ToString() + "%");
            if (stats.BounceChance != 0) AddLine(parentContainer, "Bounce chance", stats.BounceChance.ToString() + "%");
            if (stats.PiercingChance != 0) AddLine(parentContainer, "Piercing chance", stats.PiercingChance.ToString() + "%");
            if (stats.Speed != 0) AddLine(parentContainer, "Move speed", stats.Speed.ToString());
            if (stats.HP != 0) AddLine(parentContainer, "HP", stats.HP.ToString() + "%");
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
            ActionsManager.OnSelectUpgrade?.Invoke(_characterCalculatedUpgrades, _enemiesCalculatedUpgrades);
        }
    }

    private IEnumerator DelayClickEvent()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        _canClick = true;
    }
}

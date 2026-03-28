using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataManager", menuName = "ScriptableObjects/Managers/PlayerData", order = 1)]
public class PlayerDataManager : ScriptableObject
{
    [Serializable]
    public class PlayerStat
    {
        public int InitialStat = 0;
        public int AdditionnalStat = 0;
    }

    [SerializeField] private PlayerStat _bouncesMax;
    [SerializeField][Range(0, 100)] private PlayerStat _chanceOfMulti;
    [SerializeField] private PlayerStat _multiCount;
    [SerializeField] private PlayerStat _cherifStars;
    [SerializeField] private PlayerStat _projectilesPerClick;
    [SerializeField] private PlayerStat _lifeSteal;
    [SerializeField] private PlayerStat _HP;
    [SerializeField] private PlayerStat _HPRecovery;
    [SerializeField] private PlayerStat _moveSpeed;
    [SerializeField] private PlayerStat _damage;

    public void LoadPlayerData(Dictionary<string, Upgrade> upgrades)
    {
        _bouncesMax = new() { InitialStat = 0 };
        _chanceOfMulti = new() { InitialStat = 0 };
        _multiCount = new() { InitialStat = 1 };
        _cherifStars = new() { InitialStat = 0 };
        _projectilesPerClick = new() { InitialStat = 1 };
        _lifeSteal = new() { InitialStat = 0 };
        _HP = new() { InitialStat = 10 };
        _HPRecovery = new() { InitialStat = 0 };
        _moveSpeed = new() { InitialStat = 50 };
        _damage = new() { InitialStat = 1 };
        ResetAdditionnalValues();
        if (upgrades != null)
        {
            foreach(Upgrade upgrade in upgrades.Values)
            {
                switch (upgrade.GetUpgradeData().UpgradeType)
                {
                    case Upgrade.UpgradeType.BounceAdd: _bouncesMax.AdditionnalStat += upgrade.GetUpgradeData().GetDefValue(); break;
                    case Upgrade.UpgradeType.BulletsMultChance: _chanceOfMulti.AdditionnalStat += upgrade.GetUpgradeData().GetDefValue(); break;
                    case Upgrade.UpgradeType.BulletsMult: _multiCount.AdditionnalStat += upgrade.GetUpgradeData().GetDefValue(); break;
                    case Upgrade.UpgradeType.BulletsAdd: _projectilesPerClick.AdditionnalStat += upgrade.GetUpgradeData().GetDefValue(); break;
                    case Upgrade.UpgradeType.LifeSteal: _lifeSteal.AdditionnalStat += upgrade.GetUpgradeData().GetDefValue(); break;
                    case Upgrade.UpgradeType.HPAdd: _HP.AdditionnalStat += upgrade.GetUpgradeData().GetDefValue(); break;
                    case Upgrade.UpgradeType.HPRecovery: _HPRecovery.AdditionnalStat += upgrade.GetUpgradeData().GetDefValue(); break;
                    case Upgrade.UpgradeType.MoveSpeed: _moveSpeed.AdditionnalStat += upgrade.GetUpgradeData().GetDefValue(); break;
                    case Upgrade.UpgradeType.DamageAdd: _damage.AdditionnalStat += upgrade.GetUpgradeData().GetDefValue(); break;
                }
            }
        }
    }

    public int GetBouncesMax() => _bouncesMax.InitialStat + _bouncesMax.AdditionnalStat;
    public int GetChanceOfMulti() => _chanceOfMulti.InitialStat + _chanceOfMulti.AdditionnalStat;
    public int GetMultiCount() => _multiCount.InitialStat + _multiCount.AdditionnalStat;
    public int GetCherifStars() => _cherifStars.InitialStat + _cherifStars.AdditionnalStat;
    public int GetProjectilesPerClick() => _projectilesPerClick.InitialStat + _projectilesPerClick.AdditionnalStat;
    public int GetLifeSteal() => _lifeSteal.InitialStat + _lifeSteal.AdditionnalStat;
    public int GetHPMax() => _HP.InitialStat + _HP.AdditionnalStat;
    public int GetHPRecovery() => _HPRecovery.InitialStat + _HPRecovery.AdditionnalStat;
    public int GetDamage() => _damage.InitialStat + _damage.AdditionnalStat;
    public float GetMoveSpeed() => (float)(_moveSpeed.InitialStat + _moveSpeed.AdditionnalStat) / 10;

    public void ResetBaseValues()
    {
        _bouncesMax.InitialStat = 0;
        _chanceOfMulti.InitialStat = 0;
        _multiCount.InitialStat = 0;
        _cherifStars.InitialStat = 0;
        _projectilesPerClick.InitialStat = 0;
        _lifeSteal.InitialStat = 0;
        _HP.InitialStat = 0;
        _HPRecovery.InitialStat = 0;
        _damage.InitialStat = 0;
    }

    public void ResetAdditionnalValues()
    {
        _bouncesMax.AdditionnalStat = 0;
        _chanceOfMulti.AdditionnalStat = 0;
        _multiCount.AdditionnalStat = 0;
        _cherifStars.AdditionnalStat = 0;
        _projectilesPerClick.AdditionnalStat = 0;
        _lifeSteal.AdditionnalStat = 0;
        _HP.AdditionnalStat = 0;
        _HPRecovery.AdditionnalStat = 0;
        _damage.AdditionnalStat = 0;
    }

    public void OnSelectUpgrade(CalculatedUpgradeClass calculatedUpgrade)
    {
        _bouncesMax.AdditionnalStat += calculatedUpgrade.BouncesAdd;
        _chanceOfMulti.AdditionnalStat += calculatedUpgrade.MultiChance;
        _multiCount.AdditionnalStat += calculatedUpgrade.MultiCount;
        _projectilesPerClick.AdditionnalStat += calculatedUpgrade.BulletsAdd;
        _lifeSteal.AdditionnalStat += calculatedUpgrade.LifeSteal;
        _HP.AdditionnalStat += calculatedUpgrade.HPAdd;
        _HPRecovery.AdditionnalStat += calculatedUpgrade.HPRecovery;
        _moveSpeed.AdditionnalStat += calculatedUpgrade.MoveSpeed;
        _damage.AdditionnalStat += calculatedUpgrade.DamageAdd;
    }
}

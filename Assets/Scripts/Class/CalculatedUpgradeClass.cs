using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CalculatedUpgradeClass
{
    private int _bulletsAdd = 0;
    private int _multiChance = 0;
    private int _multiCount = 0;
    private int _bouncesAdd = 0;
    private int _moveSpeed = 0;
    private int _damageAdd = 0;
    private int _HPAdd = 0;
    private int _HPRecovery = 0;
    private int _LifeSteal = 0;

    public void CalculateUpgrade(List<Upgrade> upgrades, float effectiveness)
    {
        for (int i = 0; i < upgrades.Count; i++)
        {
            CalculateAttributes(upgrades[i], effectiveness);
        }
    }

    private void CalculateAttributes(Upgrade upgrade, float effectiveness)
    {
        int value = Mathf.RoundToInt(upgrade.GetUpgradeData().SessionMin * effectiveness);
        switch (upgrade.GetUpgradeData().UpgradeType)
        {
            case Upgrade.UpgradeType.BulletsAdd: _bulletsAdd += value; break;
            case Upgrade.UpgradeType.BulletsMult: _multiCount += value; break;
            case Upgrade.UpgradeType.BulletsMultChance: _multiChance += value; break;
            case Upgrade.UpgradeType.MoveSpeed: _moveSpeed += value; break;
            case Upgrade.UpgradeType.BounceAdd: _bouncesAdd += value; break;
            case Upgrade.UpgradeType.DamageAdd: _damageAdd += value; break;
            case Upgrade.UpgradeType.HPAdd: _HPAdd += value; break;
            case Upgrade.UpgradeType.HPRecovery: _HPRecovery += value; break;
            case Upgrade.UpgradeType.LifeSteal: _LifeSteal += value; break;
        }
    }

    public int BulletsAdd => _bulletsAdd;
    public int MultiChance => _multiChance;
    public int MultiCount => _multiCount;
    public int BouncesAdd => _bouncesAdd;
    public int MoveSpeed => _moveSpeed;
    public int DamageAdd => _damageAdd;
    public int HPAdd => _HPAdd;
    public int HPRecovery => _HPRecovery;
    public int LifeSteal => _LifeSteal;
}

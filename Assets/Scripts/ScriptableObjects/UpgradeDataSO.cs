using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "ScriptableObjects/Data/Upgrade", order = 1)]
public class UpgradeData : ScriptableObject
{
    public enum UpgradeType
    {
        Default,
        Bounce,
        MultiShot,
        MultiHit,
        Piercing,
    }

    [SerializeField] private string _id;
    [SerializeField] private bool _isEnemyUpgrade;
    [SerializeField] private Stats _minStats;
    [SerializeField] private Stats _maxStats;
    [SerializeField] private Stats _minDefinitiveStats;
    [SerializeField] private Stats _maxDefinitiveStats;
    [SerializeField] private UpgradeType _upgradeType;

    public UpgradeType GetUpgradeType()
    {
        return _upgradeType;
    }

    public Stats GetRandomStat()
    {
        return new()
        {
            MultishotChance = UnityEngine.Random.Range(_minStats.MultishotChance, _maxStats.MultishotChance + 1),
            MultihitChance = UnityEngine.Random.Range(_minStats.MultihitChance, _maxStats.MultihitChance + 1),
            BounceChance = UnityEngine.Random.Range(_minStats.BounceChance, _maxStats.BounceChance + 1),
            PiercingChance = UnityEngine.Random.Range(_minStats.PiercingChance, _maxStats.PiercingChance + 1),
            Speed = UnityEngine.Random.Range(_minStats.Speed, _maxStats.Speed + 1),
            HP = UnityEngine.Random.Range(_minStats.HP, _maxStats.HP + 1),
            Damage = UnityEngine.Random.Range(_minStats.Damage, _maxStats.Damage + 1),
        };
    }
}

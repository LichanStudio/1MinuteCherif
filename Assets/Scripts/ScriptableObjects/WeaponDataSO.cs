using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Data/Weapon", order = 2)]
public class WeaponData : ScriptableObject
{
    public enum ProjectileType
    {
        None,
        Bullet,
        Throwable
    }

    public enum AttackType
    {
        None,
        Slash
    }

    [Serializable]
    private class WeaponStat
    {
        public bool Enabled = false;
        public int BaseValue = 0;
    }

    [Serializable]
    private class RangeStats
    {
        public int Range = 0;
        public WeaponStat Piercing = new()
        {
            Enabled = false,
            BaseValue = 0
        };
        public WeaponStat Bouncing = new()
        {
            Enabled = false,
            BaseValue = 0
        };
        public WeaponStat MultiShot = new()
        {
            Enabled = false,
            BaseValue = 0
        };
        public WeaponStat MultiHit = new()
        {
            Enabled = false,
            BaseValue = 0
        };
        public WeaponStat ProjectileSpeed = new()
        {
            Enabled = false,
            BaseValue = 50
        };
    }

    [Serializable]
    public class Weapon
    {
        public int AdditionalDamage = 1;
        public RuntimeAnimatorController ProjectileAnimator;
        public RuntimeAnimatorController HitAnimator;
        public Sprite Sprite;
    }

    [Header("Informations")]
    [SerializeField] private string _ID;
    [SerializeField] private string _name;
    [SerializeField] private AttackType _attackType;
    [SerializeField] private ProjectileType _projectileType;
    [SerializeField] private float _baseAttackSpeed = 0.7f;
    [SerializeField] private int _projectileSpeed = 50;
    [SerializeField] private RangeStats _rangeStats = new();
    [SerializeField] private Weapon[] _weapons;
    [SerializeField] private GameObject _weaponPrefab;

    public string ID => _ID;
    public string Name => _name;

    public int ProjectileSpeed => _projectileSpeed;
    public float BaseAttackSpeed => _baseAttackSpeed;

    public Weapon GetWeapon(int tier = 0)
    {
        return _weapons[Mathf.Clamp(tier, 0, _weapons.Length - 1)];
    }

    public GameObject GetWeaponObject(Vector2 spawnPos)
    {
        if (_weaponPrefab == null) return null;
        return Instantiate(_weaponPrefab, spawnPos, Quaternion.identity);
    }

    public int GetMaxPiercing()
    {
        if (!_rangeStats.Piercing.Enabled) return 0;
        return _rangeStats.Piercing.BaseValue;
    }

    public int GetMaxBounces()
    {
        if (!_rangeStats.Bouncing.Enabled) return 0;
        return _rangeStats.Bouncing.BaseValue;
    }

    public int GetMaxMultiShot()
    {
        if (!_rangeStats.MultiShot.Enabled) return 0;
        return _rangeStats.MultiShot.BaseValue;
    }

    public int GetMaxMultiHit()
    {
        if (!_rangeStats.MultiHit.Enabled) return 0;
        return _rangeStats.MultiHit.BaseValue;
    }

    public bool IsPiercingEnabled()
    {
        return _rangeStats.Piercing.Enabled;
    }

    public bool IsMultiShotEnabled()
    {
        return _rangeStats.MultiShot.Enabled;
    }

    public bool IsMultiHitEnabled()
    {
        return _rangeStats.MultiHit.Enabled;
    }

    public bool IsBouncingEnabled()
    {
        return _rangeStats.Bouncing.Enabled;
    }

    public AttackType GetAttackType()
    {
        return _attackType;
    }

    public ProjectileType GetProjectileType()
    {
        return _projectileType;
    }

    public float GetRange()
    {
        if(_projectileType == ProjectileType.None) return 1.5f;
        return _rangeStats.Range / 10f;
    }
}

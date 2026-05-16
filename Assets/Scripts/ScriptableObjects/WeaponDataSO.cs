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
    [SerializeField] private bool _enablePiercing;
    [SerializeField] private int _basePiercing = 0;
    [SerializeField] private bool _enableBouncing;
    [SerializeField] private int _baseBouncing = 0;
    [SerializeField] private bool _enableMultiShot;
    [SerializeField] private int _baseMultiShot = 0;
    [SerializeField] private bool _enableMultiHit;
    [SerializeField] private int _baseMultiHit = 0;
    [SerializeField] private ProjectileType _projectileType;
    [SerializeField] private float _baseAttackSpeed = 0.7f;
    [SerializeField] private int _projectileSpeed = 50;
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
        if (!_enablePiercing) return 0;
        return _basePiercing;
    }

    public int GetMaxBounces()
    {
        if (!_enableBouncing) return 0;
        return _baseBouncing;
    }

    public int GetMaxMultiShot()
    {
        if (!_enableMultiShot) return 0;
        return _baseMultiShot;
    }

    public int GetMaxMultiHit()
    {
        if (!_enableMultiHit) return 0;
        return _baseMultiHit;
    }

    public bool IsPiercingEnabled()
    {
        return _enablePiercing;
    }

    public bool IsMultiShotEnabled()
    {
        return _enableMultiShot;
    }

    public bool IsMultiHitEnabled()
    {
        return _enableMultiHit;
    }

    public bool IsBouncingEnabled()
    {
        return _enableBouncing;
    }
}

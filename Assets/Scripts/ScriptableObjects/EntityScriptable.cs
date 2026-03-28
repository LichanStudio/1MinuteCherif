using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "NewEntity", menuName = "ScriptableObjects/Data/Entity", order = 1)]
public class Entity : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private int _hp = 10;
    [SerializeField] private int _golds = 10;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private Animation _entityAnimation;
    [SerializeField] private EntityManager _entityManager;

    public Entity Clone()
    {
        Entity clone = Instantiate(this);
        return clone;
    }

    public string GetId() => _id;
    public int GetGolds() => _golds;

    public int GetHp()
    {
        if (_entityManager.GetEnemyData() != null) return _hp + _entityManager.GetEnemyData().GetHPMax();
        return _hp;
    }

    public float GetSpeed()
    {
        if (_entityManager.GetEnemyData() != null) return _speed + (_entityManager.GetEnemyData().GetMoveSpeed()/10);
        return _speed;
    }

    public int GetDamage()
    {
        if (_entityManager.GetEnemyData() != null) return _damage + _entityManager.GetEnemyData().GetDamage();
        return _damage;
    }

    public int DoDamage(int damage)
    {
        _hp -= damage;
        ActionsManager.OnDamageEnemy?.Invoke(this, damage);
        if (GetHp() <= 0)
        {
            _hp = 0;
            ActionsManager.OnEntityKilled?.Invoke(this);
        }
        return GetHp();
    }
}

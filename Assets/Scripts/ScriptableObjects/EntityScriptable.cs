using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "NewEntity", menuName = "ScriptableObjects/Data/Entity", order = 1)]
public class EntitySO : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private int _hp = 10;
    [SerializeField] private int _golds = 10;
    [SerializeField] private float _speed = 3f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private Animation _entityAnimation;

    private int _damageTaken = 0;

    public EntitySO Clone()
    {
        EntitySO clone = Instantiate(this);
        return clone;
    }

    public string GetId() => _id;
    public int GetGolds() => _golds;

    public int GetHp()
    {
        return _hp;
    }

    public float GetSpeed()
    {
        return _speed;
    }

    public int GetDamage()
    {
        return _damage;
    }

    public int TakeDamage(int damage)
    {
        _damageTaken += damage;
        //ActionsManager.OnDamageEnemy?.Invoke(this, damage);
        if (_damageTaken >= GetHp())
        {
            _damageTaken = GetHp();
            //ActionsManager.OnEntityKilled?.Invoke(this);
        }
        return GetHp() - _damageTaken;
    }
}

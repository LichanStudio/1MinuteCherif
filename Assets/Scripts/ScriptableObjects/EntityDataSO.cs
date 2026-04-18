using UnityEngine;

public abstract class EntityData : ScriptableObject
{
    [Header("Informations")]
    [SerializeField] private string _ID;
    [SerializeField] private string _name;
    [SerializeField] private RuntimeAnimatorController _animator;
    [SerializeField] private WeaponData _weaponData;

    [Header("Stats")]
    [SerializeField] protected Stats _baseStats = new ()
    {
        Speed = 50,
        HP = 5,
        Damage = 1
    };
    [SerializeField] protected Stats _additionnalStats = new();

    private int _damageTaken = 0;
    protected Stats _calculatedStats;
    protected Stats[] _statsList;

    public EntityData()
    {
        _statsList = new Stats[] { _baseStats, _additionnalStats };
    }

    public string ID => _ID;
    public string Name => _name;
    public Stats BaseStats => _baseStats;
    public Stats AdditionnalStats => _additionnalStats;
    public RuntimeAnimatorController Animator => _animator;
    public WeaponData WeaponData => _weaponData;

    public void TakeDamage(int damage)
    {
        _damageTaken += damage;
    }

    public Stats GetTotalStats(bool calculate = true)
    {
        if (calculate || _calculatedStats == null)
        {
            for (int i = 0; i < _statsList.Length; i++)
            {
                if (i == 0) _calculatedStats = _statsList[i];
                else _calculatedStats += _statsList[i];
            }
        }
        return _calculatedStats;
    }
}

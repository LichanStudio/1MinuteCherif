using UnityEngine;

public abstract class EntityData : ScriptableObject
{
    [Header("Informations")]
    [SerializeField] private string _ID;
    [SerializeField] private string _name;
    [SerializeField] private RuntimeAnimatorController _animator;
    [SerializeField] private SkillData _baseAtkSkill;
    [SerializeField] private WeaponData _weaponData;

    [Header("Stats")]
    [SerializeField] protected Stats _baseStats = new ()
    {
        Speed = 50,
        HP = 5,
        Damage = 1
    };
    [SerializeField] protected Stats _additionnalStats = new();

    private bool _statsCalculated = false;
    protected Stats _calculatedStats;
    protected Stats[] _statsList;

    public string ID => _ID;
    public string Name => _name;
    public Stats BaseStats => _baseStats;
    public Stats AdditionnalStats => _additionnalStats;
    public RuntimeAnimatorController Animator => _animator;
    public WeaponData WeaponData => _weaponData;
    public SkillData BaseAtkSkill => _baseAtkSkill;

    public void AddAditionnalStats(Stats stats)
    {
        if (stats == null) return;
        if (_additionnalStats == null) _additionnalStats = stats;
        else _additionnalStats += stats;
        _statsCalculated = false;
    }

    public void ResetAditionnalStats()
    {
        _additionnalStats = new();
        _statsCalculated = false;
    }

    protected Stats[] GetStatsList()
    {
        return new Stats[] { _baseStats, _additionnalStats };
    }

    public Stats GetTotalStats(bool calculate = true)
    {
        if (!_statsCalculated || calculate || _calculatedStats == null)
        {
            Stats[] _statsList = GetStatsList();
            for (int i = 0; i < _statsList.Length; i++)
            {
                if (i == 0) _calculatedStats = _statsList[i];
                else _calculatedStats += _statsList[i];
            }
            _statsCalculated = true;
        }
        return _calculatedStats;
    }

    public int GetMultiShot()
    {
        int baseProjectiles = (int)(GetTotalStats().MultishotChance / 100f);
        float remainingChance = GetTotalStats().MultishotChance % 100f;

        if (Random.Range(0f, 100f) < remainingChance) baseProjectiles++;

        return baseProjectiles;
    }
}

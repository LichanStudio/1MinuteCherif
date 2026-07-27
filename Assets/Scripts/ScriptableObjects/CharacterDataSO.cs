using UnityEngine;

[CreateAssetMenu(fileName = "New Character Data", menuName = "ScriptableObjects/Data/Character", order = 1)]
public class CharacterData : EntityData
{
    [Header("Informations")]
    [SerializeField] private bool _isUnlockedByDefault = false;
    [SerializeField] private SkillData _specialAtkSkill;

    [Header("Stats")]
    [SerializeField] private Stats _definitiveStats;

    public bool IsUnlockedByDefault => _isUnlockedByDefault;
    public SkillData SpecialAtk => _specialAtkSkill;

    protected new Stats[] GetStatsList()
    {
        return _statsList = new Stats[] { _baseStats, _definitiveStats, _additionnalStats };
    }
}

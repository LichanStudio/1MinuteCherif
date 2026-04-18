using UnityEngine;

[CreateAssetMenu(fileName = "New Character Data", menuName = "ScriptableObjects/Data/Character", order = 1)]
public class CharacterData : EntityData
{
    [Header("Informations")]
    [SerializeField] private bool _isUnlockedByDefault = false;

    [Header("Stats")]
    [SerializeField] private Stats _definitiveStats;

    public CharacterData()
    {
        _statsList = new Stats[] { _baseStats, _definitiveStats, _additionnalStats };
    }

    public bool IsUnlockedByDefault => _isUnlockedByDefault;
}

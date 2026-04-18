using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Data", menuName = "ScriptableObjects/Data/Monster", order = 1)]
public class MonsterData : EntityData
{
    [Header("Informations")]
    [SerializeField] private GameObject _monsterPrefab;

    public GameObject GetMonsterObject(Vector2 monsterPos)
    {
        if (_monsterPrefab == null) return null;
        return Instantiate(_monsterPrefab, monsterPos, Quaternion.identity);
    }
}

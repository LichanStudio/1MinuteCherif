using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Data", menuName = "ScriptableObjects/Data/Monster", order = 1)]
public class MonsterData : EntityData
{
    [Serializable]
    public class MonsterDropItem
    {
        [SerializeField] private DropItemsData _itemData;
        [SerializeField] private int _dropChance;
        [SerializeField] private int _minDrop = 1;
        [SerializeField] private int _maxDrop = 1;
        public DropItemsData ItemData => _itemData;
        public int DropChance => _dropChance;
        public int MinDrop => _minDrop;
        public int MaxDrop => _maxDrop;
    }

    [Header("Informations")]
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private List<MonsterDropItem> _dropItems = new();

    public GameObject GetMonsterObject(Vector2 monsterPos)
    {
        if (_monsterPrefab == null) return null;
        return Instantiate(_monsterPrefab, monsterPos, Quaternion.identity);
    }

    public List<MonsterDropItem> GetDropItems()
    {
        return _dropItems;
    }

    public Dictionary<DropItemsData, int> GenerateItems()
    {

        Dictionary<DropItemsData, int> droppedItems = new();
        foreach (var dropItem in _dropItems)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            if (randomValue <= dropItem.DropChance / 100f)
            {
                int dropCount = UnityEngine.Random.Range(dropItem.MinDrop, dropItem.MaxDrop + 1);
                droppedItems.Add(dropItem.ItemData, dropCount);
            }
        }
        return droppedItems;
    }
}

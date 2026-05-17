using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Data", menuName = "ScriptableObjects/Data/Map", order = 1)]
public class MapData : ScriptableObject
{
    [Serializable]
    public class MonsterSpawnData
    {
        public MonsterData Monster;
        public AnimationCurve SpawnChanceCurve;

        private float _spwanChance = -1.0f;
        public float GetActualSpawnChance(float evaluate = -1)
        {
            if (evaluate != -1) _spwanChance = SpawnChanceCurve.Evaluate(evaluate);
            return _spwanChance;
        }
    }

    [Header("Informations")]
    [SerializeField] private string _name;
    [SerializeField] private List<LayerRule> _layerRule;
    [SerializeField] private List<MonsterSpawnData> _monsterSpawnData;

    public List<LayerRule> LayerRules => _layerRule;
    public List<MonsterSpawnData> MonsterSpawnDataList => _monsterSpawnData;
}
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Data", menuName = "ScriptableObjects/Data/Map", order = 1)]
public class MapData : ScriptableObject
{
    [Serializable]
    public class Props
    {
        public GameObject PropPrefab;
        public float SpwanChance = 1.0f;
    }

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
    [SerializeField] private string _id;
    [SerializeField] private string _name;
    [SerializeField] private int _order = 99;
    [SerializeField] private Color _mainColor = Color.white;
    [SerializeField] private Material _material;
    [SerializeField] private List<LayerRule> _layerRule;
    [SerializeField] private List<MonsterSpawnData> _monsterSpawnData;
    [SerializeField] private List<Props> _props = new();

    public List<LayerRule> LayerRules => _layerRule;
    public List<MonsterSpawnData> MonsterSpawnDataList => _monsterSpawnData;
    public string MapName => _name;
    public string Id => _id;
    public Material MapMaterial => _material;
    public List<Props> PropsList => _props;
    public Color MainColor => _mainColor;
}
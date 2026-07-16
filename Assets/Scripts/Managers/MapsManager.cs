using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using static GeneratePixelsJob;

public class MapsManager : MonoBehaviour
{
    public static MapsManager Instance { get; private set; }

    public MapsRegistry MapsReg;

    [SerializeField] private string _actualMapId = "grassland";

    private Dictionary<string, MapData> _mapsMapper = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        _mapsMapper.Clear();
        if (MapsReg == null || MapsReg.Maps == null) return;

        for (int i = 0; i < MapsReg.Maps.Count; i++)
        {
            if (i == 0) _actualMapId = MapsReg.Maps[i].Id;
            _mapsMapper.Add(MapsReg.Maps[i].Id, MapsReg.Maps[i]);
        }
    }

    public NativeArray<JobLayerRule> GetActualMapRules()
    {
        return TileTexturesManager.Instance.GetMapRules(_mapsMapper[_actualMapId]);
    }

    public List<MapData.MonsterSpawnData> GetMonstersProbs()
    {
        return _mapsMapper[_actualMapId].MonsterSpawnDataList;
    }

    public int GetMapsCount()
    {
        return _mapsMapper.Count;
    }

    public MapData GetMapByIndex(int index)
    {
        if (index < 0 || index >= _mapsMapper.Count) return null;
        return _mapsMapper.ToList()[index].Value;
    }

    public MapData SetActualMap(MapData mapData)
    {
        if (_mapsMapper.ContainsKey(mapData.Id))
        {
            _actualMapId = mapData.Id;
            return mapData;
        }
        return null;
    }

    public MapData SetActualMap(string mapId)
    {
        if (_mapsMapper.ContainsKey(mapId))
        {
            _actualMapId = mapId;
            return _mapsMapper[mapId];
        }
        return null;
    }

    public MapData GetActualMap()
    {
        return _mapsMapper[_actualMapId];
    }
}
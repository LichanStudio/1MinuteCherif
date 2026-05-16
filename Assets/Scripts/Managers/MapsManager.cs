using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static GeneratePixelsJob;

public class MapsManager : MonoBehaviour
{
    public static MapsManager Instance { get; private set; }

    public MapsRegistry MapsReg;

    private int actualMapIndex = 0;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public NativeArray<JobLayerRule> GetActualMapRules()
    {
        return TileTexturesManager.Instance.GetMapRules(MapsReg.Maps[actualMapIndex]);
    }

    public List<MapData.MonsterSpawnData> GetMonstersProbs()
    {
        return MapsReg.Maps[actualMapIndex].MonsterSpawnDataList;
    }
}
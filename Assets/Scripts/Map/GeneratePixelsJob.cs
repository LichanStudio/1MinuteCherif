using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct GeneratePixelsJob : IJobParallelFor
{
    public struct JobLayerRule
    {
        public float threshold;
        public int tileType;
        public int dataIndex;
        public int spriteCount;
    }

    [ReadOnly] public int chunkSize;
    [ReadOnly] public int ppu;
    [ReadOnly] public float noiseScale;
    [ReadOnly] public float seedOffset;
    [ReadOnly] public float worldOffsetX;
    [ReadOnly] public float worldOffsetY;
    [ReadOnly] public NativeArray<Color32> AllTexturesData;
    [ReadOnly] public NativeArray<JobLayerRule> Rules;

    public NativeArray<int> PixelRulesMapper;
    public NativeArray<Color32> resultPixels;

    public void Execute(int index)
    {
        int px = index % chunkSize;
        int py = index / chunkSize;

        int worldX = (int)worldOffsetX + px;
        int worldY = (int)worldOffsetY + py;

        float2 noiseCoord = new(worldX * noiseScale + seedOffset, worldY * noiseScale + seedOffset);
        float noiseValue = (noise.cnoise(noiseCoord) + 1f) * 0.5f;

        int tileOriginX = (worldX / ppu) * ppu;
        int tileOriginY = (worldY / ppu) * ppu;

        JobLayerRule selectedRule = Rules[0];
        for (int i = 0; i < Rules.Length; i++)
        {
            if (noiseValue >= Rules[i].threshold)
            {
                selectedRule = Rules[i];
                break;
            }
        }

        uint seed = (uint)(tileOriginX * 3913 + tileOriginY * 7451 + (int)(noiseValue * 100));
        Unity.Mathematics.Random rng = new(seed);

        int spriteIndex = rng.NextInt(0, selectedRule.spriteCount);

        int localX = worldX % ppu;
        if (localX < 0) localX += ppu;
        int localY = worldY % ppu;
        if (localY < 0) localY += ppu;

        int pixelOffset = selectedRule.dataIndex + (spriteIndex * ppu * ppu) + (localY * ppu) + localX;

        resultPixels[index] = AllTexturesData[pixelOffset];
        //PixelRulesMapper[PixelRulesMapper.Length - 1 - index] = selectedRule.dataIndex;
        PixelRulesMapper[index] = selectedRule.tileType;
    }
}
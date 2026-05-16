using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;

[BurstCompile]
public struct GeneratePixelsJob : IJobParallelFor
{
    public struct JobLayerRule
    {
        public float threshold;
        public int dataIndex; // Pour savoir quel tableau de pixels utiliser
        public int spriteCount;
    }

    [ReadOnly] public int chunkSize;
    [ReadOnly] public int ppu; // ex: 16
    [ReadOnly] public float noiseScale;
    [ReadOnly] public float seedOffset;
    [ReadOnly] public float worldOffsetX;
    [ReadOnly] public float worldOffsetY;

    // Toutes les données de tes sprites d'herbe regroupées dans un seul tableau
    // Imagine que tu as 4 sprites de 16x16, ce tableau fait 4 * 256 pixels.
    [ReadOnly] public NativeArray<Color32> AllTexturesData;
    [ReadOnly] public NativeArray<JobLayerRule> Rules;

    public NativeArray<Color32> resultPixels;

    public void Execute(int index)
    {
        int px = index % chunkSize;
        int py = index / chunkSize;

        int worldX = (int)worldOffsetX + px;
        int worldY = (int)worldOffsetY + py;

        // 1. Calcul du biome/type avec le bruit
        float2 noiseCoord = new float2(worldX * noiseScale + seedOffset, worldY * noiseScale + seedOffset);
        float noiseValue = (noise.cnoise(noiseCoord) + 1f) * 0.5f;

        // 2. Logique de grille (PPU)
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

        // 3. Calcul de la Seed unique par tuile (Remplacement de TileSeeder.Compute)
        // Utilisation d'un hash mathématique simple compatible Burst
        uint seed = (uint)(tileOriginX * 3913 + tileOriginY * 7451 + (int)(noiseValue * 100));
        Unity.Mathematics.Random rng = new Unity.Mathematics.Random(seed);

        // Choisir un sprite aléatoire parmi la liste
        int spriteIndex = rng.NextInt(0, selectedRule.spriteCount);

        // 4. Récupérer le pixel correspondant dans le sprite
        int localX = worldX % ppu;
        if (localX < 0) localX += ppu;
        int localY = worldY % ppu;
        if (localY < 0) localY += ppu;

        // Calcul de l'index dans le gros tableau grassSpritesData
        // (offset du sprite choisi + position y dans le sprite + position x)
        int pixelOffset = selectedRule.dataIndex + (spriteIndex * ppu * ppu) + (localY * ppu) + localX;

        resultPixels[index] = AllTexturesData[pixelOffset];
    }
}
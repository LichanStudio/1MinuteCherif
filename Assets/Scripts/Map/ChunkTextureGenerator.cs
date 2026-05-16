using System.Collections.Generic;
using System.Globalization;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static GeneratePixelsJob;

public class ChunkTextureGenerator : MonoBehaviour
{
    [Header("Noise Settings")]
    public float noiseScale = 0.05f; // lower = smoother
    public int noiseSeed = 48;

    private static readonly Dictionary<TileType, Color> _fallbackColors = new()
    {
        { TileType.Grass,  new Color(0.30f, 0.65f, 0.20f) },
        { TileType.Dirt,   new Color(0.60f, 0.40f, 0.20f) },
        { TileType.Water,  new Color(0.20f, 0.45f, 0.80f) },
        { TileType.Sand,   new Color(0.85f, 0.75f, 0.45f) },
        { TileType.Stone,  new Color(0.55f, 0.55f, 0.55f) },
    };

    private TileType ResolveLayer(float noiseValue)
    {
        if (TileTexturesManager.Instance.LayerRules == null) return TileType.Grass;
        List<LayerRule> layerRules = TileTexturesManager.Instance.LayerRules;
        TileType result = layerRules[^1].tileType;

        foreach (var rule in layerRules)
        {
            if (noiseValue >= rule.threshold)
            {
                result = rule.tileType;
                break;
            }
        }
        return result;
    }

    private Color SamplePixel(TileType type, int px, int py, int seed)
    {
        Sprite sprite = TileTexturesManager.Instance != null ? TileTexturesManager.Instance.GetVariant(type, seed) : null;

        if (sprite == null) return GetFallback(type);

        Texture2D src = sprite.texture;
        Rect rect = sprite.rect;

        int rectX = (int)rect.x;
        int rectY = (int)rect.y;
        int rectW = (int)rect.width;
        int rectH = (int)rect.height;

        int srcX = rectX + px % rectW;
        int srcY = rectY + py % rectH;

        return src.GetPixel(srcX, srcY);
    }

    public Sprite TextureToSprite(Texture2D texture)
    {
        Rect rect = new(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rect, Vector2.zero, 16);
    }

    private Color GetFallback(TileType type) => _fallbackColors.TryGetValue(type, out var c) ? c : Color.magenta;

    //public Color[] GeneratePixels(int chunkX, int chunkY)
    //{
    //    int chunkSize = ChunkManager.Instance.chunkPixelSize;
    //    int ppu = GameManager.Instance.PIXELS_PER_UNIT;
    //    Color[] pixels = new Color[chunkSize * chunkSize];

    //    float worldOffsetX = chunkX * chunkSize;
    //    float worldOffsetY = chunkY * chunkSize;
    //    float seedOffset = noiseSeed * 1000.3f;

    //    for (int py = 0; py < chunkSize; py++)
    //    {
    //        for (int px = 0; px < chunkSize; px++)
    //        {
    //            int worldX = (int)worldOffsetX + px;
    //            int worldY = (int)worldOffsetY + py;

    //            float wx = worldX * noiseScale + seedOffset;
    //            float wy = worldY * noiseScale + seedOffset;
    //            float noiseValue = Mathf.PerlinNoise(wx, wy);
    //            TileType tileType = ResolveLayer(noiseValue);

    //            int tileOriginX = Mathf.FloorToInt((float)worldX / ppu) * ppu;
    //            int tileOriginY = Mathf.FloorToInt((float)worldY / ppu) * ppu;

    //            int seed = TileSeeder.Compute(tileOriginX, tileOriginY, 0, 0, (int)tileType);

    //            int localX = worldX - tileOriginX;
    //            int localY = worldY - tileOriginY;

    //            pixels[py * chunkSize + px] = SamplePixel(tileType, localX, localY, seed);
    //        }
    //    }

    //    return pixels;
    //}

    public Color32[] GeneratePixels(int chunkX, int chunkY)
    {
        int chunkSize = ChunkManager.Instance.chunkPixelSize;
        int totalPixels = chunkSize * chunkSize;

        // 1. Allouer de la mémoire native (temporaire)
        NativeArray<Color32> nativePixels = new(totalPixels, Allocator.TempJob);

        // 2. Configurer le Job avec les données actuelles (Sortir les Singletons ici !)
        GeneratePixelsJob job = new()
        {
            chunkSize = chunkSize,
            ppu = GameManager.Instance.PIXELS_PER_UNIT,
            noiseScale = this.noiseScale,
            seedOffset = noiseSeed * 1000.3f,
            worldOffsetX = chunkX * chunkSize,
            worldOffsetY = chunkY * chunkSize,
            resultPixels = nativePixels,
            AllTexturesData = TileTexturesManager.Instance.AllSpritesAtlas,
            Rules = MapsManager.Instance.GetActualMapRules()
        };

        // 3. Lancer le Job sur tous les cœurs (ParallelFor)
        JobHandle handle = job.Schedule(totalPixels, 64);

        // 4. Attendre la fin (ou mieux : faire autre chose en attendant)
        handle.Complete();

        // 5. Récupérer les données et libérer la mémoire native
        Color32[] managedPixels = new Color32[totalPixels];
        nativePixels.CopyTo(managedPixels);
        nativePixels.Dispose();

        return managedPixels;
    }
}
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

    public Sprite TextureToSprite(Texture2D texture)
    {
        Rect rect = new(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rect, Vector2.zero, 16);
    }

    public (Color32[] pixels, NativeArray<int> tileTypeMapper) GeneratePixels(int chunkX, int chunkY)
    {
        int chunkSize = ChunkManager.Instance.chunkPixelSize;
        int ppu = GameManager.Instance.PIXELS_PER_UNIT;
        int totalPixels = chunkSize * chunkSize;
        NativeArray<Color32> nativePixels = new(totalPixels, Allocator.TempJob);
        NativeArray<int> pixelRulesMapper = new(totalPixels, Allocator.TempJob);

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
            Rules = MapsManager.Instance.GetActualMapRules(),
            PixelRulesMapper = pixelRulesMapper
        };

        JobHandle handle = job.Schedule(totalPixels, 64);
        handle.Complete();

        Color32[] managedPixels = new Color32[totalPixels];
        nativePixels.CopyTo(managedPixels);
        nativePixels.Dispose();

        return (managedPixels, pixelRulesMapper);
    }
}
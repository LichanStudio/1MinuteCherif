using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[DefaultExecutionOrder(150)]
public class ChunkTextureGenerator : MonoBehaviour
{
    [Header("Noise Settings")]
    public float noiseScale = 0.05f;
    public int noiseSeed = 48;

    // Cache STATIQUE géré proprement
    private static NativeArray<Color32> _cachedAtlas;
    private static NativeArray<GeneratePixelsJob.JobLayerRule> _cachedRules;
    private static int _instanceCount = 0; // Compteur pour savoir quand vider la mémoire statique

    private void Awake()
    {
        _instanceCount++;
    }

    public void InitializeStaticData()
    {
        if (_cachedAtlas.IsCreated) return;

        var atlasData = TileTexturesManager.Instance.AllSpritesAtlas;
        _cachedAtlas = new NativeArray<Color32>(atlasData, Allocator.Persistent);

        var rules = MapsManager.Instance.GetActualMapRules();
        _cachedRules = new NativeArray<GeneratePixelsJob.JobLayerRule>(rules, Allocator.Persistent);
    }

    // Changement : On passe par un conteneur ou on renvoie les NativeArrays créés !
    public JobHandle GeneratePixelsAsync(int chunkX, int chunkY,
        out NativeArray<Color32> nativePixels, out NativeArray<int> pixelRulesMapper)
    {
        int chunkSize = ChunkManager.Instance.chunkPixelSize;
        int ppu = GameManager.Instance.PIXELS_PER_UNIT;
        int totalPixels = chunkSize * chunkSize;

        // Utilisez Allocator.Persistent si le traitement peut durer dans le temps (ou TempJob si consommé sous 4 frames max)
        nativePixels = new NativeArray<Color32>(totalPixels, Allocator.Persistent);
        pixelRulesMapper = new NativeArray<int>(totalPixels, Allocator.Persistent);

        GeneratePixelsJob job = new()
        {
            chunkSize = chunkSize,
            ppu = ppu,
            noiseScale = this.noiseScale,
            seedOffset = noiseSeed * 1000.3f,
            worldOffsetX = chunkX * chunkSize,
            worldOffsetY = chunkY * chunkSize,
            resultPixels = nativePixels,
            AllTexturesData = _cachedAtlas,
            Rules = _cachedRules,
            PixelRulesMapper = pixelRulesMapper
        };

        // On retourne le handle unique de CE job, sans bloquer le thread
        return job.Schedule(totalPixels, 64);
    }

    // Le nettoyage global ne se fait QUE lorsque TOUS les générateurs sont détruits (ex: changement de niveau)
    private void OnDestroy()
    {
        _instanceCount--;
        if (_instanceCount <= 0)
        {
            if (_cachedAtlas.IsCreated) _cachedAtlas.Dispose();
            if (_cachedRules.IsCreated) _cachedRules.Dispose();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static GeneratePixelsJob;

[DefaultExecutionOrder(100)]
public class TileTexturesManager : MonoBehaviour
{
    public static TileTexturesManager Instance { get; private set; }

    [Header("Registries")]
    public TileTexturesRegistry _tileTextureRegistry;

    [Header("Settings")]
    public List<LayerRule> LayerRules = new(); // Sorted by priority (0 = lower)

    private Dictionary<TileType, Sprite[]> _tileSpritesMapper = new();
    private Dictionary<TileType, NativeArray<Color32>> _tileTexturesMapper = new();

    private NativeArray<Color32> _allSpritesAtlas;
    private NativeArray<JobLayerRule> _jobRules;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Start()
    {
        List<LayerRule> layerRules = LayerRules;
        int ppu = GameManager.Instance.PIXELS_PER_UNIT;

        int totalSprites = 0;
        foreach (var rule in layerRules)
        {
            totalSprites += GetAllVariants(rule.tileType).Length;
        }
        Debug.Log($"[TileTexturesManager] Total sprites to pack: {totalSprites}");

        _allSpritesAtlas = new NativeArray<Color32>(totalSprites * ppu * ppu, Allocator.Persistent);
        _jobRules = new NativeArray<JobLayerRule>(layerRules.Count, Allocator.Persistent);

        int currentPixelOffset = 0;
        for (int i = 0; i < layerRules.Count; i++)
        {
            Sprite[] sprites = GetAllVariants(layerRules[i].tileType);

            _jobRules[i] = new JobLayerRule
            {
                threshold = layerRules[i].threshold,
                dataIndex = currentPixelOffset,
                spriteCount = sprites.Length
            };

            foreach (var sprite in sprites)
            {
                Color32[] pixels = ExtractPixelsFromSprite(sprite, ppu);
                NativeArray<Color32>.Copy(pixels, 0, _allSpritesAtlas, currentPixelOffset, ppu * ppu);
                currentPixelOffset += ppu * ppu;
            }
        }
    }

    public void OnEnable()
    {
        Init();
    }

    private void OnDestroy()
    {
        if (_allSpritesAtlas.IsCreated) _allSpritesAtlas.Dispose();
        if (_jobRules.IsCreated) _jobRules.Dispose();
    }

    public void Init()
    {
        _tileSpritesMapper.Clear();
        _tileTexturesMapper.Clear();
        foreach (TileTexture tileTexture in _tileTextureRegistry.Textures)
        {
            _tileSpritesMapper.Add(tileTexture.TileType, tileTexture.Variants);
            _tileTexturesMapper.Add(tileTexture.TileType, PrepareSpriteData(tileTexture.Variants.ToList(), GameManager.Instance.PIXELS_PER_UNIT));
        }
    }

    public NativeArray<JobLayerRule> GetMapRules(MapData mapData)
    {
        List<LayerRule> layerRules = mapData.LayerRules;
        NativeArray<JobLayerRule> result = new(layerRules.Count, Allocator.Persistent);
        int ppu = GameManager.Instance.PIXELS_PER_UNIT;

        int currentPixelOffset = 0;
        for (int i = 0; i < layerRules.Count; i++)
        {
            Sprite[] sprites = GetAllVariants(layerRules[i].tileType);

            result[i] = new JobLayerRule
            {
                threshold = layerRules[i].threshold,
                tileType = (int)layerRules[i].tileType,
                dataIndex = currentPixelOffset,
                spriteCount = sprites.Length
            };

            foreach (var sprite in sprites)
            {
                Color32[] pixels = ExtractPixelsFromSprite(sprite, ppu);
                NativeArray<Color32>.Copy(pixels, 0, _allSpritesAtlas, currentPixelOffset, ppu * ppu);
                currentPixelOffset += ppu * ppu;
            }
        }

        return result;
    }

    public Sprite GetVariant(TileType type, int seed)
    {
        if (!_tileSpritesMapper.TryGetValue(type, out var variants) || variants.Length == 0) return null;

        System.Random rng = new(seed);
        return variants[rng.Next(0, variants.Length)];
    }

    public Sprite[] GetAllVariants(TileType type)
    {
        if (!_tileSpritesMapper.TryGetValue(type, out var variants) || variants.Length == 0) return null;
        return variants;
    }   

    public NativeArray<Color32> GetTextureData(TileType type)
    {
        if (!_tileTexturesMapper.TryGetValue(type, out var data)) return default;
        return data;
    }

    public List<NativeArray<Color32>> GetAllTextureData()
    {
        return LayerRules.Select(rule => GetTextureData(rule.tileType)).ToList();
    }

    public NativeArray<Color32> PrepareSpriteData(List<Sprite> sprites, int ppu)
    {
        int totalPixels = sprites.Count * ppu * ppu;
        NativeArray<Color32> data = new(totalPixels, Allocator.Persistent);

        for (int i = 0; i < sprites.Count; i++)
        {
            Sprite sprite = sprites[i];
            Texture2D tex = sprite.texture;

            Color32[] allPixels = tex.GetPixels32();

            int startX = (int)sprite.rect.x;
            int startY = (int)sprite.rect.y;
            int texWidth = tex.width;

            for (int y = 0; y < ppu; y++)
            {
                for (int x = 0; x < ppu; x++)
                {
                    int texIndex = (startY + y) * texWidth + (startX + x);
                    int nativeArrayIndex = (i * ppu * ppu) + (y * ppu) + x;

                    data[nativeArrayIndex] = allPixels[texIndex];
                }
            }
        }
        return data;
    }

    public Color32[] ExtractPixelsFromSprite(Sprite sprite, int ppu)
    {
        Texture2D tex = sprite.texture;
        Color32[] allPixels = tex.GetPixels32();

        int startX = Mathf.RoundToInt(sprite.rect.x);
        int startY = Mathf.RoundToInt(sprite.rect.y);
        int texWidth = tex.width;

        Color32[] result = new Color32[ppu * ppu];

        for (int y = 0; y < ppu; y++)
        {
            for (int x = 0; x < ppu; x++)
            {
                int texIndex = (startY + y) * texWidth + (startX + x);
                int nativeArrayIndex = (y * ppu) + x;
                result[nativeArrayIndex] = allPixels[texIndex];
            }
        }

        return result;
    }

    public NativeArray<JobLayerRule> JobRules => _jobRules;
    public NativeArray<Color32> AllSpritesAtlas => _allSpritesAtlas;
}

using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(ChunkTextureGenerator))]
public class Chunk : MonoBehaviour
{
    public int chunkX = 0;
    public int chunkY = 0;

    private ChunkTextureGenerator generator;
    private SpriteRenderer spriteRenderer;

    private static Dictionary<Vector2Int, Texture2D> textureCache = new();
    private GameObject _colliderParent;
    private Dictionary<int, PolygonCollider2D> _ruleColliderMapper = new();

    void Awake()
    {
        generator = GetComponent<ChunkTextureGenerator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void GeneratePixels(int chunkSize)
    {
        var (pixels, pixelGrid) = generator.GeneratePixels(chunkX, chunkY);
        Vector2Int key = new(chunkX, chunkY);

        if (!textureCache.TryGetValue(key, out Texture2D tex))
        {
            tex = new Texture2D(chunkSize, chunkSize, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            textureCache[key] = tex;
        }

        tex.SetPixels32(pixels);
        tex.Apply();

        spriteRenderer.sprite = generator.TextureToSprite(tex);

        if (_colliderParent == null)
        {
            _colliderParent = new($"Collider_{chunkX}_{chunkY}");
            _colliderParent.transform.SetParent(transform);
            _colliderParent.transform.localPosition = Vector3.zero;
            _colliderParent.layer = gameObject.layer;
        }

        GenerateZones(pixelGrid, chunkSize);

        pixelGrid.Dispose();
    }

    private void GenerateZones(NativeArray<int> pixelGrid, int chunkSize)
    {
        PolygonCollider2D[] colliders = _colliderParent.GetComponentsInChildren<PolygonCollider2D>();
        int indexPolygons = 0;
        foreach (TileType type in System.Enum.GetValues(typeof(TileType)))
        {
            PolygonCollider2D poly = null;
            TerrainScript tScript = null;
            if (indexPolygons < colliders.Length)
            {
                poly = colliders[indexPolygons];
                indexPolygons++;
                poly.TryGetComponent(out tScript);
            }
            else
            {
                GameObject newCollider = new($"Collider_{type}");
                newCollider.transform.SetParent(_colliderParent.transform);
                newCollider.transform.localPosition = Vector3.zero;
                newCollider.layer = _colliderParent.layer;
                poly = newCollider.AddComponent<PolygonCollider2D>();
                poly.isTrigger = true;
                tScript = poly.AddComponent<TerrainScript>();
            }
            switch (type)
            {
                case TileType.Water:
                    tScript.SpeedModifier = -10;
                    break;
            }

            if (_ruleColliderMapper.ContainsKey((int)type)) _ruleColliderMapper[(int)type] = poly;
            else _ruleColliderMapper.Add((int)type, poly);

            if (poly != null)
            {
                poly.pathCount = 0;
                poly.gameObject.SetActive(false);
            }
        }

        ContourColliderBuilder.BuildZonesContours(
            pixelGrid,
            chunkSize,
            1f / GameManager.Instance.PIXELS_PER_UNIT,
            _ruleColliderMapper
        );
    }
}
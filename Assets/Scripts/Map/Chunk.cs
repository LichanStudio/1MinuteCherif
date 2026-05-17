using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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

        PolygonCollider2D poly = null;
        if (_colliderParent == null)
        {
            _colliderParent = new($"Collider_{chunkX}_{chunkY}");
            _colliderParent.transform.SetParent(transform);
            _colliderParent.transform.localPosition = Vector3.zero;
        }
        else
        {
            poly = _colliderParent.GetComponent<PolygonCollider2D>();
        }
        if (poly == null) poly = _colliderParent.AddComponent<PolygonCollider2D>();
            
        float pixelSize = 1f / GameManager.Instance.PIXELS_PER_UNIT;

        ContourColliderBuilder.BuildWaterContours(
            pixelGrid,
            chunkSize,
            (int)TileType.Water,
            pixelSize,
            poly
        );
        ContourColliderBuilder.BuildWaterContours(
            pixelGrid,
            chunkSize,
            (int)TileType.Stone,
            pixelSize,
            poly
        );
        // TODO : gérer plusieurs poly pour les différents types
        // Essayer de tout regrouper dans la même boucle ?
        pixelGrid.Dispose();
    }
}
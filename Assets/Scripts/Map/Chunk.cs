using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        generator = GetComponent<ChunkTextureGenerator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //public IEnumerator GeneratePixels()
    //{
    //    int chunkSize = ChunkManager.Instance.chunkPixelSize;
    //    Color[] pixels = generator.GeneratePixels(chunkX, chunkY);

    //    Vector2Int key = new Vector2Int(chunkX, chunkY);

    //    if (!textureCache.TryGetValue(key, out Texture2D tex))
    //    {
    //        tex = new Texture2D(chunkSize, chunkSize, TextureFormat.RGBA32, false)
    //        {
    //            filterMode = FilterMode.Point,
    //            wrapMode = TextureWrapMode.Clamp
    //        };
    //        textureCache[key] = tex;
    //    }

    //    tex.SetPixels(pixels);
    //    tex.Apply();
    //    spriteRenderer.sprite = generator.TextureToSprite(tex);
    //    yield return null;
    //}

    public IEnumerator GeneratePixels()
    {
        int chunkSize = ChunkManager.Instance.chunkPixelSize;

        // 1. On récupère maintenant un Color32[] (beaucoup plus léger)
        Color32[] pixels = generator.GeneratePixels(chunkX, chunkY);

        Vector2Int key = new Vector2Int(chunkX, chunkY);

        if (!textureCache.TryGetValue(key, out Texture2D tex))
        {
            // On garde RGBA32 car c'est le format natif pour Color32
            tex = new Texture2D(chunkSize, chunkSize, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            textureCache[key] = tex;
        }

        // 2. Utilise SetPixels32 au lieu de SetPixels
        // C'est un transfert direct de mémoire, presque instantané.
        tex.SetPixels32(pixels);

        // 3. Appliquer les changements au GPU
        tex.Apply();

        spriteRenderer.sprite = generator.TextureToSprite(tex);

        yield return null;
    }
}
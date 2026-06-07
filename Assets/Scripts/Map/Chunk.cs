using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Chunk : MonoBehaviour
{
    public int chunkX = 0;
    public int chunkY = 0;

    private SpriteRenderer spriteRenderer;
    private Texture2D _myTexture;
    private GameObject _colliderParent;
    private readonly Dictionary<int, PolygonCollider2D> _colliderRecycleMap = new();

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ApplyPixels(NativeArray<Color32> pixels, NativeArray<int> pixelGrid, int chunkSize)
    {
        if (_myTexture == null)
        {
            _myTexture = new Texture2D(chunkSize, chunkSize, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                name = $"ChunkTexture_{gameObject.name}"
            };
        }

        _myTexture.LoadRawTextureData(pixels);
        _myTexture.Apply(false);

        spriteRenderer.sprite = TextureToSprite(_myTexture);

        GenerateZones(pixelGrid, chunkSize);

        if (pixels.IsCreated) pixels.Dispose();
        if (pixelGrid.IsCreated) pixelGrid.Dispose();
    }

    private void GenerateZones(NativeArray<int> pixelGrid, int chunkSize)
    {
        if (_colliderParent == null)
        {
            _colliderParent = new($"Collider_Holder");
            _colliderParent.transform.SetParent(transform);
            _colliderParent.transform.localPosition = Vector3.zero;
            _colliderParent.layer = gameObject.layer;
        }

        foreach (TileType type in System.Enum.GetValues(typeof(TileType)))
        {
            int typeIndex = (int)type;

            if (!_colliderRecycleMap.ContainsKey(typeIndex))
            {
                GameObject colliderGO = new($"Collider_{type}");
                colliderGO.transform.SetParent(_colliderParent.transform);
                colliderGO.transform.localPosition = Vector3.zero;
                colliderGO.layer = _colliderParent.layer;

                PolygonCollider2D poly = colliderGO.AddComponent<PolygonCollider2D>();
                poly.isTrigger = true;
                poly.pathCount = 0;

                var terrainScript = poly.AddComponent<TerrainScript>();
                switch (type)
                {
                    case TileType.Stone:
                        terrainScript.SpeedModifier = -5;
                        break;
                    case TileType.Water:
                        terrainScript.SpeedModifier = -10;
                        break;
                    default:
                        terrainScript.SpeedModifier = 0;
                        break;
                }

                _colliderRecycleMap[typeIndex] = poly;
            }
            else
            {
                _colliderRecycleMap[typeIndex].pathCount = 0;
            }
        }

        float pixelSize = 1f / GameManager.Instance.PIXELS_PER_UNIT;

        ContourColliderBuilder.BuildZonesContours(pixelGrid, chunkSize, pixelSize, _colliderRecycleMap);
    }

    public Sprite TextureToSprite(Texture2D texture)
    {
        if (texture == null) return null;

        float ppu = GameManager.Instance.PIXELS_PER_UNIT;
        Rect rect = new(0, 0, texture.width, texture.height);

        return Sprite.Create(texture, rect, Vector2.zero, ppu);
    }

    private void OnDestroy()
    {
        if (_myTexture != null) Destroy(_myTexture);
    }
}
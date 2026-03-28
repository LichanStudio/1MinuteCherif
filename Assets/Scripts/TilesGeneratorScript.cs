using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesGeneratorScript : MonoBehaviour
{
    [Header("Game Managers")]
    [SerializeField] private GameManager _gameManager;

    [Header("Tilemaps")]
    public Tilemap sandTilemap;
    public Tilemap rockTilemap;

    [Header("Tiles Assets")]
    public TileBase sandTile;
    public TileBase rockRuleTile;

    [Header("Settings")]
    public Transform player;
    public int chunkSize = 16;
    public int viewDistance = 3;
    public int keepDistance = 5;
    public float scale = 10f;
    public float rockThreshold = 0.6f;

    [Header("Seed")]
    public bool useRandomSeed = true;
    public float seed;

    private HashSet<Vector2Int> _generatedChunks = new();
    private bool _generationActive = true;

    public void Awake()
    {
        sandTilemap.ClearAllTiles();
        rockTilemap.ClearAllTiles();
        GenrateRandomSeed();
    }

    public void OnEnable()
    {
        ActionsManager.OnEndSession += SetGenerationInactive;
        ActionsManager.OnStartSession += SetGenerationActive;
    }

    public void OnDisable()
    {
        ActionsManager.OnEndSession -= SetGenerationInactive;
        ActionsManager.OnStartSession -= SetGenerationActive;
    }

    void Update()
    {
        if (player == null) return;

        int currentChunkX = Mathf.FloorToInt(player.position.x / chunkSize);
        int currentChunkY = Mathf.FloorToInt(player.position.y / chunkSize);

        if (_generationActive)
        {
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                for (int y = -viewDistance; y <= viewDistance; y++)
                {
                    Vector2Int chunkCoord = new(currentChunkX + x, currentChunkY + y);

                    if (!_generatedChunks.Contains(chunkCoord))
                    {
                        GenerateChunk(chunkCoord.x, chunkCoord.y);
                        _generatedChunks.Add(chunkCoord);
                    }
                }
            }
        }

        if (Time.frameCount % 60 == 0) CleanupChunks(currentChunkX, currentChunkY);
    }

    private void SetGenerationActive()
    {
        _generationActive = true;
    }

    private void SetGenerationInactive()
    {
        _generationActive = true;
    }

    private void GenerateChunk(int chunkX, int chunkY)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                int worldX = chunkX * chunkSize + x;
                int worldY = chunkY * chunkSize + y;

                float xCoord = (worldX / scale) + seed;
                float yCoord = (worldY / scale) + seed;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                Vector3Int pos = new(worldX, worldY, 0);

                sandTilemap.SetTile(pos, sandTile);
                if (sample > rockThreshold) rockTilemap.SetTile(pos, rockRuleTile);
            }
        }
    }

    private void CleanupChunks(int centerX, int centerY)
    {
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();

        foreach (var chunk in _generatedChunks)
        {
            if (Mathf.Abs(chunk.x - centerX) > keepDistance || Mathf.Abs(chunk.y - centerY) > keepDistance)
            {
                chunksToRemove.Add(chunk);
            }
        }

        foreach (var chunk in chunksToRemove)
        {
            RemoveChunk(chunk.x, chunk.y);
            _generatedChunks.Remove(chunk);
        }
    }

    private void RemoveChunk(int chunkX, int chunkY)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector3Int pos = new(chunkX * chunkSize + x, chunkY * chunkSize + y, 0);
                sandTilemap.SetTile(pos, null);
                rockTilemap.SetTile(pos, null);
            }
        }
    }

    public void GenrateRandomSeed()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(-100000f, 100000f);
        }
    }
}

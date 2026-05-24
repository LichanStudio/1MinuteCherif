using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[DefaultExecutionOrder(300)]
public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance { get; private set; }

    [Header("References")]
    public GameObject chunkPrefab;

    [Header("Settings")]
    public int RenderDistanceX = 2;
    public int RenderDistanceY = 3;
    public int chunkPixelSize = 256;

    private Dictionary<Vector2Int, Chunk> _loadedChunks = new();
    private Queue<GameObject> _pool = new();
    private Queue<(Vector2Int coord, Color[] pixels, int version)> readyChunks = new();
    private readonly object queueLock = new();

    private Vector2Int _lastPlayerChunk;
    private float _ppu = 1.0f;
    private bool _init = false;
    private int _generationId = 0;
    private float _chunkWorldSize = 1.0f;
    private int _chunksLoadedPerFrame = 2; // Configurable

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        _ppu = GameManager.Instance.PIXELS_PER_UNIT;
        _chunkWorldSize = chunkPixelSize / _ppu;
    }

    private void Update()
    {
        Vector2Int current = WorldToChunkCoord(PlayerManager.Instance.PlayerObject.transform.position);

        if (!_init || current != _lastPlayerChunk)
        {
            UpdateChunks(current);
            _lastPlayerChunk = current;
        }
    }

    private void UpdateChunks(Vector2Int playerChunk)
    {
        _init = true;
        _generationId++;
        StartCoroutine(LoadChunksAround(playerChunk));
        StartCoroutine(UnloadDistantChunks(playerChunk));
    }

    private Vector2Int WorldToChunkCoord(Vector3 worldPos)
    {

        float x = worldPos.x / _chunkWorldSize;
        float y = worldPos.y / _chunkWorldSize;

        return new Vector2Int(
            Mathf.FloorToInt(x + 0.0001f),
            Mathf.FloorToInt(y + 0.0001f)
        );
    }

    private Vector3 ChunkToWorldPos(Vector2Int coord)
    {
        return new Vector3(coord.x * chunkPixelSize / _ppu, coord.y * chunkPixelSize / _ppu, 0f);
    }

    private IEnumerator LoadChunksAround(Vector2Int center)
    {
        int currentGen = _generationId;
        List<Vector2Int> coords = new();
        
        for (int dx = -RenderDistanceX; dx <= RenderDistanceX; dx++)
        {
            for (int dy = -RenderDistanceY; dy <= RenderDistanceY; dy++)
            {
                coords.Add(new Vector2Int(center.x + dx, center.y + dy));
            }
        }

        coords.Sort((a, b) => (a - center).sqrMagnitude.CompareTo((b - center).sqrMagnitude));

        int loaded = 0;
        foreach (var coord in coords)
        {
            if (currentGen != _generationId) yield break;
            if (!_loadedChunks.ContainsKey(coord))
            {
                SpawnChunk(coord);
                loaded++;
                if (loaded >= _chunksLoadedPerFrame) yield return null;
            }
        }
    }

    private void SpawnChunk(Vector2Int coord)
    {
        GameObject chunkObject = GetFromPool();
        chunkObject.transform.position = ChunkToWorldPos(coord);
        chunkObject.SetActive(true);

        if (chunkObject.TryGetComponent(out Chunk chunk))
        {
            chunk.chunkX = coord.x;
            chunk.chunkY = coord.y;
            _loadedChunks[coord] = chunk;
            chunk.GeneratePixels(chunkPixelSize);
        }
    }

    private IEnumerator UnloadDistantChunks(Vector2Int center)
    {
        List<Vector2Int> toRemove = new();

        int cx = center.x;
        int cy = center.y;

        foreach (Vector2Int coord in _loadedChunks.Keys)
        {
            if (Mathf.Abs(coord.x - cx) > RenderDistanceX ||
                Mathf.Abs(coord.y - cy) > RenderDistanceY)
            {
                toRemove.Add(coord);
            }
        }

        foreach (Vector2Int coord in toRemove)
        {
            if (_loadedChunks.ContainsKey(coord))
            {
                ReturnToPool(_loadedChunks[coord].gameObject);
                _loadedChunks.Remove(coord);
            }
            yield return null;
        }
    }

    private GameObject GetFromPool()
    {
        if (_pool.Count > 0)
        {
            GameObject poolObject = _pool.Dequeue();
            poolObject.SetActive(true);
            return poolObject;
        }
        return Instantiate(chunkPrefab, transform);
    }

    private void ReturnToPool(GameObject poolObject)
    {
        poolObject.SetActive(false);
        _pool.Enqueue(poolObject);
    }
}
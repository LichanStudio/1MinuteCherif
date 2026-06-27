using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
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
    public int chunksLoadedPerFrame = 2;

    private Dictionary<Vector2Int, Chunk> _loadedChunks = new();
    private Dictionary<Vector2Int, JobHandle> _pendingJobs = new();
    private Queue<GameObject> _pool = new();
    private Queue<(Vector2Int coord, JobHandle handle, NativeArray<Color32> pixels, NativeArray<int> grid)> readyChunks = new();

    private Vector2Int _lastPlayerChunk;
    private float _ppu = 1.0f;
    private bool _init = false;
    private int _generationId = 0;
    private float _chunkWorldSize = 1.0f;
    private ChunkTextureGenerator _chunkGenerator;

    private Coroutine _loadCoroutine;
    private Coroutine _unloadCoroutine;

    private void Awake()
    {
        _ppu = GameManager.Instance.PIXELS_PER_UNIT;
        _chunkWorldSize = chunkPixelSize / _ppu;
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        _chunkGenerator = GetComponent<ChunkTextureGenerator>();
        ReloadMapData();
    }

    private void Update()
    {
        Vector2Int current = WorldToChunkCoord(PlayerManager.Instance.PlayerObject.transform.position);

        if (!_init || current != _lastPlayerChunk)
        {
            UpdateChunks(current);
            _lastPlayerChunk = current;
        }

        ApplyReadyChunks();
    }

    public void ReloadMapData()
    {
        _chunkGenerator.UpdateSpriteAltas();
        _chunkGenerator.UpdateMapRules();
    }

    private void UpdateChunks(Vector2Int playerChunk)
    {
        _init = true;
        _generationId++;

        if (_loadCoroutine != null) StopCoroutine(_loadCoroutine);
        _loadCoroutine = StartCoroutine(LoadChunksAround(playerChunk));

        if (_unloadCoroutine != null) StopCoroutine(_unloadCoroutine);
        _unloadCoroutine = StartCoroutine(UnloadDistantChunks(playerChunk));
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
        List<Vector2Int> localCoords = new();

        for (int dx = -RenderDistanceX; dx <= RenderDistanceX; dx++)
        {
            for (int dy = -RenderDistanceY; dy <= RenderDistanceY; dy++)
            {
                localCoords.Add(new Vector2Int(center.x + dx, center.y + dy));
            }
        }
        localCoords.Sort((a, b) => (a - center).sqrMagnitude.CompareTo((b - center).sqrMagnitude));

        int loaded = 0;
        foreach (var coord in localCoords)
        {
            if (currentGen != _generationId) yield break;

            if (!_loadedChunks.ContainsKey(coord) && !_pendingJobs.ContainsKey(coord))
            {
                SpawnChunk(coord);
                loaded++;

                if (loaded >= chunksLoadedPerFrame)
                {
                    yield return null;
                    loaded = 0;
                }
            }
        }

        _loadCoroutine = null;
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

            var generator = chunk.GetComponent<ChunkTextureGenerator>();

            JobHandle handle = generator.GeneratePixelsAsync(coord.x, coord.y,
                out NativeArray<Color32> pixels, out NativeArray<int> grid);

            _pendingJobs[coord] = handle;
            readyChunks.Enqueue((coord, handle, pixels, grid));
        }
    }

    private void ApplyReadyChunks()
    {
        int applied = 0;
        while (readyChunks.Count > 0 && applied < chunksLoadedPerFrame)
        {
            var (coord, handle, pixels, grid) = readyChunks.Peek();

            if (!handle.IsCompleted) break;

            readyChunks.Dequeue();

            if (_loadedChunks.TryGetValue(coord, out Chunk chunk))
            {
                handle.Complete();
                chunk.ApplyPixels(pixels, grid, chunkPixelSize);
            }
            else
            {
                if (pixels.IsCreated) pixels.Dispose();
                if (grid.IsCreated) grid.Dispose();
            }

            _pendingJobs.Remove(coord);
            applied++;
        }
    }

    private IEnumerator UnloadDistantChunks(Vector2Int center)
    {
        List<Vector2Int> toRemove = new();
        int cx = center.x;
        int cy = center.y;

        var keysCopy = new List<Vector2Int>(_loadedChunks.Keys);

        foreach (Vector2Int coord in keysCopy)
        {
            if (Mathf.Abs(coord.x - cx) > (RenderDistanceX * 2) ||
                Mathf.Abs(coord.y - cy) > (RenderDistanceY * 2))
            {
                toRemove.Add(coord);
            }
        }

        foreach (Vector2Int coord in toRemove)
        {
            if (_loadedChunks.TryGetValue(coord, out var chunk))
            {
                ReturnToPool(chunk.gameObject);
                _loadedChunks.Remove(coord);
                _pendingJobs.Remove(coord);
            }
            yield return null;
        }
    
        _unloadCoroutine = null;
    }

    private GameObject GetFromPool()
    {
        if (_pool.Count > 0)
        {
            return _pool.Dequeue();
        }
        return Instantiate(chunkPrefab, transform);
    }

    private void ReturnToPool(GameObject poolObject)
    {
        poolObject.SetActive(false);
        _pool.Enqueue(poolObject);
    }
}
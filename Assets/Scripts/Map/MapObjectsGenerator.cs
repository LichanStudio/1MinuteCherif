using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapObjectsGenerator : MonoBehaviour
{
    [Header("Configuration des Objets")]
    public int numberOfObjectsPerZone = 3;
    public float zoneSize = 10f;
    public float exclusionRadius = 5f;

    [Header("Debug")]
    [SerializeField] private List<Vector2> _positionsDone = new();
    [SerializeField] private List<GameObject> _spawnedObjects = new();

    private bool _isMoving = false;
    private bool _isInGame = false;
    private MapData _mapData = null;

    private void OnEnable()
    {
        ActionsManager.OnPlayerRun += OnPlayerMoving;
        ActionsManager.OnPlayerIdle += OnPlayerIdle;
        ActionsManager.OnStartSession += OnStartSession;
        ActionsManager.OnEndSession += OnEndSession;
    }

    private void OnDisable()
    {
        ActionsManager.OnPlayerRun -= OnPlayerMoving;
        ActionsManager.OnPlayerIdle -= OnPlayerIdle;
        ActionsManager.OnStartSession -= OnStartSession;
        ActionsManager.OnEndSession -= OnEndSession;
    }

    public void Update()
    {
        if (_isMoving && _isInGame) GenerateObjectsAtCoordinate(transform.position);
    }

    public void OnPlayerMoving()
    {
        _isMoving = true;
        if (_mapData == null) UpdateMap();
    }

    public void OnPlayerIdle() {
        _isMoving = false;
    }

    public void OnStartSession()
    {
        _isInGame = true;
        UpdateMap();
    }

    private void UpdateMap()
    {
        _mapData = MapsManager.Instance.GetActualMap();
    }

    public bool CheckPositionAlreadyDone(Vector2 position)
    {
        return _positionsDone.Contains(position);
    }

    public void ClearPositionsDone()
    {
        _positionsDone.Clear();
    }

    public void OnEndSession()
    {
        _isInGame = false;
        ClearPositionsDone();
        ClearSpawnedObjects();
    }

    public void GenerateObjectsAtCoordinate(Vector2 zoneCoordinate)
    {
        Vector2 roundedCoords = zoneCoordinate.Snap(zoneSize);
        if (CheckPositionAlreadyDone(roundedCoords)) return;

        _positionsDone.Add(roundedCoords);

        StartCoroutine(GenerationCoroutine());
    }

    public void ClearSpawnedObjects()
    {
        foreach (var obj in _spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        _spawnedObjects.Clear();
    }

    public IEnumerator GenerationCoroutine()
    {
        if (_mapData == null || _mapData.PropsList == null || _mapData.PropsList.Count == 0) yield break;

        for (int i = 0; i < numberOfObjectsPerZone; i++)
        {
            int randomIndex = Random.Range(0, _mapData.PropsList.Count);
            MapData.Props prefabToSpawn = _mapData.PropsList[randomIndex];

            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(exclusionRadius, zoneSize);
            Vector3 finalPosition = (Vector2)transform.position + (randomDirection * randomDistance);

            _spawnedObjects.Add(Instantiate(prefabToSpawn.PropPrefab, finalPosition, Quaternion.identity));

            yield return null;
        }
    }
}

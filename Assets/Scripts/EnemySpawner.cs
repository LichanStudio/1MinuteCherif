using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _maxDistance = 30f;
    [SerializeField] private float _minDistance = 10f;
    [SerializeField] private float _spawnInterval = 1f;
    [SerializeField] private float _baseSpawnCount = 10f;
    [SerializeField] private AnimationCurve _spawnRateCurve;

    private float _timer;

    public void Update()
    {
        //if (_gameManager == null || _spawnRateCurve == null || _movementManager  == null || _gameManager.GetSecondsLeft() <= 0) return;
        _timer += Time.deltaTime;
        if (_timer > _spawnInterval) SpawnTic();
    }

    private void SpawnTic()
    {
        /*float timePercent = (float)_gameManager.GetTimePlayed() / _gameManager.GetSessionDuration();
        float currentSpawnRate = _spawnRateCurve.Evaluate(timePercent) * _baseSpawnCount;
        for (int i = 0; i < currentSpawnRate; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float randomDist = Random.Range(_minDistance, _maxDistance);
            Vector2 spawnOffset = randomDir * randomDist;
            Vector2 playerPosition = _movementManager.GetPlayer().transform.position;
            SpawnEnemy(playerPosition + spawnOffset, null);
        }
        _timer = 0f;*/
    }

    public void SpawnEnemy(Vector2 position, MonsterData monsterData)
    {
        GameObject enemyObj = Instantiate(_enemyPrefab, position, Quaternion.identity);
        if (enemyObj.TryGetComponent(out EnemyScript enemyScript))
        {
            enemyScript.Awake();
            enemyScript.OnEnable();
        }
    }
}

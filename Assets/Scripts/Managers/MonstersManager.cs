using System.Collections;
using System.Threading;
using UnityEngine;

public class MonstersManager : MonoBehaviour
{
    public static MonstersManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private MonstersRegistry _monsterRegistry;
    [SerializeField] private float _spawnRange = 10f;
    [SerializeField] private AnimationCurve _spawnQuantityCurve;
    [SerializeField] private AnimationCurve _spawnRateCurve;

    private bool _spawnMonsters = false;
    private Coroutine _coroutine;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnEnable()
    {
        ActionsManager.OnPlayerKilled += OnPlayerKilled;
        ActionsManager.OnStartSession += OnStartSession;
        ActionsManager.OnEndSession += OnEndSession;
    }

    public void OnDisable()
    {
        ActionsManager.OnPlayerKilled -= OnPlayerKilled;
        ActionsManager.OnStartSession -= OnStartSession;
        ActionsManager.OnEndSession -= OnEndSession;
    }

    private void OnStartSession()
    {
        _spawnMonsters = true;
        _coroutine = StartCoroutine(SpawnCoroutine());
    }

    private void OnEndSession() {
        _spawnMonsters = false;
        StopCoroutine(_coroutine);
    }

    private void OnPlayerKilled()
    {
        _spawnMonsters = false;
         RemoveAllMonster(true);
    }

    private void RemoveAllMonster(bool stopCoroutine)
    {
        if (stopCoroutine && _coroutine != null) StopCoroutine(_coroutine);
        /*foreach (var monster in FindObjectsOfType<Monster>())
        {
            Destroy(monster.gameObject);
        }*/
    }

    private void SpawnMonster()
    {
        MonsterData monsterData = _monsterRegistry.GetRandomMonster();
        if (monsterData == null) return;
        Vector2 spawnPosition = new(GetRandomSpread(), GetRandomSpread());
        if (PlayerManager.Instance != null && PlayerManager.Instance.PlayerObject != null) spawnPosition += (Vector2)PlayerManager.Instance.PlayerObject.transform.position;
        GameObject monsterObject = monsterData.GetMonsterObject(spawnPosition);
        if (monsterObject != null && monsterObject.TryGetComponent(out EnemyScript enemyScript))
        {
            enemyScript.SetMonsterData(monsterData);
        }
    }

    private float GetRandomSpread()
    {
        float value = Random.Range(_spawnRange/2f, _spawnRange);
        return Random.value > 0.5f ? value : -value;
    }

    private IEnumerator SpawnCoroutine()
    {
        while (_spawnMonsters)
        {
            float quantity = _spawnQuantityCurve.Evaluate(GameManager.Instance.GetTimePlayed() / GameManager.Instance.GetSessionDuration());
            float speed = _spawnRateCurve.Evaluate(1f - (GameManager.Instance.GetTimePlayed() / GameManager.Instance.GetSessionDuration()));
            if (quantity < 1f) quantity = 1f;
            for (int i = 0; i < quantity; i++)
            {
                SpawnMonster();
            }
            yield return new WaitForSeconds(speed);
        }
    }
}
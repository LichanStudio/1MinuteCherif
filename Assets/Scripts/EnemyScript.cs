using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _stoppingDistance = 1.5f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private StatusBarScript _statusBar;

    private Rigidbody2D _rigidBody;
    private MonsterData _entity;
    private float _timeSinceLastAttack = 0f;
    private float _speed = 5f;
    private int _damageTaken = 0;

    public void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public void OnEnable()
    {
        _timeSinceLastAttack = 99f;
        _damageTaken = 0;
        ActionsManager.OnEndSession += HandleSessionEnd;
    }

    public void OnDisable()
    {
        ActionsManager.OnEndSession -= HandleSessionEnd;
    }

    public void FixedUpdate()
    {
        if (_entity == null || MovementManager.Instance == null) return;
        _rigidBody.linearVelocity = MovementManager.Instance.MoveTowardPlayer(gameObject, _stoppingDistance) * _speed;
    }

    public void Update()
    {
        if (PlayerManager.Instance == null || PlayerManager.Instance.PlayerObject == null) return;
        float sqrDistance = (transform.position - PlayerManager.Instance.PlayerObject.transform.position).sqrMagnitude;
        float sqrRange = _attackRange * _attackRange;

        _timeSinceLastAttack += Time.deltaTime;
        if (_timeSinceLastAttack >= _attackSpeed && sqrDistance < sqrRange)
        {
            //ActionsManager.OnDamagePlayer?.Invoke(_entity.GetDamage());
            _timeSinceLastAttack = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        _damageTaken += damage;
        if (_statusBar != null) _statusBar.SetCurrentValue(_entity.GetTotalStats().HP - _damageTaken);
        if (_damageTaken >= _entity.GetTotalStats().HP)
        {
            HandleEntityKilled();
        }
    }

    private void HandleEntityKilled()
    {
        //if (_gameManager != null) _gameManager.AddKilledEnemy();
        //ActionsManager.OnEntityKilled?.Invoke(entity);
        Destroy(gameObject);
    }

    public MonsterData GetEntity()
    {
        return _entity;
    }

    public void SetMonsterData(MonsterData monsterData)
    {
        _entity = monsterData;
        _speed = monsterData.GetTotalStats().Speed / 10f;
        if (_statusBar != null)
        {
            _statusBar.SetMaxValue(_entity.GetTotalStats().HP);
            _statusBar.SetCurrentValue(_entity.GetTotalStats().HP);
        }
    }

    private void HandleSessionEnd()
    {
        if (this != null && gameObject != null) Destroy(gameObject);
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}

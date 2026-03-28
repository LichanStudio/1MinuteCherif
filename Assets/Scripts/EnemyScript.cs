using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private MovementManager _movementManager;

    [Header("Settings")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _stoppingDistance = 1.5f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private Entity _entityData;
    [SerializeField] private StatusBarScript _statusBar;

    private Rigidbody2D _rigidBody;
    private Entity _entity;
    private float _timeSinceLastAttack = 0f;

    public void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        if (_entityData != null)
        {
            _entity = _entityData.Clone();
            _statusBar.SetMaxValue(_entity.GetHp());
            _statusBar.SetCurrentValue(_entity.GetHp());
        }
    }

    public void OnEnable()
    {
        _timeSinceLastAttack = 99f;
        ActionsManager.OnDamageEnemy += HandleDamage;
        ActionsManager.OnEndSession += HandleSessionEnd;
        ActionsManager.OnEntityKilled += HandleEntityKilled;
    }

    public void OnDisable()
    {
        ActionsManager.OnDamageEnemy -= HandleDamage;
        ActionsManager.OnEndSession -= HandleSessionEnd;
        ActionsManager.OnEntityKilled -= HandleEntityKilled;
    }

    public void FixedUpdate()
    {
        if (_entityData == null) return;
        _rigidBody.linearVelocity = _movementManager.MoveTowardPlayer(gameObject, _stoppingDistance) * _entity.GetSpeed();
    }

    public void Update()
    {
        float sqrDistance = (transform.position - _movementManager.GetPlayer().transform.position).sqrMagnitude;
        float sqrRange = _attackRange * _attackRange;

        _timeSinceLastAttack += Time.deltaTime;
        if (_timeSinceLastAttack >= _attackSpeed && sqrDistance < sqrRange)
        {
            ActionsManager.OnDamagePlayer?.Invoke(_entity.GetDamage());
            _timeSinceLastAttack = 0;
        }
    }

    private void HandleDamage(Entity entity, int damage)
    {
        if (_entity != null && _entity == entity)
        {
            if (_statusBar != null) _statusBar.SetCurrentValue(_entity.GetHp());
        }
    }

    public void HandleEntityKilled(Entity entity)
    {
        if (entity == _entity)
        {
            if (_gameManager != null) _gameManager.AddKilledEnemy();
            Destroy(gameObject);
        }
    }

    public Entity GetEntity()
    {
        return _entity;
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

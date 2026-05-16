using System.Collections;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject _damageContainer;
    [SerializeField] private float _stoppingDistance = 1.5f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private StatusBarScript _statusBar;
    [SerializeField] private Animator _animator;

    private Rigidbody2D _rigidBody;
    private MonsterData _entity;
    private float _timeSinceLastAttack = 0f;
    private float _speed = 5f;
    private int _damageTaken = 0;
    private bool _dying = false;

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
        if (!_dying) _rigidBody.linearVelocity = MovementManager.Instance.MoveTowardPlayer(gameObject, _stoppingDistance) * _speed;
        else _rigidBody.linearVelocity = Vector2.zero;
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

    public void TakeDamage(int damage, GameObject damageObject = null)
    {
        _damageTaken += damage;
        if (_statusBar != null)
        {
            _statusBar.SetCurrentValue(_entity.GetTotalStats().HP - _damageTaken);
        }
        ShowDamage(damageObject, damage);
        if (_damageTaken >= _entity.GetTotalStats().HP)
        {
            HandleEntityKilled();
        }
    }

    private void RemoveDamageLabels()
    {
        if (_damageContainer == null) return;
        Transform _damageTransform = _damageContainer.transform;
        for (int i = _damageTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = _damageTransform.GetChild(i);
            child.SetParent(null, true);
        }
    }

    private void HandleEntityKilled()
    {
        _dying = true;
        //if (_gameManager != null) _gameManager.AddKilledEnemy();
        //ActionsManager.OnEntityKilled?.Invoke(entity);
        StartCoroutine(DieAnimation());
    }

    private IEnumerator DieAnimation()
    {
        if(!_dying) yield break;
        if (TryGetComponent(out CapsuleCollider2D capsuleCollider)) capsuleCollider.enabled = false;
        if (TryGetComponent(out PolygonCollider2D polyCollider)) polyCollider.enabled = false;
        yield return new WaitForSeconds(0.4f);
        RemoveDamageLabels();
        yield return new WaitForSeconds(0.1f);
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
        if (monsterData.Animator != null)
        {
            _animator.runtimeAnimatorController = monsterData.Animator;
        }
    }

    private void HandleSessionEnd()
    {
        if (this != null && gameObject != null) Destroy(gameObject);
    }

    private void ShowDamage(GameObject damageObject, int damage = 1)
    {
        if(_damageContainer == null || damageObject == null) return;
        damageObject.transform.SetParent(_damageContainer.transform);
        damageObject.transform.localPosition = Vector3.zero;
        if (damageObject.TryGetComponent(out DamageLabelScript damageLabel))
        {
            damageLabel.SetDamage(damage);
        }
        damageObject.SetActive(true);
    }

    public bool IsDying()
    {
        return _dying;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}

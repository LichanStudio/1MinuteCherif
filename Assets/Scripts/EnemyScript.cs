using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : EntityScript
{
    [Header("Settings")]
    [SerializeField] private GameObject _damageContainer;
    [SerializeField] private float _stoppingDistance = 1.5f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _attackSpeed = 4f;

    [Header("Knockback Propagation")]
    [SerializeField] private float _knockbackDuration = 0.2f;
    [SerializeField] private float _propagationRadius = 1.0f;
    [SerializeField] private float _propagationFactor = 0.6f;

    [Header("Avoidance")]
    [SerializeField] private float _avoidanceRadius = 0.6f;
    [SerializeField] private float _avoidanceForce = 3f;
    [SerializeField] private float _avoidanceInterval = 0.1f;
    [SerializeField] private LayerMask _enemyLayer;

    [Header("Game Objects")]
    [SerializeField] private StatusBarScript _statusBar;
    [SerializeField] private Animator _deleteAnimator;


    private readonly List<Collider2D> _avoidanceResults = new();
    private Vector2 _knockbackDirection;
    private Rigidbody2D _rigidBody;
    private MonsterData _entity;
    private ContactFilter2D _enemyContactFilter;
    private Vector2 _cachedAvoidanceVelocity = Vector2.zero;
    private float _avoidanceCooldown = 0f;
    private float _timeSinceLastAttack = 0f;
    private float _monsterJitter = 0.02f;
    private float _speed = 5f;
    private float _animationSpeed = 1f;
    private int _damageTaken = 0;
    private bool _dying = false;
    private bool _isKnockedBack = false;

    public void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();

        _enemyContactFilter = new();
        _enemyContactFilter.SetLayerMask(_enemyLayer);
        _enemyContactFilter.useLayerMask = true;

        float radiusVar = _avoidanceRadius * 0.2f;
        float forceVar = _avoidanceForce * 0.2f;
        _avoidanceRadius += UnityEngine.Random.Range(-radiusVar, radiusVar);
        _avoidanceForce += UnityEngine.Random.Range(-forceVar, forceVar);
    }

    public void OnEnable()
    {
        _knockbackDirection = Vector2.zero;
        _timeSinceLastAttack = 99f;
        _damageTaken = 0;
        if (_animator != null) _animator.speed = 1f;
        if (_deleteAnimator != null) _deleteAnimator.gameObject.SetActive(false);
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1f);
        }
        if (_statusBar != null) _statusBar.gameObject.SetActive(true);
        ActionsManager.OnEndSession += HandleSessionEnd;
    }

    public void OnDisable()
    {
        ActionsManager.OnEndSession -= HandleSessionEnd;
    }

    public void FixedUpdate()
    {
        if (_entity == null || MovementManager.Instance == null) return;

        if (!IsDying())
        {
            if (!_isKnockedBack)
            {
                Vector2 moveVelocity = MovementManager.Instance.MoveTowardPlayer(gameObject, _stoppingDistance) * _speed;

                _avoidanceCooldown -= Time.fixedDeltaTime;
                if (_avoidanceCooldown <= 0f)
                {
                    _cachedAvoidanceVelocity = ComputeAvoidanceVelocity();
                    _avoidanceCooldown = _avoidanceInterval + UnityEngine.Random.Range(-_monsterJitter, _monsterJitter);
                }

                _rigidBody.linearVelocity = moveVelocity + _cachedAvoidanceVelocity;
            }
        }
        else
        {
            _rigidBody.linearVelocity = Vector2.zero;

            if (_spriteRenderer != null)
            {
                float speed = 2f;
                float newAlpha = Mathf.MoveTowards(_spriteRenderer.color.a, 0f, speed * Time.deltaTime);
                _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, newAlpha);
            }
        }
    }

    public void Update()
    {
        if (PlayerManager.Instance == null || PlayerManager.Instance.PlayerObject == null) return;
        float sqrDistance = (transform.position - PlayerManager.Instance.PlayerObject.transform.position).sqrMagnitude;
        float sqrRange = _attackRange * _attackRange;

        _timeSinceLastAttack += Time.deltaTime;
        if (_timeSinceLastAttack >= _attackSpeed && sqrDistance < sqrRange)
        {
            _timeSinceLastAttack = 0;
            AnimationManager.Instance.StartAttackAnimation(_animator, _entity);
        }
    }

    private void HandleSessionEnd()
    {
        if (this != null && gameObject != null) Destroy(gameObject);
    }

    public void SetKnockbackDirection(Vector2 direction)
    {
        _knockbackDirection = direction.normalized;
    }

    public void TakeDamage(int damage, GameObject damageObject = null)
    {
        _damageTaken += damage;

        if (_statusBar != null) _statusBar.SetCurrentValue(_entity.GetTotalStats().HP - _damageTaken);
        _knockbackDirection = (transform.position - PlayerManager.Instance.PlayerObject.transform.position).normalized;
        ShowDamage(damageObject, damage);
        OnHitted();
        StartCoroutine(KnockbackRoutine(5f));
        AnimationManager.Instance.StartHittedAnimation(_animator);
        PropagateForceToNeighbors(5f);

        if (_damageTaken >= _entity.GetTotalStats().HP) HandleEntityKilled();
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
        if (_animator != null) _animator.speed = 0f;
        if (_deleteAnimator != null) _deleteAnimator.gameObject.SetActive(true);
        StartCoroutine(DieAnimation());
    }

    private IEnumerator DieAnimation()
    {
        if (!_dying) yield break;
        if (TryGetComponent(out CapsuleCollider2D capsuleCollider)) capsuleCollider.enabled = false;
        if (TryGetComponent(out PolygonCollider2D polyCollider)) polyCollider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        if (_statusBar != null) _statusBar.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.8f);
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
        _animationSpeed = 0.9f + (_speed / 30f); // base speed = 30f;
        if (_statusBar != null)
        {
            _statusBar.SetMaxValue(_entity.GetTotalStats().HP);
            _statusBar.SetCurrentValue(_entity.GetTotalStats().HP);
        }
        if (monsterData.Animator != null)
        {
            _animator.runtimeAnimatorController = monsterData.Animator;
            _animator.Play("run_front");
            _animator.speed = _animationSpeed;
        }
    }

    private void ShowDamage(GameObject damageObject, int damage = 1)
    {
        if (_damageContainer == null || damageObject == null) return;
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

    private IEnumerator KnockbackRoutine(float force)
    {
        _isKnockedBack = true;
        _rigidBody.linearVelocity = Vector2.zero;
        _rigidBody.AddForce(_knockbackDirection * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(_knockbackDuration);

        _isKnockedBack = false;
    }

    private void PropagateForceToNeighbors(float originalForce)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _propagationRadius);

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.gameObject != this.gameObject && collider.TryGetComponent(out EnemyScript neighborEnemy))
            {
                Vector2 toNeighbor = (collider.transform.position - transform.position).normalized;
                float dotProduct = Vector2.Dot(_knockbackDirection, toNeighbor);

                if (dotProduct > -0.2f)
                {
                    float propagatedForce = originalForce * _propagationFactor;
                    neighborEnemy.ReceivePropagatedKnockback(_knockbackDirection, propagatedForce);
                }
            }
        }
    }

    public void ReceivePropagatedKnockback(Vector2 direction, float force)
    {
        if (_dying || _isKnockedBack) return;
        _knockbackDirection = direction.normalized;
        StartCoroutine(KnockbackRoutine(force));
    }

    private Vector2 ComputeAvoidanceVelocity()
    {
        // -------------------------------------
        _avoidanceResults.Clear();
        // -------------------------------------
        Physics2D.OverlapCircle(transform.position, _avoidanceRadius, _enemyContactFilter, _avoidanceResults);
        // -------------------------------------
        int neighborCount = _avoidanceResults.Count;
        if (neighborCount <= 1) return Vector2.zero;
        // -------------------------------------
        Vector2 avoidanceVector = Vector2.zero;
        int validNeighbors = 0;
        // -------------------------------------
        Vector2 desiredDirection = MovementManager.Instance.MoveTowardPlayer(gameObject, _stoppingDistance).normalized;
        Vector2 perpendicularLeft = new(-desiredDirection.y, desiredDirection.x);
        Vector2 perpendicularRight = new(desiredDirection.y, -desiredDirection.x);

        // -------------------------------------
        for (int i = 0; i < neighborCount; i++)
        {
            // -------------------------------------
            Collider2D neighborCollider = _avoidanceResults[i];
            // -------------------------------------
            if (neighborCollider != null && neighborCollider != _rigidBody.GetComponent<Collider2D>())
            {
                EnemyScript neighborScript = neighborCollider.GetComponent<EnemyScript>();
                if (neighborScript != null && neighborScript.IsDying()) continue;
                // -------------------------------------
                Vector2 awayFromNeighbor = (Vector2)transform.position - (Vector2)neighborCollider.transform.position;
                // -------------------------------------
                float sqrDistance = awayFromNeighbor.sqrMagnitude;
                // -------------------------------------
                if (sqrDistance > 0f)
                {
                    float distance = Mathf.Sqrt(sqrDistance);
                    float safeDistance = distance < 0.1f ? 0.1f : distance;
                    // -------------------------------------
                    Vector2 repulsionForce = awayFromNeighbor / (distance * safeDistance);
                    // -------------------------------------
                    // 5 au carré = 25 (éviter des calculs de racine carrée pour optimiser)
                    float repulsionLengthSqr = repulsionForce.sqrMagnitude;
                    if (repulsionLengthSqr > 25f) repulsionForce = repulsionForce.normalized * 5f;
                    // -------------------------------------
                    avoidanceVector += repulsionForce;
                    validNeighbors++;
                    // -------------------------------------
                    float alignment = Vector2.Dot(desiredDirection, -awayFromNeighbor) / distance;
                    // -------------------------------------
                    if (alignment > 0.7f)
                    {
                        float leftDot = Vector2.Dot(perpendicularLeft, awayFromNeighbor);
                        Vector2 bypassDirection = (leftDot > 0f) ? perpendicularLeft : perpendicularRight;

                        Vector2 bypassFinal = bypassDirection * (2.0f / safeDistance);
                        // -------------------------------------
                        // 3 au carré = 9 (éviter des calculs de racine carrée pour optimiser)
                        if (bypassFinal.sqrMagnitude > 9f) bypassFinal = bypassFinal.normalized * 3f;

                        avoidanceVector += bypassFinal;
                    }
                }
                else
                {
                    avoidanceVector += UnityEngine.Random.insideUnitCircle.normalized * _avoidanceForce;
                    validNeighbors++;
                }
            }
        }
        // -------------------------------------
        if (validNeighbors > 0) avoidanceVector /= validNeighbors;
        // -------------------------------------
        // 2 au carré = 4 (éviter des calculs de racine carrée pour optimiser)
        if (avoidanceVector.sqrMagnitude > 4f) avoidanceVector = avoidanceVector.normalized * 2f;
        // -------------------------------------
        return avoidanceVector * _avoidanceForce;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _propagationRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, _avoidanceRadius);
        Gizmos.DrawWireSphere(transform.position, _avoidanceForce);
    }
}

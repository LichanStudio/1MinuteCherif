using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileScript : MonoBehaviour
{
    [Header("Settings")]
    public float detectionRadius = 10f;
    public float lifeTime = 5f;
    public LayerMask enemyLayer;

    private Rigidbody2D _rigidBody;
    private Dictionary<GameObject, Collider2D> _hittedTargets = new();
    private float _speed = 10.0f;
    private int _currentBounces = 0;
    private int _currentPierces = 0;
    private int _maxPierces = 0;
    private int _maxBounces = 0;
    private int _countMultihits = 1;
    private int _damage = 0;
    private EntityData _casterData;
    private bool _targetEnemies = true;

    void Awake()
    {
        _hittedTargets.Clear();
        _rigidBody = GetComponent<Rigidbody2D>();
        ResetData();
    }

    void Start()
    {
        _rigidBody.linearVelocity = transform.right * _speed;
        Destroy(gameObject, lifeTime);
    }

    public void OnDestroy()
    {
        _hittedTargets.Clear();
    }

    public void SetSpeed(int speed)
    {
        _speed = (float)speed / 10f;
    }

    public void ResetData()
    {
        _currentBounces = 0;
        _currentPierces = 0;
        _maxPierces = 0;
        _maxBounces = 0;
        _countMultihits = 1;
    }

    public void SetTargetEnemies(bool targetEnemies)
    {
        _targetEnemies = targetEnemies;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider == null || !collider.isTrigger || collider.IsDestroyed() || collider.gameObject == null) return;

        if (_targetEnemies && !collider.CompareTag("Enemy")) return;
        else if(!_targetEnemies && !collider.CompareTag("Player")) return;

        if (_hittedTargets.ContainsKey(collider.gameObject)) return;

        _hittedTargets.Add(collider.gameObject, collider);

        if (_targetEnemies && collider.gameObject.TryGetComponent(out EnemyScript enemy))
        {
            if (enemy.IsDying()) return;
            for (int i = 0; i < _countMultihits; i++)
            {
                ActionsManager.OnDamageEntity?.Invoke(enemy, _damage);
                enemy.SetKnockbackDirection((enemy.transform.position - transform.position).normalized);
            }
        }
        else if(!_targetEnemies && collider.gameObject.TryGetComponent(out PlayerScript player))
        {
            for (int i = 0; i < _countMultihits; i++)
            {
                ActionsManager.OnDamageEntity?.Invoke(player, _damage);
            }
        }

        /*if (!_isMagic && _playerDataManager.GetChanceOfMulti() > Random.Range(0, 100))
        {
            for (int i = 0; i < _playerDataManager.GetMultiCount(); i++)
            {
                if (Instantiate(gameObject, transform.position, Quaternion.identity).TryGetComponent(out ProjectileScript projectile))
                {
                    projectile.SetMagic(true);
                    projectile.SetInitialDirection(new(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
                    //projectile.SetHittedTargets(new Dictionary<GameObject, Collider2D>(_hittedTargets));
                }
            }
            Destroy(gameObject);
            return;
        }

        if (_isMagic || _currentBounces > _playerDataManager.GetBouncesMax())
        {
            Destroy(gameObject);
        }
        else
        {
            RedirectToNearestEnemy();
        }*/

        if (_currentBounces < _maxBounces)
        {
            _currentBounces++;
            RedirectToNearestEnemy();
        }
        else if (_currentPierces < _maxPierces)
        {
            _currentPierces++;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCasterData(EntityData casterData)
    {
        _casterData = casterData;
        if (_casterData != null)
        {
            _damage += _casterData.GetTotalStats().Damage;
            _maxBounces += GameManager.Instance.GetProcs(_casterData.GetTotalStats().BounceChance);
            _maxPierces += GameManager.Instance.GetProcs(_casterData.GetTotalStats().PiercingChance);
            _countMultihits += GameManager.Instance.GetProcs(_casterData.GetTotalStats().MultihitChance);
        }
        if (_casterData.WeaponData != null)
        {
            _damage += _casterData.WeaponData.GetWeapon().AdditionalDamage;
            _maxBounces += _casterData.WeaponData.GetMaxBounces();
            _maxPierces += _casterData.WeaponData.GetMaxPiercing();
            _countMultihits += _casterData.WeaponData.GetMaxMultiHit();

        }
    }

    public void SetHittedTargets(Dictionary<GameObject, Collider2D> hittedTargets)
    {
        _hittedTargets = hittedTargets;
    }

    public void SetInitialDirection(Vector2 direction)
    {
        GoTo(direction);
    }

    public void SetInitialDirection(Vector2 target, Vector2 from, float additionnalAngle = 0f)
    {
        Vector2 direction = (target - from).normalized;
        GoTo(direction, additionnalAngle);
    }

    public void GoTo(Vector2 direction, float additionnalAngle = 0f)
    {
        _rigidBody.linearVelocity = direction.normalized * _speed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += additionnalAngle;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void RedirectToNearestEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);

        float closestDistance = Mathf.Infinity;
        Collider2D enemyTarget = null;

        foreach (Collider2D enemy in hitEnemies)
        {
            if (!enemy.isTrigger || !enemy.CompareTag("Enemy") || (_hittedTargets.Count > 0 && _hittedTargets.ContainsKey(enemy.gameObject))) continue;

            float distance = Vector2.Distance(transform.position, enemy.bounds.center);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                enemyTarget = enemy;
            }
        }

        if (enemyTarget != null)
        {
            GoTo(enemyTarget.bounds.center - transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

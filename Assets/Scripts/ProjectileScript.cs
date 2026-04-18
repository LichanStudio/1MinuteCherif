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
    private int _currentPierce = 0;
    private EntityData _casterData;

    void Awake()
    {
        _hittedTargets.Clear();
        _rigidBody = GetComponent<Rigidbody2D>();
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null && !collider.IsDestroyed() && collider.gameObject != null && collider.isTrigger && collider.CompareTag("Enemy"))
        {
            if (_hittedTargets.ContainsKey(collider.gameObject)) return;

            _hittedTargets.Add(collider.gameObject, collider);

            int damage = 1;
            int maxBounces = 0;
            int maxPierces = 0;

            if (_casterData != null) damage = _casterData.GetTotalStats().Damage;
            if (_casterData.WeaponData != null)
            {
                damage += _casterData.WeaponData.GetWeapon().AdditionalDamage;
                maxBounces = _casterData.WeaponData.GetMaxBounces();
                maxPierces = _casterData.WeaponData.GetMaxPiercing();
            }

            if (collider.gameObject.TryGetComponent(out EnemyScript enemy))
            {
                enemy.TakeDamage(damage);
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
            if (_currentBounces < maxBounces)
            {
                _currentBounces++;
                RedirectToNearestEnemy();
            }
            else if (_currentPierce < maxPierces)
            {
                _currentPierce++;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetCasterData(EntityData casterData)
    {
        _casterData = casterData;
    }

    public void SetHittedTargets(Dictionary<GameObject, Collider2D> hittedTargets)
    {
        _hittedTargets = hittedTargets;
    }

    public void SetInitialDirection(Vector2 direction)
    {
        GoTo(direction);
    }

    public void SetInitialDirection(Vector2 target, Vector2 from)
    {
        Vector2 direction = (target - from).normalized;
        GoTo(direction);
    }

    public void GoTo(Vector2 direction)
    {
        _rigidBody.linearVelocity = direction.normalized * _speed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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

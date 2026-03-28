using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileScript : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private PlayerDataManager _playerDataManager;

    [Header("Settings")]
    public float speed = 12f;
    public float detectionRadius = 10f;
    public float lifeTime = 5f;
    public LayerMask enemyLayer;

    private Rigidbody2D _rigidBody;
    private int _currentBounces = 0;
    private bool _isMagic = false;
    private Dictionary<GameObject, Collider2D> _hittedTargets = new();

    void Awake()
    {
        _hittedTargets.Clear();
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        _rigidBody.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    public void OnDestroy()
    {
        _hittedTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (_playerDataManager == null) return;
        if (collider != null && !collider.IsDestroyed() && collider.gameObject != null && collider.isTrigger && collider.CompareTag("Enemy"))
        {
            if(_hittedTargets.ContainsKey(collider.gameObject)) return;

            _currentBounces++;
            _hittedTargets.Add(collider.gameObject, collider);

            if (collider.gameObject.TryGetComponent(out EnemyScript enemy))
            {
                if (enemy.GetEntity() != null)
                {
                    enemy.GetEntity().DoDamage(_playerDataManager.GetDamage());
                }
            }

            if (!_isMagic && _playerDataManager.GetChanceOfMulti() > Random.Range(0, 100))
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
            }
        }
    }
    private void SetMagic(bool value)
    {
        if (value) _isMagic = value;
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
        GoTo(target - from);
    }

    public void GoTo(Vector2 direction)
    {
        _rigidBody.linearVelocity = direction.normalized * speed;
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

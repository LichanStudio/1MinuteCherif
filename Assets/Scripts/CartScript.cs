using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CartScript : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private MovementManager _movementManager;

    [Header("Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private GameObject _sit;

    private Rigidbody2D _rigidBody;
    private Vector2 _direction;
    private bool _available = true;
    private float _timeBeforeDestroy = 0f;
    private SaloonScript _saloon;

    public void Awake()
    {
        _available = true;
        _timeBeforeDestroy = 0f;
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        if (_movementManager == null) return;
        if (_available)
        {
            Vector2 directionToPlayer = _movementManager.MoveTowardPlayer(gameObject);
            if (directionToPlayer != Vector2.zero) _direction = directionToPlayer;
        }
        else
        {
            _timeBeforeDestroy += Time.fixedDeltaTime;
        }
        _rigidBody.linearVelocity = _direction * _speed;
        if (_timeBeforeDestroy >= _lifeTime) Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null && collider.gameObject.CompareTag("Player"))
        {
            if (_available) PutInside(collider.gameObject);
        }
    }

    public void PutInside(GameObject toPutInside)
    {
        if (_sit == null) return;
        toPutInside.transform.SetParent(_sit.transform);
        toPutInside.SetActive(false);
        StartCoroutine(SwitchToSaloon());
    }

    public void PutOutside()
    {
        if (_sit == null) return;
        for (int i = _sit.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = _sit.transform.GetChild(i);
            child.gameObject.SetActive(true);
            child.SetParent(null);
        }
    }

    public IEnumerator SwitchToSaloon()
    {
        if (_gameManager == null) yield break;
        _gameManager.SetGlobalLight(0f);
        _available = false;
        yield return new WaitForSeconds(1.5f);
        PutOutside();
        if (_saloon != null)
        {
            SpawnInSaloon();
            yield return new WaitForSeconds(0.5f);
            _gameManager.SetGlobalLight(1f);
        }
        else
        {
            _movementManager.TeleportPlayer(new(Random.Range(-1000f,1000f), Random.Range(-1000f, 1000f)));
            yield return new WaitForSeconds(0.5f);
            _gameManager.SetGlobalLight(1f);
            yield return new WaitForSeconds(0.5f);
            ActionsManager.OnStartSession?.Invoke();
        }
    }

    public void SpawnInSaloon()
    {
        if (_saloon != null)
        {
            _saloon.gameObject.SetActive(true);
            _saloon.TeleportPlayerIn();
        }
    }

    public void SetSaloon(SaloonScript saloonScript)
    {
        _saloon = saloonScript;
    }
}

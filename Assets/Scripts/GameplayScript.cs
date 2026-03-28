using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayScript : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private PlayerDataManager _playerDataManager;

    [Header("Settings")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _EscUI;

    private Collider2D[] _collider2Ds;
    private const float _SPAWN_DELAY = 0.1f;
    private bool _isEsc = false;
    private bool _wasPaused = false;

    public void Awake()
    {
        _collider2Ds = GetComponents<Collider2D>();
        _isEsc = false;
    }

    public void OnEnable()
    {
        if (_EscUI != null) _EscUI.SetActive(_isEsc);
    }

    public void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!_isEsc) _wasPaused = _gameManager.IsGamePaused();
            OnPressEsc();
        }
    }

    void OnClick(InputValue value)
    {
        if (!value.isPressed || _playerDataManager == null) return;

        StartCoroutine(SpawnProjectilesWithDelay());
    }

    public void OnPressEsc()
    {
        _isEsc = !_isEsc;
        if (_EscUI != null) _EscUI.SetActive(_isEsc);
        if (!_wasPaused) _gameManager.TogglePause(_isEsc);

    }

    public Vector2 GetCenter()
    {
        if (_collider2Ds.Length > 0)
        {
            foreach (var collider in _collider2Ds)
            {
                if (collider != null && collider.isTrigger)
                {
                    return collider.bounds.center;
                }
            }
        }
        return Vector2.zero;
    }


    private IEnumerator SpawnProjectilesWithDelay()
    {
        int count = _playerDataManager.GetProjectilesPerClick();

        for (int i = 0; i < count; i++)
        {
            ActionsManager.OnShoot?.Invoke();
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
            worldPos.z = 0f;

            Vector2 colliderCenter = GetCenter();

            if (Instantiate(_projectilePrefab, colliderCenter, Quaternion.identity).TryGetComponent<ProjectileScript>(out var projectile))
            {
                projectile.SetInitialDirection(worldPos, colliderCenter);
            }

            yield return new WaitForSeconds(_SPAWN_DELAY / _playerDataManager.GetProjectilesPerClick());
        }
    }
}

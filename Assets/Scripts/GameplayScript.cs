using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayScript : MonoBehaviour
{
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
            if (!_isEsc) _wasPaused = GameManager.Instance.IsGamePaused();
            OnPressEsc();
        }
    }

    void OnClick(InputValue value)
    {
        if (!value.isPressed) return;

        StartCoroutine(SpawnProjectilesWithDelay());
    }

    public void OnPressEsc()
    {
        _isEsc = !_isEsc;
        if (_EscUI != null) _EscUI.SetActive(_isEsc);
        if (!_wasPaused) GameManager.Instance.TogglePause(_isEsc);

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
        int count = 1;

        if (CharacterManager.Instance != null && CharacterManager.Instance.SelectedCharacter != null && CharacterManager.Instance.SelectedCharacter.WeaponData != null)
        {
            for (int i = 0; i < count; i++)
            {
                ActionsManager.OnShoot?.Invoke();
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
                worldPos.z = 0f;

                Vector2 colliderCenter = GetCenter();
                WeaponData weaponData = CharacterManager.Instance.SelectedCharacter.WeaponData;
                GameObject newProjectile = weaponData.GetWeaponObject(colliderCenter);

                if (weaponData != null && newProjectile != null && newProjectile.TryGetComponent<ProjectileScript>(out var projectile))
                {
                    projectile.SetInitialDirection(worldPos, colliderCenter);
                    projectile.SetSpeed(weaponData.ProjectileSpeed);
                    projectile.SetCasterData(CharacterManager.Instance.SelectedCharacter);
                }

                yield return new WaitForSeconds(_SPAWN_DELAY / count);
            }
        }
        yield return new WaitForSeconds(0.1f);
    }
}

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
    private bool _isSessionOn = false;
    private const float _DEFAULT_ATTACK_SPEED = 0.7f;

    private Coroutine _attackCoroutine;

    public void Awake()
    {
        _collider2Ds = GetComponents<Collider2D>();
        _isEsc = false;
    }

    public void OnEnable()
    {
        if (_EscUI != null) _EscUI.SetActive(_isEsc);
        ActionsManager.OnStartSession += OnSessionStart;
        ActionsManager.OnEndSession += OnSessionEnd;
    }

    public void OnDisable()
    {
        ActionsManager.OnStartSession -= OnSessionStart;
        ActionsManager.OnEndSession -= OnSessionEnd;
    }

    public void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!_isEsc) _wasPaused = GameManager.Instance.IsGamePaused();
            OnPressEsc();
        }
    }

    private void OnSessionStart()
    {
        _isSessionOn = true;
        _attackCoroutine = StartCoroutine(SpawnProjectiles());
    }

    private void OnSessionEnd()
    {
        _isSessionOn = false;
        if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);
    }

    public IEnumerator SpawnProjectiles()
    {
        while (_isSessionOn)
        {
            int projectilesToSpawn = 1 + GameManager.Instance.GetProcs(CharacterManager.Instance.SelectedCharacter.GetTotalStats().MultishotChance);
            float angle = projectilesToSpawn * 4f;
            float minAngle = -angle;
            float maxAngle = angle;
            float range = Mathf.Abs(minAngle) + Mathf.Abs(maxAngle);
            float procAngle = range / projectilesToSpawn;
            for (int i = 0; i < projectilesToSpawn; i++)
            {
                SpawnProjectile(minAngle + (procAngle * i));
            }
            if (CharacterManager.Instance != null && CharacterManager.Instance.SelectedCharacter != null && CharacterManager.Instance.SelectedCharacter.WeaponData != null)
            {
                yield return new WaitForSeconds(CharacterManager.Instance.SelectedCharacter.WeaponData.BaseAttackSpeed);
            }
            else
            {
                yield return new WaitForSeconds(_DEFAULT_ATTACK_SPEED);
            }
        }
        yield return null;
    }

    void OnClick(InputValue value)
    {
        if (!value.isPressed) return;
        if (!_isSessionOn) ActionsManager.OnStartSession?.Invoke();
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


    private void SpawnProjectile(float angle = 0f)
    {
        if (CharacterManager.Instance != null && CharacterManager.Instance.SelectedCharacter != null && CharacterManager.Instance.SelectedCharacter.WeaponData != null)
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
                projectile.SetInitialDirection(worldPos, colliderCenter, angle);
                projectile.SetSpeed(weaponData.ProjectileSpeed);
                projectile.SetCasterData(CharacterManager.Instance.SelectedCharacter);
            }
        }
    }
}

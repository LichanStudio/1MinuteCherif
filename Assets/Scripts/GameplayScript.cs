using System.Collections;
using TMPro;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject _projectileSpawnPoint;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _EscUI;

    private const float _SPAWN_DELAY = 0.1f;
    private bool _isEsc = false;
    private bool _wasPaused = false;
    private bool _isSessionOn = false;
    private const float _DEFAULT_ATTACK_SPEED = 0.7f;

    private Coroutine _attackCoroutine;

    public void Awake()
    {
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
        if (CharacterManager.Instance == null) yield break;

        CharacterData characterData = CharacterManager.Instance.SelectedCharacter;

        while (_isSessionOn)
        {
            if (characterData != null && characterData.BaseAtkSkill != null)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
                SkillContext skillContext = new()
                {
                    TargetPosition = worldPos,
                    Count = characterData.BaseAtkSkill.Context.Count,
                    Time = characterData.BaseAtkSkill.Context.Time,
                    InitialPosition = _projectileSpawnPoint.transform.position
                };
                characterData.BaseAtkSkill.Execute(characterData, skillContext);
            }

            if (characterData != null && characterData.WeaponData != null)
            {
                yield return new WaitForSeconds(characterData.WeaponData.BaseAttackSpeed);
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
        CharacterData characterData = CharacterManager.Instance.SelectedCharacter;

        if (characterData != null && characterData.SpecialAtk != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
            characterData.SpecialAtk.Execute(characterData, new()
            {
                Count = characterData.SpecialAtk.Context.Count,
                Time = characterData.SpecialAtk.Context.Time,
                PrefabZone = characterData.SpecialAtk.Context.PrefabZone,
                InitialPosition = _projectileSpawnPoint.transform.position,
                TargetPosition = worldPos
            });
        }
        //if (!_isSessionOn) ActionsManager.OnStartSession?.Invoke();
    }

    public void OnPressEsc()
    {
        _isEsc = !_isEsc;
        if (_EscUI != null) _EscUI.SetActive(_isEsc);
        if (!_wasPaused) GameManager.Instance.TogglePause(_isEsc);
    }
}

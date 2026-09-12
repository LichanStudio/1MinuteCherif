using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : EntityScript<CharacterData>
{
    [Header("Settings")]
    [SerializeField] private GameObject _projectileSpawnPoint;

    private const float _SPAWN_DELAY = 0.1f;
    private const float _DEFAULT_ATTACK_SPEED = 0.7f;

    private bool _isSessionOn = false;
    private Coroutine _attackCoroutine;

    public override void Awake()
    {
        base.Awake();
        _isPlayer = true;
    }

    public void OnEnable()
    {
        ActionsManager.OnStartSession += OnSessionStart;
        ActionsManager.OnEndSession += OnSessionEnd;
        ActionsManager.OnClick += UseMainSkill;
    }

    public void OnDisable()
    {
        ActionsManager.OnStartSession -= OnSessionStart;
        ActionsManager.OnEndSession -= OnSessionEnd;
        ActionsManager.OnClick -= UseMainSkill;
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

    public override void TakeDamage(int damage, GameObject dmgLabel = null)
    {
        ActionsManager.OnDamagePlayer?.Invoke(this, damage);
        OnHitted();
    }

    public void SetAnimatorController(RuntimeAnimatorController animatorController)
    {
        if(_animator == null) return;
        _animator.runtimeAnimatorController = animatorController;
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
                characterData.BaseAtkSkill.Execute(this, skillContext);
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

    public void UseMainSkill()
    {
        CharacterData characterData = CharacterManager.Instance.SelectedCharacter;

        if (characterData != null && characterData.SpecialAtk != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
            characterData.SpecialAtk.Execute(this, new()
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
}

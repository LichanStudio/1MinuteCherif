using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private StatusBarScript _HPStatusBar;
    [SerializeField] private Animator _playerAnimator;

    private float recoveryRate = 1f;
    private float _timeSinceRecovery = 0f;

    private int _damageTaken = 0;

    public void OnEnable()
    {
        if(_HPStatusBar != null) _HPStatusBar.AutoMask = false;
        ActionsManager.OnDamagePlayer += HandleDamage;
        ActionsManager.OnDamageEnemy += HandleEnemyDamage;
    }

    public void OnDisable()
    {
        ActionsManager.OnDamagePlayer -= HandleDamage;
        ActionsManager.OnDamageEnemy -= HandleEnemyDamage;
    }

    public void Update()
    {
        _timeSinceRecovery += Time.deltaTime;
        /*if (_timeSinceRecovery >= recoveryRate && _playerDataManager != null && _playerDataManager.GetHPRecovery() > 0)
        {
            _damageTaken -= _playerDataManager.GetHPRecovery() * _playerDataManager.GetHPMax() / 100;
            _timeSinceRecovery = 0f;
            UpdateStatus();
        }*/
    }

    public void HandleDamage(int damage)
    {
        /*_damageTaken += damage;
        _damageTaken = Mathf.Clamp(_damageTaken, 0, _playerDataManager.GetHPMax());
        UpdateStatus();
        if (_playerDataManager.GetHPMax() - _damageTaken <= 0)
        {
            ActionsManager.OnPlayerKilled?.Invoke();
        }*/
    }

    public void HandleEnemyDamage(MonsterData enemy, int damage)
    {
        /*if (_playerDataManager.GetLifeSteal() > 0)
        {
            int lifeSteal = damage * _playerDataManager.GetLifeSteal() / 100;
            lifeSteal = Mathf.Clamp(lifeSteal, 0, _playerDataManager.GetHPMax());
            _damageTaken -= lifeSteal;
            UpdateStatus();
        }*/
    }

    public void SetAnimatorController(RuntimeAnimatorController animatorController)
    {
        if(_playerAnimator == null) return;
        _playerAnimator.runtimeAnimatorController = animatorController;
    }
}

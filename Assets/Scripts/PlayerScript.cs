using UnityEngine;

public class PlayerScript : EntityScript
{
    private float recoveryRate = 1f;
    private float _timeSinceRecovery = 0f;

    private int _damageTaken = 0;

    public override void Awake()
    {
        base.Awake();
        _isPlayer = true;
    }

    public void OnEnable()
    {
        ActionsManager.OnDamageEnemy += OnDamageEnemy;
    }

    public void OnDisable()
    {
        ActionsManager.OnDamageEnemy -= OnDamageEnemy;
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

    public override void TakeDamage(int damage, GameObject dmgLabel = null)
    {
        _damageTaken += damage;
        /*_damageTaken += damage;
        _damageTaken = Mathf.Clamp(_damageTaken, 0, _playerDataManager.GetHPMax());
        UpdateStatus();
        if (_playerDataManager.GetHPMax() - _damageTaken <= 0)
        {
            ActionsManager.OnPlayerKilled?.Invoke();
        }*/
        ActionsManager.OnDamagePlayer?.Invoke(this, damage);
        OnHitted();
    }

    public void OnDamageEnemy(EnemyScript enemyScript, int damage)
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
        if(_animator == null) return;
        _animator.runtimeAnimatorController = animatorController;
    }
}

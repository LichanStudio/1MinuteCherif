using TMPro;
using UnityEngine;

public class GoldsScript : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager _gameManager;

    [Header("Settings")]
    [SerializeField] private TextMeshProUGUI _goldAmount;

    public void OnEnable()
    {
        ActionsManager.OnEntityKilled += OnEntityKilled;
        ActionsManager.OnPlayerKilled += OnPlayerKilled;
        ActionsManager.OnSelectDefinitiveUpgrade += OnSelectDefinitiveUpgrade;
        UpdateGolds();
    }

    public void OnDisable()
    {
        ActionsManager.OnEntityKilled -= OnEntityKilled;
        ActionsManager.OnPlayerKilled -= OnPlayerKilled;
        ActionsManager.OnSelectDefinitiveUpgrade -= OnSelectDefinitiveUpgrade;
    }

    public void OnEntityKilled(MonsterData monsterData)
    {
        //_gameManager.AddGolds(entity.GetGolds());
        UpdateGolds();
    }

    public void OnPlayerKilled()
    {
        _gameManager.OnPlayerKilled();
        UpdateGolds();
    }

    public void OnSelectDefinitiveUpgrade(Upgrade upgrade)
    {
        UpdateGolds();
    }

    public void UpdateGolds()
    {
        //if (_goldAmount != null) _goldAmount.text = _gameManager.GetGolds().ToString();
    }
}

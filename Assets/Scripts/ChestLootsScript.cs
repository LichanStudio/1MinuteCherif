using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestLootsScript : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private UpgradesManager _upgradesManager;

    [Header("Upgrade Effectiveness Settings")]
    [SerializeField] private float _effectivenessDescreaseSpeed = 0.025f;
    [SerializeField] private float _maxEffectiveness = 1f;
    [SerializeField] private float _minEffectiveness = 0.4f;

    [Header("Settings")]
    [SerializeField] private TextMeshProUGUI _goingWrongValue;
    [SerializeField] private TextMeshProUGUI _everythingLabel;
    [SerializeField] private List<UpgradeUIScript> _upgradesUI = new();

    private bool _goingWrong = false;
    private bool _everythingGoingWrong = false;
    private float _goingWrongPercent = 0f;

    public void OnEnable()
    {
        _goingWrong = false;
        _everythingGoingWrong = false;
        _goingWrongPercent = 0;
        _everythingLabel.gameObject.SetActive(false);
        ActionsManager.OnSelectUpgrade += OnSelectUpgrade;
        StartCoroutine(DelayClickGoingWrong());
    }

    public void OnDisable()
    {
        ActionsManager.OnSelectUpgrade -= OnSelectUpgrade;
    }

    public void Update()
    {
        if (_goingWrong)
        {
            _goingWrongPercent += _effectivenessDescreaseSpeed * Time.unscaledDeltaTime;
            float playerEff = _maxEffectiveness - _goingWrongPercent;
            float enemyEff = _goingWrongPercent;
            if (!_everythingGoingWrong)
            {
                playerEff = Mathf.Clamp(playerEff, _minEffectiveness, _maxEffectiveness);
                enemyEff = Mathf.Clamp(enemyEff, 0f, _maxEffectiveness);
            }
            else
            {
                enemyEff += 0.2f;
            }
            if (_goingWrongValue != null) _goingWrongValue.text = Mathf.RoundToInt(enemyEff * 100).ToString();
            UpdateLoots(playerEff, enemyEff);
        }
    }

    public void GenerateLoots(bool goingWrong = false)
    {
        _gameManager.TogglePause(true);
        gameObject.SetActive(true);
        if (goingWrong) _goingWrongPercent = 0.3f;
        _everythingGoingWrong = goingWrong;
        _everythingLabel.gameObject.SetActive(goingWrong);
        for (int i = 0; i < _upgradesUI.Count; i++)
        {
            _upgradesUI[i].SetUpgrades(_upgradesManager.GenerateLoots(goingWrong), _maxEffectiveness, _goingWrongPercent);
        }
    }

    public void UpdateLoots(float playerEff, float enemyEff)
    {
        for (int i = 0; i < _upgradesUI.Count; i++)
        {
            _upgradesUI[i].UpdateUI(playerEff, enemyEff);
        }
    }

    public void OnSelectUpgrade(CalculatedUpgradeClass playerUpgrade, CalculatedUpgradeClass enemyUpgrade)
    {
        gameObject.SetActive(false);
        _gameManager.TogglePause(false);
    }

    private IEnumerator DelayClickGoingWrong()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        _goingWrong = true;
    }
}

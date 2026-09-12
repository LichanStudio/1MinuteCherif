using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private Coroutine _coroutine;
    private int _secondsPlayed = 0;
    private int _secondsPickUpgrade = 0;
    private int _totalSeconds = 0;

    private bool _isPickingUpgrade = false;
    private bool _isPlaying = false;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        _isPlaying = false;
        _isPickingUpgrade = false;
        _secondsPickUpgrade = 0;
        _secondsPlayed = 0;
        _totalSeconds = 0;
    }

    public void OnEnable()
    {
        ActionsManager.OnPlayerKilled += OnPlayerKilled;
        ActionsManager.OnStartEvent += OnStartEvent;
        ActionsManager.OnSelectUpgrade += OnSelectUpgrade;
        ActionsManager.OnStartUpgradeSelection += OnStartUpgradeSelection;
        StartTimer();
    }

    public void OnDisable()
    {
        ActionsManager.OnStartEvent -= OnStartEvent;
        ActionsManager.OnPlayerKilled -= OnPlayerKilled;
        ActionsManager.OnSelectUpgrade -= OnSelectUpgrade;
        ActionsManager.OnStartUpgradeSelection -= OnStartUpgradeSelection;
        StopTimer();
    }

    private void OnStartEvent()
    {
        _isPlaying = true;
        _isPickingUpgrade = false;
        _secondsPickUpgrade = 0;
        _secondsPlayed = 0;
        UpdateTime();
    }

    private void UpdateTime()
    {
        if (!GameManager.Instance.IsGamePaused()) ActionsManager.OnUpdateTime?.Invoke(_secondsPlayed, GameManager.Instance.GetSessionDuration());
        ActionsManager.OnUpdateRealTime?.Invoke();
    }

    private IEnumerator AddSecondEverySecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            _totalSeconds++;
            if (_isPlaying && !GameManager.Instance.IsGamePaused()) _secondsPlayed++;
            if (_isPickingUpgrade) _secondsPickUpgrade++;
            UpdateTime();

            if (!_isPickingUpgrade && _isPlaying)
            {
                if (GetSecondsLeft() <= 0)
                {
                    _isPlaying = false;
                    ActionsManager.OnEndEvent?.Invoke();
                }
                if (_secondsPlayed > 1 && GetSecondsLeft() % 10 == 0)
                {
                    ActionsManager.OnStartUpgradeSelection?.Invoke();
                }
            }
        }
    }

    private void OnStartUpgradeSelection()
    {
        _isPickingUpgrade = true;
    }

    private void OnSelectUpgrade(Stats upgrade, Stats previousUpgrade)
    {
        _isPickingUpgrade = false;
    }

    private void StopTimer()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
    }

    private void StartTimer()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(AddSecondEverySecond());
    }

    private void OnPlayerKilled()
    {
        StopTimer();
    }

    public int GetSecondsPlayed() => _secondsPlayed;
    public int GetSecondsPickUpgrade() => _secondsPickUpgrade;

    public int GetTotalSeconds() => _totalSeconds;

    public int GetSecondsLeft()
    {
        return GameManager.Instance.GetSessionDuration() - _secondsPlayed;
    }
}

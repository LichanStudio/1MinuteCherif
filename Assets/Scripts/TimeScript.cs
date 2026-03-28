using System.Collections;
using TMPro;
using UnityEngine;

public class TimeScript : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameManager _gameManager;

    [Header("Game Objects")]
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private ChestLootsScript _chestLootsUI;

    private Coroutine _coroutine;

    public void OnEnable()
    {
        _gameManager.LoadGame();
        ActionsManager.OnPlayerKilled += OnPlayerKilled;
        ActionsManager.OnStartSession += OnStartSession;
        ActionsManager.OnStartSession?.Invoke();
    }

    public void OnDisable()
    {
        ActionsManager.OnStartSession -= OnStartSession;
        ActionsManager.OnPlayerKilled -= OnPlayerKilled;
    }

    private void OnStartSession()
    {
        _gameManager.StartNewSession();
        _coroutine = StartCoroutine(AddSecondEverySecond());
        UpdateTimer();
    }

    private IEnumerator AddSecondEverySecond()
    {
        if (_gameManager == null) yield break;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            _gameManager.AddSeconds(1);
            UpdateTimer();
            if (_gameManager.GetSecondsLeft() <= 0)
            {
                ActionsManager.OnEndSession?.Invoke();
                yield break;
            }
            if(_gameManager.GetTimePlayed() > 5 && _gameManager.GetSecondsLeft() % 10 == 0)
            {
                int percentWrong = Random.Range(0, _gameManager.GetTimePlayed() + 50);
                bool everythingWrong = percentWrong >= 50;
                if (_chestLootsUI != null) _chestLootsUI.GenerateLoots(everythingWrong);
            }
        }
    }

    public void UpdateTimer()
    {
        if(_timerText == null) return;
        _timerText.text = $"{_gameManager.GetSecondsLeft()}";
    }

    private void OnPlayerKilled()
    {
        StopCoroutine(_coroutine);
        _gameManager.AddSeconds(99);
        ActionsManager.OnEndSession?.Invoke();
    }
}

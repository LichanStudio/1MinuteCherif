using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    private List<SessionEventData> _sessionEvents;
    private int _currentEventIndex = 0;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnEnable()
    {
        ActionsManager.OnBeforeSessionStart += BeforeSessionStart;
        ActionsManager.OnEndEvent += OnEndEvent;
    }

    public void OnDisable()
    {
        ActionsManager.OnBeforeSessionStart -= BeforeSessionStart;
        ActionsManager.OnEndEvent -= OnEndEvent;
    }

    private void BeforeSessionStart()
    {
        GenerateSessionEvents();
        ActionsManager.OnStartSession?.Invoke();
        ActionsManager.OnStartEvent?.Invoke();
    }

    private void GenerateSessionEvents()
    {
        int numberOfEvents = Random.Range(MapsManager.Instance.GetActualMap().MinEvents, MapsManager.Instance.GetActualMap().MaxEvents + 1);
        _sessionEvents = new()
        {
            CreateNewEvent(SessionEventData.SessionEventType.MonstersWaves, null, true)
        };

        SessionEventData[] possibleEvents = MapsManager.Instance.GetActualMap().PossibleEvents;
        for (int i = 0; i < numberOfEvents; i++)
        {
            SessionEventData sessionEvent = GenerateSessionEvent(possibleEvents);
            _sessionEvents[i] = sessionEvent;
        }

        _sessionEvents.Add(CreateNewEvent(SessionEventData.SessionEventType.BossBattle, null, false));
        _sessionEvents.Add(CreateNewEvent(SessionEventData.SessionEventType.FinalChestLoot, null, false));
    }

    private SessionEventData CreateNewEvent(SessionEventData.SessionEventType eventType, string title = "", bool isTimeLimited = true)
    {
        SessionEventData newEvent = ScriptableObject.CreateInstance<SessionEventData>();
        newEvent.Title = title;
        newEvent.EventType = eventType;
        newEvent.IsTimeLimited = isTimeLimited;
        return newEvent;
    }

    private SessionEventData GenerateSessionEvent(SessionEventData[] possibleEvents)
    {
        return possibleEvents[Random.Range(0, possibleEvents.Length)];
    }

    private void OnEndEvent()
    {
        _currentEventIndex++;
        if (_currentEventIndex >= _sessionEvents.Count)
        {
            ActionsManager.OnEndSession?.Invoke();
            return;
        }
        if (_sessionEvents[_currentEventIndex].IsTimeLimited)
        {
            ActionsManager.OnStartEvent?.Invoke();
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "ScriptableObjects/Data/SessionEvent", order = 3)]
public class SessionEventData : ScriptableObject
{
    public enum SessionEventType
    {
        MonstersWaves = 0,
        BossBattle = 1,
        FinalChestLoot = 2,
        SlimeWave = 3,
        Custom = 99
    }

    [SerializeField] public SessionEventType EventType;
    [SerializeField] public string Title;
    [SerializeField] public bool IsTimeLimited;
}

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss", menuName = "ScriptableObjects/Data/Boss", order = 1)]
public class BossData : MonsterData
{
    [Header("Boss Specific Informations")]
    [SerializeField] private List<EntityScript.EntitySkill> _skills = new();
}
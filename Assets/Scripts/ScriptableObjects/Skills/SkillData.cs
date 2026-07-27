using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "ScriptableObjects/Data/Skill", order = 3)]
public class SkillData : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private string _skillName;
    [SerializeField] private SkillContext _skillContext;
    [SerializeField] private List<SkillBehaviour> _behaviours;

    public void Execute(EntityData caster, SkillContext context)
    {
        Debug.Log($"Activation de {_skillName}");
        foreach (SkillBehaviour behaviour in _behaviours)
        {
            behaviour.ApplyEffect(caster, context);
        }
    }

    public SkillContext Context => _skillContext;
}

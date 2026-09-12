using UnityEngine;

public abstract class SkillBehaviour : ScriptableObject
{
    public abstract void ApplyEffect(IEntityScript caster, SkillContext context);
}
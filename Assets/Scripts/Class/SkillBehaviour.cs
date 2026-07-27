using UnityEngine;

public abstract class SkillBehaviour : ScriptableObject
{
    public abstract void ApplyEffect(EntityData caster, SkillContext context);
}
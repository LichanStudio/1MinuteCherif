using UnityEngine;

[CreateAssetMenu(fileName = "ThrowSpiralBehaviour", menuName = "ScriptableObjects/Skills Behaviours/Throw Spiral")]
public class SkillThrowSpiral : SkillBehaviour
{
    public override void ApplyEffect(EntityData caster, SkillContext context)
    {
        ProjectilesManager.Instance.SpawnProjectilesSpiralAsync(caster, context);
    }
}
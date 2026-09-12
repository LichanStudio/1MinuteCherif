using UnityEngine;

[CreateAssetMenu(fileName = "ThrowBehaviour", menuName = "ScriptableObjects/Skills Behaviours/Throw")]
public class SkillThrow : SkillBehaviour
{
    public override void ApplyEffect(IEntityScript caster, SkillContext context)
    {
        ProjectilesManager.Instance.SpawnProjectilesAsync(caster, context);
    }
}
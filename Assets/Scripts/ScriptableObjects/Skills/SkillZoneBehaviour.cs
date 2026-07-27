using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "SkillZone", menuName = "ScriptableObjects/Skills Behaviours/Zone")]
public class SkillZoneBehaviour : SkillBehaviour
{
    public override void ApplyEffect(EntityData caster, SkillContext context)
    {
        if (context != null && context.PrefabZone != null)
        {
            GameObject zone = Instantiate(context.PrefabZone);
            zone.transform.position = context.InitialPosition;
            Vector2 direction = (context.TargetPosition - context.InitialPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            zone.transform.rotation = Quaternion.Euler(0, 0, angle-90f);
        }
    }
}
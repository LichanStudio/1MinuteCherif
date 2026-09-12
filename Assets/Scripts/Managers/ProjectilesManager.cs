using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ProjectilesManager : MonoBehaviour
{
    public static ProjectilesManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SpawnProjectilesAsync(IEntityScript caster, SkillContext skillContext)
    {
        StartCoroutine(SpawnProjectiles(caster, skillContext));
    }

    public void SpawnProjectilesSpiralAsync(IEntityScript caster, SkillContext skillContext)
    {
        StartCoroutine(SpawnProjectilesSpiral(caster, skillContext));
    }

    public IEnumerator SpawnProjectiles(IEntityScript caster, SkillContext skillContext)
    {
        /* Verify Unity Object */
        if (caster == null || (caster is MonoBehaviour mono && mono == null)) yield break;

        /* Verify EntityData */
        EntityData entityData = caster.BaseData;
        if (entityData == null || entityData.WeaponData == null) yield break;

        int projectilesToSpawn = 1 + entityData.WeaponData.GetMaxMultiShot() + entityData.GetMultiShot();
        float angle = projectilesToSpawn * 4f;
        float minAngle = -angle;
        float maxAngle = angle;
        float range = Mathf.Abs(minAngle) + Mathf.Abs(maxAngle);
        float procAngle = range / projectilesToSpawn;

        for (int i = 0; i < projectilesToSpawn; i++)
        {
            /* Re-check state before each yield frame */
            if (caster == null || (caster is MonoBehaviour activeMono && activeMono == null)) yield break;

            /* Same check for EntityData */
            entityData = caster.BaseData;
            if (entityData == null || entityData.WeaponData == null) yield break;

            SpawnProjectile(entityData, skillContext.InitialPosition, skillContext.TargetPosition, minAngle + (procAngle * i));
            yield return null;
        }
    }

    public IEnumerator SpawnProjectilesSpiral(IEntityScript caster, SkillContext skillContext)
    {
        /* Verify Unity Object */
        if (caster == null || (caster is MonoBehaviour mono && mono == null)) yield break;

        /* Verify EntityData */
        EntityData entityData = caster.BaseData;
        if (entityData == null || entityData.WeaponData == null) yield break;

        float procAngle = 360f / skillContext.Count;
        Vector3 initialTarget = skillContext.InitialPosition + new Vector3(0f, 1f, 0f);
        float waitSeconds = skillContext.Time > 0 ? skillContext.Time / skillContext.Count : 0f;
        for (int i = 0; i < skillContext.Count; i++)
        {
            SpawnProjectile(entityData, skillContext.InitialPosition, initialTarget, procAngle * i);
            if (skillContext.Time > 0f) yield return new WaitForSeconds(waitSeconds);
            else yield return null;

            /* Re-check state after each yield frame */
            if (caster == null || (caster is MonoBehaviour activeMono && activeMono == null)) yield break;

            /* Same check for EntityData */
            entityData = caster.BaseData;
            if (entityData == null || entityData.WeaponData == null) yield break;
        }
    }

    private void SpawnProjectile(EntityData caster, Vector2 origine, Vector2 target, float angle = 0f)
    {
        if (caster == null || caster.WeaponData == null) return;

        ActionsManager.OnSpawnProjectile?.Invoke();

        WeaponData weaponData = caster.WeaponData;
        GameObject newProjectile = weaponData.GetWeaponObject(origine);

        if (weaponData != null && newProjectile != null && newProjectile.TryGetComponent<ProjectileScript>(out var projectile))
        {
            projectile.SetInitialDirection(target, origine, angle);
            projectile.SetSpeed(weaponData.ProjectileSpeed);
            projectile.SetCasterData(CharacterManager.Instance.SelectedCharacter);
            projectile.SetTargetEnemies(caster is CharacterData);
        }
    }
}

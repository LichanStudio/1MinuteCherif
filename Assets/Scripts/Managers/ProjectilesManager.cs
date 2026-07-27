using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectilesManager : MonoBehaviour
{
    public static ProjectilesManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SpawnProjectilesAsync(EntityData caster, SkillContext skillContext)
    {
        StartCoroutine(SpawnProjectiles(caster, skillContext));
    }

    public void SpawnProjectilesSpiralAsync(EntityData caster, SkillContext skillContext)
    {
        StartCoroutine(SpawnProjectilesSpiral(caster, skillContext));
    }

    public IEnumerator SpawnProjectiles(EntityData caster, SkillContext skillContext)
    {
        if (caster == null || caster.IsDestroyed() || caster.WeaponData == null) yield break;
        int projectilesToSpawn = 1 + caster.WeaponData.GetMaxMultiShot() + caster.GetMultiShot();
        float angle = projectilesToSpawn * 4f;
        float minAngle = -angle;
        float maxAngle = angle;
        float range = Mathf.Abs(minAngle) + Mathf.Abs(maxAngle);
        float procAngle = range / projectilesToSpawn;
        for (int i = 0; i < projectilesToSpawn; i++)
        {
            if (caster == null || caster.IsDestroyed() || caster.WeaponData == null) yield break;
            SpawnProjectile(caster, skillContext.InitialPosition, skillContext.TargetPosition, minAngle + (procAngle * i));
            yield return null;
        }
    }

    public IEnumerator SpawnProjectilesSpiral(EntityData caster, SkillContext skillContext)
    {
        if (caster == null || caster.IsDestroyed() || caster.WeaponData == null) yield break;
        float procAngle = 360f / skillContext.Count;
        Vector3 initialTarget = skillContext.InitialPosition + new Vector3(0f, 1f, 0f);
        float waitSeconds = skillContext.Time > 0 ? skillContext.Time / skillContext.Count : 0f;
        for (int i = 0; i < skillContext.Count; i++)
        {
            if (caster == null || caster.IsDestroyed() || caster.WeaponData == null) yield break;
            SpawnProjectile(caster, skillContext.InitialPosition, initialTarget, procAngle * i);
            if (skillContext.Time > 0f) yield return new WaitForSeconds(waitSeconds);
            else yield return null;
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

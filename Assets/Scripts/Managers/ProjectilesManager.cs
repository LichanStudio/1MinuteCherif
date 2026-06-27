using System.Collections;
using UnityEngine;

public class ProjectilesManager : MonoBehaviour
{
    public static ProjectilesManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public IEnumerator SpawnProjectiles(EntityData caster, Vector2 origine, Vector2 target)
    {
        int projectilesToSpawn = 1 + caster.WeaponData.GetMaxMultiHit();
        float angle = projectilesToSpawn * 4f;
        float minAngle = -angle;
        float maxAngle = angle;
        float range = Mathf.Abs(minAngle) + Mathf.Abs(maxAngle);
        float procAngle = range / projectilesToSpawn;
        for (int i = 0; i < projectilesToSpawn; i++)
        {
            SpawnProjectile(caster, origine, target, minAngle + (procAngle * i));
            yield return null;
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
            Debug.Log($"Projectile spawned with speed: {caster is CharacterData}");
            projectile.SetTargetEnemies(caster is CharacterData);
        }
    }
}

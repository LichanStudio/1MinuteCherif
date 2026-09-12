using UnityEngine;

public interface IEntityScript
{
    EntityData BaseData { get; }

    void TakeDamage(int damage, GameObject dmgLabel = null);
}
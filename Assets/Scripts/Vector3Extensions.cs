using UnityEngine;
public static class Vector3Extensions
{
    /// <summary>
    /// Arrondit un Vector3 selon une précision donnée (ex: snap à un pas de 5f).
    /// </summary>
    public static Vector3 Snap(this Vector3 vector, float precision)
    {
        if (precision <= 0f) return vector; // Évite la division par zéro

        return new Vector3(
            Mathf.Round(vector.x / precision) * precision,
            Mathf.Round(vector.y / precision) * precision,
            Mathf.Round(vector.z / precision) * precision
        );
    }

    /// <summary>
    /// Version Vector2 si tu travailles uniquement en 2D.
    /// </summary>
    public static Vector2 Snap(this Vector2 vector, float precision)
    {
        if (precision <= 0f) return vector;

        return new Vector2(
            Mathf.Round(vector.x / precision) * precision,
            Mathf.Round(vector.y / precision) * precision
        );
    }
}

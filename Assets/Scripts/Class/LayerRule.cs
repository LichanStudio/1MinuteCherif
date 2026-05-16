using UnityEngine;

[System.Serializable]
public class LayerRule
{
    public TileType tileType;
    [Range(0f, 1f)] public float threshold;
}
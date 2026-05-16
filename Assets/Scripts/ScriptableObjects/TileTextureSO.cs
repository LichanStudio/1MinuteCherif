using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Texture", menuName = "ScriptableObjects/Data/TileTexture", order = 1)]
public class TileTexture : ScriptableObject
{
    [SerializeField] private TileType _tileType;
    [SerializeField] private Sprite[] _variants;

    public TileType TileType => _tileType;
    public Sprite[] Variants => _variants;
}

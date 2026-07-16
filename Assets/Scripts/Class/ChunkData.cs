[System.Serializable]
public enum TileType
{
    None,
    Grass,
    Dirt,
    Water,
    Sand,
    Stone,
    Montain,
    Snow,
    Lava
}

[System.Serializable]
public class TileLayer
{
    public string name;
    public TileType tileType;
    public bool[,] tiles;
    public int sortingOrder;

    public TileLayer(string name, TileType type, int chunkSize, int order)
    {
        this.name = name;
        this.tileType = type;
        this.sortingOrder = order;
        tiles = new bool[chunkSize, chunkSize];
    }
}
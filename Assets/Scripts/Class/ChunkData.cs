[System.Serializable]
public enum TileType
{
    Grass,
    Dirt,
    Water,
    Sand,
    Stone
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
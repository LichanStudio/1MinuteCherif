public static class TileSeeder
{
    public static int Compute(int chunkX, int chunkY, int localX, int localY, int layerIndex)
    {
        int worldX = chunkX * 10000 + localX;
        int worldY = chunkY * 10000 + localY;
        return HashCombine(HashCombine(worldX, worldY), layerIndex);
    }

    private static int HashCombine(int a, int b)
    {
        unchecked
        {
            return a ^ (b + (int)0x9e3779b9 + (a << 6) + (a >> 2));
        }
    }
}
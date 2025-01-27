using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileData
{
    public Tile tile;
    public int height;
    public int width;
    public int minimumXLocation;
    public int minimumYLocation;
    public int maximumXLocation;
    public int maximumYLocation;
}

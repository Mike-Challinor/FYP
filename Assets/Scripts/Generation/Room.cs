using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Room
{
    public int width;
    public int height;
    public int xLocation;
    public int yLocation;
    public int numberOfDoors;
    public List<Vector3Int> doorLocations; // List of door positions
    public List<Vector3Int> wallLocations; // List of wall positions
    public Direction entranceDirection;
    public int generationCount;
    public bool isSideRoom;
}
public enum Direction
{
    North,
    East,
    South,
    West,
    None
}

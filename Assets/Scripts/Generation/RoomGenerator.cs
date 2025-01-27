using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    // Tilemap variable
    public Tilemap m_wallTileMap;
    public Tilemap m_floorTileMap;

    // Tiles for walls
    public TileBase[] m_horizontalWallsTopUpper;
    public TileBase[] m_horizontalWallsBottom;
    public TileBase[] m_verticalWallsLeft;
    public TileBase[] m_verticalWallsRight;
    public TileBase[] m_bottomRightCornerWall;
    public TileBase[] m_bottomLeftCornerWall;
    public TileBase[] m_bottomLeftCornerWallUpper;
    public TileBase[] m_bottomRightCornerWallUpper;
    public TileBase m_topLeftCornerWall;
    public TileBase m_topLeftCornerWallUpper;
    public TileBase m_topLeftCornerWallLower;
    public TileBase m_topRightCornerWall;
    public TileBase m_insideWallCornerLeftUpper;
    public TileBase m_insideWallCornerLeftLower;
    public TileBase m_insideWallCornerRightUpper;
    public TileBase m_insideWallCornerRightLower;
    public TileBase m_verticalDoorwayTopLeft;
    public TileBase m_verticalDoorwayTopRight;
    public TileBase m_floorTile;

    // Room dimensions
    public int m_width;
    public int m_height;

    // Starting point location
    public int m_xLocation;
    public int m_yLocation;

    // Room settings
    public int m_numberOfDoorways = 1;
    public List<Vector3Int> m_doorLocations;
    public List<Vector3Int> m_wallLocations;
    public List<Vector3Int> m_floorLocations;

    // Generator settings
    public float m_drawTime = 0.0f;
    public bool m_isDrawing = false;
    public bool m_useTimer = true;

    private int m_distanceBetweenRooms;

    public void GenerateRoom(int width, int height, int xLocation, int yLocation, int numberOfDoors, List<Vector3Int> doorLocations, List<Vector3Int> wallLocations, List<Vector3Int> floorLocations)
    {
        // Ensure the game is running
        if (!Application.isPlaying)
        {
            Debug.LogWarning("The game must be running to generate the room.");
            return;
        }

        m_width = width;
        m_height = height;
        m_xLocation = xLocation;
        m_yLocation = yLocation;
        m_numberOfDoorways = numberOfDoors;
        m_doorLocations = doorLocations;
        m_wallLocations = wallLocations;
        m_floorLocations = floorLocations;

        StartCoroutine(StartDrawingRoom());
    }

    private IEnumerator StartDrawingRoom()
    {
        // Start drawing
        m_isDrawing = true;

        // Wait until walls and floors have been drawn
        yield return DrawWallsAndFloors();

        // Wait until doors have been drawn
        yield return DrawDoors();

        // End drawing
        m_isDrawing = false;
    }

    private IEnumerator DrawWallsAndFloors()
    {
        // Loop for drawing floors first
        foreach (Vector3Int position in m_floorLocations)
        {
            if (!m_floorTileMap.HasTile(position)) // Avoid overwriting existing tiles, if necessary
            {
                m_floorTileMap.SetTile(position, m_floorTile);
            }
        }

        //Loop for drawing the walls
        for (int j = 0; j <= m_height; j++) // Loop for height
        {
            for (int i = 0; i <= m_width; i++) // Loop for width
            {
                int yPos = m_yLocation + j;
                int xPos = m_xLocation + i;

                Vector3Int Position = new Vector3Int(xPos, yPos, 0);

                switch (j)
                {
                    case 0: // Bottom walls
                        if (i == 0)
                        {
                            var chosenWall = m_verticalWallsLeft[Random.Range(0, m_verticalWallsLeft.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);
                        }

                        else if (i == m_width)
                        {
                            var chosenWall = m_verticalWallsRight[Random.Range(0, m_verticalWallsRight.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);
                        }
                        else
                        {
                            var chosenWall = m_horizontalWallsBottom[Random.Range(0, m_horizontalWallsBottom.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);
                        }
                        break;

                    case 1: // Second from bottom wall
                        if (i == 0) // Left edge
                        {
                            var chosenWall = m_verticalWallsLeft[Random.Range(0, m_verticalWallsLeft.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);
                        }

                        else if (i == m_width)
                        {
                            var chosenWall = m_verticalWallsRight[Random.Range(0, m_verticalWallsRight.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);
                        }
                        else // Middle tiles
                        {
                            var chosenWall = m_horizontalWallsTopUpper[Random.Range(0, m_horizontalWallsTopUpper.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);
                        }
                        break;

                    case var _ when j == m_height: // Top walls
                        if (i == 0)
                        {
                            m_wallTileMap.SetTile(Position, m_topLeftCornerWall);
                        }
                        else if (i == m_width)
                        {
                            m_wallTileMap.SetTile(Position, m_topRightCornerWall);
                        }
                        else
                        {
                            var chosenWall = m_horizontalWallsTopUpper[Random.Range(0, m_horizontalWallsTopUpper.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);
                        }
                        break;

                    case var _ when j == m_height - 1: // One below top walls
                        if (i == 0) // Left edge
                        {
                            var chosenWall = m_verticalWallsLeft[Random.Range(0, m_verticalWallsLeft.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);
                        }

                        else if (i == m_width) // Right edge
                        {
                            var chosenWall = m_verticalWallsRight[Random.Range(0, m_verticalWallsRight.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);
                        }

                        else // All walls second from top that aren't edges
                        {
                            var chosenWall = m_horizontalWallsBottom[Random.Range(0, m_horizontalWallsBottom.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);
                        }
                        break;


                    default: // For other rows
                        if (i == 0 && !m_doorLocations.Contains(Position)) // Left edge
                        {
                            var chosenWall = m_verticalWallsLeft[Random.Range(0, m_verticalWallsLeft.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);

                        }
                        else if (i == m_width && !m_doorLocations.Contains(Position)) // Right edge
                        {
                            var chosenWall = m_verticalWallsRight[Random.Range(0, m_verticalWallsRight.Length)];
                            m_wallTileMap.SetTile(Position, chosenWall);
                        }
                        break;
                }

                if (m_useTimer) { yield return StartCoroutine(DrawTimer()); }

            }
        }
    }

    private IEnumerator DrawDoors()
    {
        // Loop for drawing the doors
        for (int d = 0; d < m_numberOfDoorways; d++)
        {
            // Check whether door location is on left edge
            if (m_doorLocations[d].x == m_xLocation)
            {
                // Get first position of the doorway
                Vector3Int Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y - 1, 0);
                var chosenWall = m_horizontalWallsTopUpper[Random.Range(0, m_horizontalWallsTopUpper.Length)];
                m_wallTileMap.SetTile(Position, chosenWall);

                // Second position of doorway
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y - 2, 0);
                chosenWall = m_horizontalWallsBottom[Random.Range(0, m_horizontalWallsBottom.Length)];
                m_wallTileMap.SetTile(Position, chosenWall);

                // Third position of doorway
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y + 2, 0);
                m_wallTileMap.SetTile(Position, m_insideWallCornerLeftLower);

                // Fourth position of doorway
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y + 3, 0);
                m_wallTileMap.SetTile(Position, m_insideWallCornerLeftUpper);

                yield return DrawConnectingCorridors(m_doorLocations[d], 0);

                // Delete unrequired walls
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y + 0, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y + 1, 0);
                m_wallTileMap.SetTile(Position, null);
            }

            // Check whether door location is on right edge
            else if (m_doorLocations[d].x == m_xLocation + m_width)
            {
                // Get first position of the doorway
                Vector3Int Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y - 1, 0);
                var chosenWall = m_horizontalWallsTopUpper[Random.Range(0, m_horizontalWallsTopUpper.Length)];
                m_wallTileMap.SetTile(Position, chosenWall);

                // Second position of doorway
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y - 2, 0);
                chosenWall = m_horizontalWallsBottom[Random.Range(0, m_horizontalWallsBottom.Length)];
                m_wallTileMap.SetTile(Position, chosenWall);

                // Third position of doorway
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y + 2, 0);
                m_wallTileMap.SetTile(Position, m_insideWallCornerRightLower);

                // Fourth position of doorway
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y + 3, 0);
                m_wallTileMap.SetTile(Position, m_insideWallCornerRightUpper);

                yield return DrawConnectingCorridors(m_doorLocations[d], 1);

                // Delete unrequired walls
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y + 0, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y + 1, 0);
                m_wallTileMap.SetTile(Position, null);
            }

            // Check whether door location is on bottom edge
            else if (m_doorLocations[d].y == m_yLocation)
            {
                // Get first position of the doorway
                Vector3Int Position = new Vector3Int(m_doorLocations[d].x - 1, m_doorLocations[d].y + 1, 0);
                m_wallTileMap.SetTile(Position, m_topRightCornerWall);

                // Second position of doorway
                Position = new Vector3Int(m_doorLocations[d].x - 1, m_doorLocations[d].y, 0);
                var chosenWall = m_verticalWallsRight[Random.Range(0, m_verticalWallsRight.Length)];
                m_wallTileMap.SetTile(Position, chosenWall);

                // Third position of doorway
                Position = new Vector3Int(m_doorLocations[d].x + 2, m_doorLocations[d].y + 1, 0);
                m_wallTileMap.SetTile(Position, m_topLeftCornerWall);

                // Fourth position of doorway
                Position = new Vector3Int(m_doorLocations[d].x + 2, m_doorLocations[d].y, 0);
                chosenWall = m_verticalWallsLeft[Random.Range(0, m_verticalWallsLeft.Length)];
                m_wallTileMap.SetTile(Position, chosenWall);

                yield return DrawConnectingCorridors(m_doorLocations[d], 2);

                // Delete unrequired walls
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y + 1, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x + 1, m_doorLocations[d].y, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x + 1, m_doorLocations[d].y + 1, 0);
                m_wallTileMap.SetTile(Position, null);

            }

            // Check whether door location is on top edge
            else if (m_doorLocations[d].y == m_yLocation + m_height)
            {

                // Get first position of the doorway
                Vector3Int Position = new Vector3Int(m_doorLocations[d].x - 2, m_doorLocations[d].y, 0);
                var chosenWall = m_bottomRightCornerWallUpper[Random.Range(0, m_bottomRightCornerWall.Length)];
                m_wallTileMap.SetTile(Position, chosenWall);

                // Second position of doorway
                Position = new Vector3Int(m_doorLocations[d].x - 2, m_doorLocations[d].y - 1, 0);
                chosenWall = m_bottomRightCornerWall[Random.Range(0, m_bottomRightCornerWall.Length)];
                m_wallTileMap.SetTile(Position, chosenWall);

                // Third position of doorway
                Position = new Vector3Int(m_doorLocations[d].x + 3, m_doorLocations[d].y, 0);
                chosenWall = m_bottomLeftCornerWallUpper[Random.Range(0, m_bottomLeftCornerWall.Length)];
                m_wallTileMap.SetTile(Position, chosenWall);

                // Fourth position of doorway
                Position = new Vector3Int(m_doorLocations[d].x + 3, m_doorLocations[d].y - 1, 0);
                chosenWall = m_bottomLeftCornerWall[Random.Range(0, m_bottomLeftCornerWall.Length)];
                m_wallTileMap.SetTile(Position, chosenWall);

                yield return DrawConnectingCorridors(m_doorLocations[d], 3);

                // Delete unrequired walls
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x, m_doorLocations[d].y - 1, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x + 1, m_doorLocations[d].y, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x + 1, m_doorLocations[d].y - 1, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x + 2, m_doorLocations[d].y, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x + 2, m_doorLocations[d].y - 1, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x - 1, m_doorLocations[d].y, 0);
                m_wallTileMap.SetTile(Position, null);
                Position = new Vector3Int(m_doorLocations[d].x - 1, m_doorLocations[d].y - 1, 0);
                m_wallTileMap.SetTile(Position, null);

            }

            if (m_useTimer) { yield return StartCoroutine(DrawTimer()); }

        }
    }

    private IEnumerator DrawConnectingCorridors(Vector3Int doorLocation, int direction)
    {
        Vector3Int Position = new Vector3Int(doorLocation.x, doorLocation.y, 0);

        switch (direction)
        {
            case 0: // Left direction

                for (int i = 1; i <= m_distanceBetweenRooms; i++)
                {
                    // Top walls
                    var chosenWall = m_horizontalWallsTopUpper[Random.Range(0, m_horizontalWallsTopUpper.Length)];
                    Position = new Vector3Int(doorLocation.x - i, doorLocation.y - 1, 0); // Bottom wall (top)
                    m_wallTileMap.SetTile(Position, chosenWall);
                    Position = new Vector3Int(doorLocation.x - i, doorLocation.y + 3, 0); // Top wall (top)
                    m_wallTileMap.SetTile(Position, chosenWall);

                    // Bottom walls
                    chosenWall = m_horizontalWallsBottom[Random.Range(0, m_horizontalWallsBottom.Length)];
                    Position = new Vector3Int(doorLocation.x - i, doorLocation.y - 2, 0); // Bottom wall (bottom)
                    m_wallTileMap.SetTile(Position, chosenWall);
                    Position = new Vector3Int(doorLocation.x - i, doorLocation.y + 2, 0); // Top wall (bottom)
                    m_wallTileMap.SetTile(Position, chosenWall);

                    // Floor
                    Position = new Vector3Int(doorLocation.x, doorLocation.y, 0); // Bottom floor (right) 
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x - i, doorLocation.y, 0); // Bottom floor (left)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x, doorLocation.y + 1, 0); // Top floor (right)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x - i, doorLocation.y + 1, 0); // Top floor (left)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                }
                break;

            case 1: // Right direction

                for (int i = 1; i <= m_distanceBetweenRooms; i++)
                {
                    // Top walls
                    var chosenWall = m_horizontalWallsTopUpper[Random.Range(0, m_horizontalWallsTopUpper.Length)];
                    Position = new Vector3Int(doorLocation.x + i, doorLocation.y - 1, 0); // Bottom wall (top)
                    m_wallTileMap.SetTile(Position, chosenWall);
                    Position = new Vector3Int(doorLocation.x + i, doorLocation.y + 3, 0); // Bottom wall (top)
                    m_wallTileMap.SetTile(Position, chosenWall);

                    // Bottom walls
                    chosenWall = m_horizontalWallsBottom[Random.Range(0, m_horizontalWallsBottom.Length)];
                    Position = new Vector3Int(doorLocation.x + i, doorLocation.y - 2, 0); // Top wall (bottom)
                    m_wallTileMap.SetTile(Position, chosenWall);
                    Position = new Vector3Int(doorLocation.x + i, doorLocation.y + 2, 0); // Top wall (bottom
                    m_wallTileMap.SetTile(Position, chosenWall);

                    // Floor
                    Position = new Vector3Int(doorLocation.x + i, doorLocation.y, 0); // Bottom floor
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x + i, doorLocation.y + 1, 0); // Top floor
                    m_floorTileMap.SetTile(Position, m_floorTile);
                }
                break;

            case 2: // Downwards direction
                for (int i = 1; i <= m_distanceBetweenRooms; i++)
                {
                    // Top walls
                    var chosenWall = m_verticalWallsRight[Random.Range(0, m_verticalWallsRight.Length)];
                    Position = new Vector3Int(doorLocation.x - 1, doorLocation.y - i, 0); // Left wall (top)
                    m_wallTileMap.SetTile(Position, chosenWall);
                    Position = new Vector3Int(doorLocation.x - 1, doorLocation.y - i - 1, 0); // Left wall (bottom)
                    m_wallTileMap.SetTile(Position, chosenWall);

                    // Bottom walls
                    chosenWall = m_verticalWallsLeft[Random.Range(0, m_verticalWallsLeft.Length)];
                    Position = new Vector3Int(doorLocation.x + 2, doorLocation.y - i, 0); // Right wall (top)
                    m_wallTileMap.SetTile(Position, chosenWall);
                    chosenWall = m_verticalWallsLeft[Random.Range(0, m_verticalWallsLeft.Length)];
                    Position = new Vector3Int(doorLocation.x + 2, doorLocation.y - i - 1, 0); // Right wall (bottom)
                    m_wallTileMap.SetTile(Position, chosenWall);

                    // Floor
                    Position = new Vector3Int(doorLocation.x - 1, doorLocation.y + 1, 0); // Far left floor (top)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x - 1, doorLocation.y, 0); // Far left floor (top middle)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x - 1, doorLocation.y - i, 0); // Far left floor (middle)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x - 1, doorLocation.y - i - 1, 0); // Far left floor (bottom)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x, doorLocation.y + 1, 0); // Left floor (top )
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x, doorLocation.y, 0); // Left floor (top middle)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x, doorLocation.y - i, 0); // Left floor (middle)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x, doorLocation.y - i - 1, 0); // Left floor (bottom)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x + 1, doorLocation.y + 1, 0); // Right floor (top)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x + 1, doorLocation.y, 0); // Right floor (top middle)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x + 1, doorLocation.y - i, 0); // Right floor (middle)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x + 1, doorLocation.y - i - 1, 0); // Right floor (bottom)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x + 2, doorLocation.y + 1, 0); // Far Right floor (top)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x + 2, doorLocation.y, 0); // Far Right floor (top middle)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x + 2, doorLocation.y - i, 0); // Far Right floor (middle)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                    Position = new Vector3Int(doorLocation.x + 2, doorLocation.y - i - 1, 0); // Far Right floor (bottom)
                    m_floorTileMap.SetTile(Position, m_floorTile);
                }
                break;
        }

        if (m_useTimer) { yield return DrawTimer(); }
        
    }

    private IEnumerator DrawTimer()
    {
        yield return new WaitForSeconds(m_drawTime);
    }

    public void SetUseTimer(bool usetimer)
    {
        m_useTimer = usetimer;
    }

    public void SetDistanceBetweenRooms(int distanceBetweenRooms)
    {
        m_distanceBetweenRooms = distanceBetweenRooms;
    }
}

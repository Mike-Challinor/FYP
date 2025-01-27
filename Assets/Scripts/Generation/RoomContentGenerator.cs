using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class RoomContentGenerator : MonoBehaviour
{
    // Tilemap variable
    public Tilemap m_propTileMap;

    // Tile variables
    public Tile m_wellTile;

    // Tile data settings
    private TileData m_wellTileData;

    // Room dimensions
    public int m_width;
    public int m_height;

    // Starting point location
    public int m_xLocation;
    public int m_yLocation;

    // Room settings
    public List<Vector3Int> m_doorLocations;
    public List<Vector3Int> m_wallLocations;
    public List<Vector3Int> m_floorLocations;
    public List<Vector3Int> m_possibleTileLocations;
    private List<Vector3Int> m_alterList = new List<Vector3Int>();
    public bool m_isSideRoom;

    // Generator settings
    public float m_drawTime = 0.0f;
    public bool m_isDrawing = false;
    public bool m_useTimer = true;

    private void Start()
    {
        m_wellTileData = new TileData()
        {
            tile = m_wellTile,
            height = 3,
            width = 3,
            minimumXLocation = 4,
            minimumYLocation = 2,
            maximumXLocation = 4,
            maximumYLocation = 5
        };
    }

    public void GenerateRoom(int width, int height, int xLocation, int yLocation, List<Vector3Int> doorLocations, List<Vector3Int> wallLocations, List<Vector3Int> floorLocations, bool isSideRoom)
    {
        // Ensure the game is running
        if (!Application.isPlaying)
        {
            Debug.LogWarning("The game must be running to generate the room content.");
            return;
        }

        m_width = width;
        m_height = height;
        m_xLocation = xLocation;
        m_yLocation = yLocation;
        m_doorLocations = doorLocations;
        m_wallLocations = wallLocations;
        m_floorLocations = floorLocations;
        m_isSideRoom = isSideRoom;

        StartCoroutine(StartDrawingRoomContent());
    }

    private IEnumerator StartDrawingRoomContent()
    {
        Debug.Log("Starting to draw room content");

        m_isDrawing = true;

        Debug.Log($"XLocation is: {m_xLocation} YLocation is: {m_yLocation} Width is: {m_width} Height is: {m_height} Room halfway is: {m_width / 2} ");

        // Spawn the wells in the scene
        if (m_wellTile != null)
        {
            // Set the amount of wells in the scene between 1 and 4
            int amountToDraw = RandomNumberGenerator(1, 4);
            for (int i = 0; i < amountToDraw; i++)
            {
                Vector3Int position;

                if (i == 0) // Place the first tile
                {
                    position = new Vector3Int(
                        RandomNumberGenerator(m_xLocation + m_wellTileData.minimumXLocation, m_xLocation + m_width - m_wellTileData.maximumXLocation),
                        RandomNumberGenerator(m_yLocation + m_wellTileData.minimumYLocation, m_yLocation + m_height - m_wellTileData.maximumYLocation),
                        0
                    );

                    // Validate position
                    while (position.x == m_xLocation + m_width / 2 || position.x == m_xLocation + m_width / 2 + 1 || position.x == m_xLocation + m_width / 2 + 2 || position.x == m_xLocation + m_width / 2 - 1 || position.x == m_xLocation + m_width / 2 - 2 || position.x == m_xLocation + m_width / 2 + 3)
                    {
                        Debug.Log($"X Position is {position.x} and the halfway point is: {position.x == position.x + m_width / 2}");
                        position.x = RandomNumberGenerator(m_xLocation + m_wellTileData.minimumXLocation, m_xLocation + m_width - m_wellTileData.maximumXLocation);
                    }

                    while (position.y == m_yLocation + m_height / 2 || position.y == m_yLocation + m_height / 2 - 1 || position.y == m_yLocation + m_height / 2 - 2 || position.y == m_yLocation + m_height / 2 + 1)
                    {
                        position.y = RandomNumberGenerator(m_yLocation + m_wellTileData.minimumYLocation, m_yLocation + m_height - m_wellTileData.maximumYLocation);
                    }

                    Debug.Log($"Placing first tile at: {position}");
                    m_propTileMap.SetTile(position, m_wellTileData.tile);
                    m_alterList.Add(position);
                }
                
                else if (i == 1) // Place the second tile parallel horizontally
                {
                    Vector3Int previousPos = m_alterList[m_alterList.Count - 1];
                    Vector3Int leftWall = new Vector3Int(m_xLocation, previousPos.y, 0);
                    Vector3Int rightWall = new Vector3Int(m_xLocation + m_width, previousPos.y, 0);

                    int xPosition;

                    if (Vector3Int.Distance(leftWall, previousPos) < Vector3Int.Distance(rightWall, previousPos)) // Nearer to the left wall
                    {
                        xPosition = m_xLocation + m_width - (previousPos.x - leftWall.x);
                    }
                    else
                    {
                        xPosition = m_xLocation + (rightWall.x - previousPos.x);
                    }

                    position = new Vector3Int(xPosition, previousPos.y, 0);

                    if (IsPositionValid(position))
                    {
                        Debug.Log($"Placing second tile at: {position}");
                        m_propTileMap.SetTile(position, m_wellTileData.tile);
                        m_alterList.Add(position);
                    }

                    else
                    {
                        Debug.Log($"Unable to place second tile at: {position}");
                    }
                }

                
                else // Place subsequent tiles vertically
                {
                    Vector3Int previousPos = m_alterList[0];

                    if (i == 2)
                    {
                        previousPos = m_alterList[m_alterList.Count - 1];
                    }

                    Vector3Int topWall = new Vector3Int(previousPos.x, m_yLocation + m_height, 0);
                    Vector3Int bottomWall = new Vector3Int(previousPos.x, m_yLocation, 0);

                    int yPosition;

                    if (Vector3Int.Distance(bottomWall, previousPos) < Vector3Int.Distance(topWall, previousPos)) // Closer to the bottom wall
                    {
                        yPosition = m_yLocation + m_height - (previousPos.y - bottomWall.y) - m_wellTileData.height;
                    }
                    else // Closer to the top wall
                    {
                        yPosition = m_yLocation + (topWall.y - previousPos.y);
                    }

                    position = new Vector3Int(previousPos.x, yPosition, 0);

                    if (IsPositionValid(position))
                    {
                        Debug.Log($"Placing subsequent tile at: {position}");
                        m_propTileMap.SetTile(position, m_wellTileData.tile);
                        m_alterList.Add(position);
                    }
                }
            }
        }

        if (m_useTimer)
        {
            yield return StartCoroutine(DrawTimer());
            m_isDrawing = false;
        }

        m_isDrawing = false;

        Debug.Log("Ended drawing room content");
    }

    private bool IsPositionValid(Vector3Int position)
    {
        return position.x >= m_xLocation && position.x < m_xLocation + m_width &&
               position.y >= m_yLocation && position.y < m_yLocation + m_height;
    }

    private IEnumerator DrawTimer()
    {
        yield return new WaitForSeconds(m_drawTime);
    }

    public void SetUseTimer(bool usetimer)
    {
        m_useTimer = usetimer;
    }

    private int RandomNumberGenerator(int min, int max)
    {
        return Random.Range(min, max + 1);
    }
}

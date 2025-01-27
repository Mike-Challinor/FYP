using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RoomContentGenerator : MonoBehaviour
{
    // Tilemap variable
    public Tilemap m_propTileMapNoCollision;
    public Tilemap m_propTileMapCollision;

    // Tile variables
    public Tile m_altarTile;
    public TileBase[] m_altarTileArray;
    public TileBase[] m_pillarTileArray;
    public TileBase[] m_objectTileArrayOneHeight;
    public TileBase[] m_objectTileArrayTwoHeightLeft;
    public TileBase[] m_objectTileArrayTwoHeightRight;
    public TileBase[] m_objectTileArrayThreeWidth;

    // Tile data settings
    private TileData m_altarTileData;

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
    public List<Vector3Int> m_possibleTileLocations = new List<Vector3Int>();
    private List<Vector3Int> m_alterList = new List<Vector3Int>();
    private List<Vector3Int> m_pillarList = new List<Vector3Int>();
    public bool m_isSideRoom;

    // Generator settingsfor each
    public float m_drawTime = 0.0f;
    public bool m_isDrawing = false;
    public bool m_useTimer = true;
    public int m_maxNumberOfObjects = 10;

    private void Start()
    {
        m_altarTileData = new TileData()
        {
            tile = m_altarTile,
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

        // Reset possible tile locations
        if (m_possibleTileLocations.Count != 0)
        {
            m_possibleTileLocations.Clear();
        }

        // Add all floor locations as possible positions
        foreach (Vector3Int position in m_floorLocations)
        {
            if (position.x == m_xLocation + m_width - 1 || position.y == m_yLocation + 2 || position.y == m_yLocation + m_height - 2)
            {
                m_possibleTileLocations.Add(position);
            }
        }

        // Remove door locations as possible locations
        List<Vector3Int> positionsToRemove = new List<Vector3Int>(); // Temporary list to store positions to remove

        // Remove door locations as possible locations
        foreach (Vector3Int position in m_possibleTileLocations)
        {
            foreach (Vector3Int location in m_doorLocations)
            {
                if (position.x == location.x)
                {
                    positionsToRemove.Add(position);
                    positionsToRemove.Add(new Vector3Int(position.x - 1, position.y, position.z));
                    positionsToRemove.Add(new Vector3Int(position.x + 1, position.y, position.z));
                    positionsToRemove.Add(new Vector3Int(position.x + 2, position.y, position.z));
                }
                else if (position.y == location.y)
                {
                    positionsToRemove.Add(position);
                    positionsToRemove.Add(new Vector3Int(position.x, position.y - 1, position.z));
                    positionsToRemove.Add(new Vector3Int(position.x, position.y - 2, position.z));
                    positionsToRemove.Add(new Vector3Int(position.x, position.y + 1, position.z));
                }
            }
        }

        // Remove all collected positions
        foreach (Vector3Int pos in positionsToRemove.Distinct().ToList()) // Use Distinct to avoid duplicate removals
        {
            m_possibleTileLocations.Remove(pos);
        }

        StartCoroutine(StartDrawingRoomContent());
    }

    private IEnumerator StartDrawingRoomContent()
    {
        Debug.Log("Starting to draw room content");

        m_isDrawing = true;

        Debug.Log($"XLocation is: {m_xLocation} YLocation is: {m_yLocation} Width is: {m_width} Height is: {m_height} Room halfway is: {m_width / 2} ");

        // Set the altars in the scene
        StartCoroutine(SetAltarLocations());
        yield return SetAltarLocations();

        // Set the pillars in the scene
        StartCoroutine(SetPillarLocations());
        yield return SetPillarLocations();

        // Set the objects in the scene
        //StartCoroutine(SetObjectLocations());
        //yield return SetObjectLocations();

        m_isDrawing = false;

        Debug.Log("Ended drawing room content");

    }

    private IEnumerator SetAltarLocations()
    {
        // Set the amount of wells in the scene between 1 and 4
        int amountToDraw = RandomNumberGenerator(1, 4);
        for (int i = 0; i < amountToDraw; i++)
        {
            Vector3Int position;

            if (i == 0) // Place the first tile
            {
                position = new Vector3Int(
                    RandomNumberGenerator(m_xLocation + m_altarTileData.minimumXLocation, m_xLocation + m_width - m_altarTileData.maximumXLocation),
                    RandomNumberGenerator(m_yLocation + m_altarTileData.minimumYLocation, m_yLocation + m_height - m_altarTileData.maximumYLocation),
                    0
                );

                // Validate position
                while (position.x == m_xLocation + m_width / 2 || position.x == m_xLocation + m_width / 2 + 1 || position.x == m_xLocation + m_width / 2 + 2 || position.x == m_xLocation + m_width / 2 - 1 || position.x == m_xLocation + m_width / 2 - 2 || position.x == m_xLocation + m_width / 2 + 3)
                {
                    Debug.Log($"X Position is {position.x} and the halfway point is: {position.x == position.x + m_width / 2}");
                    position.x = RandomNumberGenerator(m_xLocation + m_altarTileData.minimumXLocation, m_xLocation + m_width - m_altarTileData.maximumXLocation);
                }

                while (position.y == m_yLocation + m_height / 2 || position.y == m_yLocation + m_height / 2 - 1 || position.y == m_yLocation + m_height / 2 - 2 || position.y == m_yLocation + m_height / 2 + 1)
                {
                    position.y = RandomNumberGenerator(m_yLocation + m_altarTileData.minimumYLocation, m_yLocation + m_height - m_altarTileData.maximumYLocation);
                }

                Debug.Log($"Placing first tile at: {position}");
                m_propTileMapNoCollision.SetTile(position, m_altarTileData.tile);
                m_alterList.Add(position);

                if (m_possibleTileLocations.Contains(position))
                {
                    // Remove bottom row of the alter from available positions
                    m_possibleTileLocations.Remove(position);
                    position.x = position.x - 1;
                    m_possibleTileLocations.Remove(position);
                    position.x = position.x + 2;
                    m_possibleTileLocations.Remove(position);

                    // Remove middle row of the alter from available positions
                    position.y = position.y - 1;
                    m_possibleTileLocations.Remove(position);
                    position.x = position.x - 1;
                    m_possibleTileLocations.Remove(position);
                    position.x = position.x - 1;
                    m_possibleTileLocations.Remove(position);

                    // Remove top row of the alter from available positions
                    position.y = position.y - 1;
                    m_possibleTileLocations.Remove(position);
                    position.x = position.x + 1;
                    m_possibleTileLocations.Remove(position);
                    position.x = position.x + 1;
                    m_possibleTileLocations.Remove(position);

                }
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
                    m_propTileMapNoCollision.SetTile(position, m_altarTileData.tile);
                    m_alterList.Add(position);

                    if (m_possibleTileLocations.Contains(position))
                    {
                        // Remove bottom row of the alter from available positions
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x - 1;
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x + 2;
                        m_possibleTileLocations.Remove(position);

                        // Remove middle row of the alter from available positions
                        position.y = position.y - 1;
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x - 1;
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x - 1;
                        m_possibleTileLocations.Remove(position);

                        // Remove top row of the alter from available positions
                        position.y = position.y - 1;
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x + 1;
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x + 1;
                        m_possibleTileLocations.Remove(position);

                    }
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
                    yPosition = m_yLocation + m_height - (previousPos.y - bottomWall.y) - m_altarTileData.height;
                }
                else // Closer to the top wall
                {
                    yPosition = m_yLocation + (topWall.y - previousPos.y);
                }

                position = new Vector3Int(previousPos.x, yPosition, 0);

                if (IsPositionValid(position))
                {
                    Debug.Log($"Placing subsequent tile at: {position}");
                    m_propTileMapNoCollision.SetTile(position, m_altarTileData.tile);
                    m_alterList.Add(position);

                    if (m_possibleTileLocations.Contains(position))
                    {
                        // Remove bottom row of the alter from available positions
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x - 1;
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x + 2;
                        m_possibleTileLocations.Remove(position);

                        // Remove middle row of the alter from available positions
                        position.y = position.y + 1;
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x - 1;
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x - 1;
                        m_possibleTileLocations.Remove(position);

                        // Remove top row of the alter from available positions
                        position.y = position.y + 1;
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x + 1;
                        m_possibleTileLocations.Remove(position);
                        position.x = position.x + 1;
                        m_possibleTileLocations.Remove(position);

                    }
                }
            }


            int placeTileOnAltar = RandomNumberGenerator(0, 21);

            // Check whether to spawn item on the altar
            if (placeTileOnAltar > 12)
            {
                Debug.Log("Place tile on altar");

                // Select a random object to place on the altar
                var chosenTile = m_altarTileArray[Random.Range(0, m_altarTileArray.Length)];

                // Get position of the last altar added
                position = m_alterList[m_alterList.Count - 1];

                // Update position to be the centre of the altars position
                position.y = position.y + 1;

                // Set tile
                m_propTileMapCollision.SetTile(position, chosenTile);
            }

            else
            {
                // Dont spawn an object on the altar
                Debug.Log("Don't place tile on altar");
            }

            if (m_useTimer) { yield return StartCoroutine(DrawTimer()); }

        }

    }

    private IEnumerator SetPillarLocations()
    {
        int numberOfPillars = RandomNumberGenerator(1, 2); // Set the number of pillars to 1 or 2 (each pillar will have its position mirrored)

        // Set pillars on the top wall
        for (int i = 0; i < numberOfPillars; i++)
        {
            if (i == 0)
            {
                // Place first pillar
                PlaceFirstPillarTopWall();

                // Place the second pillar
                PlaceSecondPillarTopWall();
            }

            else
            {
                // Place first pillar
                PlaceFirstPillarTopWall();

                // Place the second pillar
                PlaceSecondPillarTopWall();
            }

            if (m_useTimer) { yield return StartCoroutine(DrawTimer()); }

        }
    }
    
    private void PlaceFirstPillarTopWall()
    {
        Vector3Int position;
        int xPosition = RandomNumberGenerator(m_xLocation + 1, m_xLocation + m_width - 1);
        int yPosition = m_yLocation + m_height - 2;

        // Loop for checking whether position needs to be re-rolled
        while (m_doorLocations.Any(location => location.x == xPosition) ||
            m_doorLocations.Any(location => location.x + 1 == xPosition) ||
            m_doorLocations.Any(location => location.x + 2 == xPosition) ||
            m_doorLocations.Any(location => location.x + 3 == xPosition) ||
            m_doorLocations.Any(location => location.x - 1 == xPosition) ||
            m_pillarList.Any(location => location.x == xPosition))
        {
            xPosition = RandomNumberGenerator(m_xLocation + 1, m_xLocation + m_width - 1);
        }

        // Set the position of the pillar
        position = new Vector3Int(xPosition, yPosition, 0);

        // Select a random tile within the pillar array
        var chosenTile = m_pillarTileArray[Random.Range(0, m_pillarTileArray.Length)];

        // Set tile
        m_propTileMapCollision.SetTile(position, chosenTile);

        // Add positions to the list of pillar locations
        m_pillarList.Add(position);

        if (m_possibleTileLocations.Contains(position))
        {
            // Remove bottom tile of the pillar from available positions
            m_possibleTileLocations.Remove(position);

            // Remove middle tile of the pillar from available positions
            position.y = position.y + 1;
            m_possibleTileLocations.Remove(position);

            // Remove top tile of the pillar from available positions
            position.y = position.y + 1;
            m_possibleTileLocations.Remove(position);

        }

    }

    private void PlaceSecondPillarTopWall()
    {
        Vector3Int position;
        int xPosition;

        // Get the position of the previous pillar
        Vector3Int previousPos = m_pillarList[m_pillarList.Count - 1];
        Vector3Int leftWall = new Vector3Int(m_xLocation, previousPos.y, 0);
        Vector3Int rightWall = new Vector3Int(m_xLocation + m_width, previousPos.y, 0);

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

            // Select a random tile within the pillar array
            var chosenTile = m_pillarTileArray[Random.Range(0, m_pillarTileArray.Length)];

            // Set the tile
            m_propTileMapCollision.SetTile(position, chosenTile);

            // Add positions to the list of pillar locations
            m_pillarList.Add(position);

            if (m_possibleTileLocations.Contains(position))
            {
                // Remove bottom tile of the pillar from available positions
                m_possibleTileLocations.Remove(position);

                // Remove middle tile of the pillar from available positions
                position.y = position.y + 1;
                m_possibleTileLocations.Remove(position);

                // Remove top tile of the pillar from available positions
                position.y = position.y + 1;
                m_possibleTileLocations.Remove(position);

            }
        }

        else
        {
            Debug.Log($"Unable to place second tile at: {position}");
        }
    }

    private IEnumerator SetObjectLocations()
    {
        Debug.Log("Setting object locations");

        // Declare position variable
        Vector3Int position;

        int loopCount = m_maxNumberOfObjects;

        // Loop through for each object to place
        for (int i = 0; i < loopCount; i++)
        {
            // Break out of loop if no more objects can be placed
            if (m_possibleTileLocations.Count == 0)
            {
                Debug.LogError("No more possible locations");
                yield break;
            }

            position = m_possibleTileLocations[RandomNumberGenerator(0, m_possibleTileLocations.Count - 1)];

            if (position.x == m_xLocation + 1) // Is left wall
            {
                Vector3Int tempPosition;
                tempPosition = new Vector3Int(position.x, position.y + 1, position.z);

                // Check if the tile above is also free
                if (m_possibleTileLocations.Contains(tempPosition))
                {
                    // Select a random tile within the object array
                    var chosenTile = m_objectTileArrayTwoHeightLeft[Random.Range(0, m_objectTileArrayTwoHeightLeft.Length - 1)];

                    // Set the tile
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    // Remove tiles from possible locations list
                    m_possibleTileLocations.Remove(position);
                    m_possibleTileLocations.Remove(tempPosition);
                }

                else
                {
                    // Select a random tile within the object array
                    var chosenTile = m_objectTileArrayOneHeight[Random.Range(0, m_objectTileArrayOneHeight.Length - 1)];

                    // Set the tile
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    // Remove tile from possible locations list
                    m_possibleTileLocations.Remove(position);
                }

                loopCount--;
            }

            else if (position.x == m_xLocation + m_height - 1) // Is Right Wall
            {
                Vector3Int tempPosition;
                tempPosition = new Vector3Int(position.x, position.y + 1, position.z);

                // Check if the tile above is also free
                if (m_possibleTileLocations.Contains(tempPosition))
                {
                    // Select a random tile within the object array
                    var chosenTile = m_objectTileArrayTwoHeightRight[Random.Range(0, m_objectTileArrayTwoHeightRight.Length - 1)];

                    // Set the tile
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    // Remove tiles from possible locations list
                    m_possibleTileLocations.Remove(position);
                    m_possibleTileLocations.Remove(tempPosition);
                }

                else
                {
                    // Select a random tile within the object array
                    var chosenTile = m_objectTileArrayOneHeight[Random.Range(0, m_objectTileArrayOneHeight.Length - 1)];

                    // Set the tile
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    // Remove tile from possible locations list
                    m_possibleTileLocations.Remove(position);
                }

                loopCount--;
            }

            else if (position.y == m_yLocation - 2) // Is Top Wall
            {
                Vector3Int tempPosition;
                Vector3Int tempPosition2;
                tempPosition = new Vector3Int(position.x - 1, position.y, position.z);
                tempPosition2 = new Vector3Int(position.x + 1, position.y, position.z);

                // Check if the tile to the left and right is free
                if (m_possibleTileLocations.Contains(tempPosition) && m_possibleTileLocations.Contains(tempPosition2))
                {
                    // Select a random tile within the object array
                    var chosenTile = m_objectTileArrayThreeWidth[Random.Range(0, m_objectTileArrayThreeWidth.Length - 1)];

                    // Set the tile
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    // Remove tiles from possible locations list
                    m_possibleTileLocations.Remove(position);
                    m_possibleTileLocations.Remove(tempPosition);
                    m_possibleTileLocations.Remove(tempPosition);

                }

                else
                {
                    // Select a random tile within the object array
                    var chosenTile = m_objectTileArrayOneHeight[Random.Range(0, m_objectTileArrayOneHeight.Length - 1)];

                    // Set the tile
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    // Remove tile from possible locations list
                    m_possibleTileLocations.Remove(position);
                }

                loopCount--;
            }

            else
            {
                loopCount++;
            }

            if (m_useTimer) { yield return StartCoroutine(DrawTimer()); }

        }

        yield break;

    }

    private IEnumerator SetRockLocations()
    {
        Debug.Log("Setting Rock locations");

        if (m_useTimer) { yield return StartCoroutine(DrawTimer()); }
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

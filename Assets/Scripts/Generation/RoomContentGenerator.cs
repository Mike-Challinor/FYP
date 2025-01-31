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
    public Tile m_rockTile;
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
    public int m_minNumberOfRocks = 4;
    public int m_maxNumberOfRocks = 6;

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
            if (position.x == m_xLocation + m_width - 1 || position.x == m_xLocation + 1 || position.y == m_yLocation + m_height - 2)
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

        // Remove all door positions
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
        yield return SetAltarLocations();

        // Set the pillars in the scene
        yield return SetPillarLocations();

        // Set the objects in the scene
        yield return SetObjectLocations();

        // Set the rocks in the scene
        yield return SetRockLocations();

        // Wait for timer before finishing drawing
        yield return DrawTimer();

        m_isDrawing = false;

        Debug.Log("Ended drawing room content");

        yield break;

    }

    private IEnumerator SetAltarLocations()
    {
        // Set the amount of wells in the scene between 1 and 4
        int amountToDraw = RandomNumberGenerator(1, 4);

        for (int i = 0; i <= amountToDraw; i++)
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

                else
                {
                    Debug.Log($"Altar {i + 1} position: {position} is not valid.");
                }
            }

            int placeTileOnAltar = RandomNumberGenerator(0, 21);

            // Check whether to spawn item on the altar
            if (placeTileOnAltar > 6)
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
        Debug.Log("Starting SetPillarLocations");
        int numberOfPillars = RandomNumberGenerator(1, 2); // Set the number of pillars to 1 or 2

        for (int i = 0; i < numberOfPillars; i++)
        {
            Debug.Log($"Placing Pillar {i + 1} of {numberOfPillars}");

            // Place the first pillar
            yield return PlaceFirstPillar();

            // Optional timer
            if (m_useTimer)
            {
                Debug.Log("Starting DrawTimer after first pillar");
                yield return DrawTimer();
            }

            // Place the second pillar
            yield return PlaceSecondPillar();

            // Optional timer
            if (m_useTimer)
            {
                Debug.Log("Starting DrawTimer after second pillar");
                yield return DrawTimer();
            }
        }

        Debug.Log("Ending SetPillarLocations");
        yield break;
    }

    private IEnumerator PlaceFirstPillar()
    {
        Debug.Log("Starting PlaceFirstPillar");
        Vector3Int position;
        int xPosition = RandomNumberGenerator(m_xLocation + 1, m_xLocation + m_width - 1);
        int yPosition = m_yLocation + m_height - 2;

        int maxAttempts = 100;
        int attempts = 0;

        // Loop for checking whether position needs to be re-rolled
        while ((m_doorLocations.Any(location => location.x == xPosition) ||
                m_doorLocations.Any(location => location.x + 1 == xPosition) ||
                m_doorLocations.Any(location => location.x + 2 == xPosition) ||
                m_doorLocations.Any(location => location.x + 3 == xPosition) ||
                m_doorLocations.Any(location => location.x - 1 == xPosition) ||
                m_pillarList.Any(location => location.x == xPosition)) &&
                attempts < maxAttempts)
        {
            xPosition = RandomNumberGenerator(m_xLocation + 1, m_xLocation + m_width - 1);
            attempts++;
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogError("Max attempts reached in PlaceFirstPillar while finding xPosition.");
            yield break;
        }

        position = new Vector3Int(xPosition, yPosition, 0);

        if (m_pillarTileArray == null || m_pillarTileArray.Length == 0)
        {
            Debug.LogError("Pillar tile array is null or empty!");
            yield break;
        }

        // Select a random tile within the pillar array
        var chosenTile = m_pillarTileArray[Random.Range(0, m_pillarTileArray.Length)];

        if (m_propTileMapCollision == null)
        {
            Debug.LogError("Tilemap is null!");
            yield break;
        }

        // Set tile
        m_propTileMapCollision.SetTile(position, chosenTile);

        Debug.Log($"Placed first pillar at {position}");

        // Add positions to the list of pillar locations
        m_pillarList.Add(position);

        if (m_possibleTileLocations.Contains(position))
        {
            m_possibleTileLocations.Remove(position);
        }

        Debug.Log("Ending PlaceFirstPillar");
        yield break;
    }

    private IEnumerator PlaceSecondPillar()
    {
        Debug.Log("Starting PlaceSecondPillar");

        if (m_pillarList.Count == 0)
        {
            Debug.LogError("Pillar list is empty!");
            yield break;
        }

        Vector3Int previousPos = m_pillarList[m_pillarList.Count - 1];
        Vector3Int leftWall = new Vector3Int(m_xLocation, previousPos.y, 0);
        Vector3Int rightWall = new Vector3Int(m_xLocation + m_width, previousPos.y, 0);

        int xPosition = Vector3Int.Distance(leftWall, previousPos) < Vector3Int.Distance(rightWall, previousPos)
            ? m_xLocation + m_width - (previousPos.x - leftWall.x)
            : m_xLocation + (rightWall.x - previousPos.x);

        xPosition = Mathf.Clamp(xPosition, m_xLocation + 1, m_xLocation + m_width - 1);
        Vector3Int position = new Vector3Int(xPosition, previousPos.y, 0);

        if (!IsPositionValid(position))
        {
            Debug.LogError($"Position {position} is invalid.");
            yield break;
        }

        if (m_pillarTileArray == null || m_pillarTileArray.Length == 0)
        {
            Debug.LogError("Pillar tile array is null or empty!");
            yield break;
        }

        // Select a random tile within the pillar array
        var chosenTile = m_pillarTileArray[Random.Range(0, m_pillarTileArray.Length)];

        if (m_propTileMapCollision == null)
        {
            Debug.LogError("Tilemap is null!");
            yield break;
        }

        // Set tile
        m_propTileMapCollision.SetTile(position, chosenTile);

        Debug.Log($"Placed second pillar at {position}");

        // Add positions to the list of pillar locations
        m_pillarList.Add(position);

        if (m_possibleTileLocations.Contains(position))
        {
            m_possibleTileLocations.Remove(position);
        }

        Debug.Log("Ending PlaceSecondPillar");
        yield break;
    }

    private IEnumerator SetObjectLocations()
    {
        Debug.Log("Setting object locations");

        // Declare position variable
        Vector3Int position;

        int loopCount = m_maxNumberOfObjects;

        for (int i = 0; i < loopCount;)
        {
            // Break out of the loop if no more objects can be placed
            if (m_possibleTileLocations.Count == 0)
            {
                Debug.LogError("No more possible locations");
                yield break;
            }

            // Get a random position from the possible locations
            position = m_possibleTileLocations[RandomNumberGenerator(0, m_possibleTileLocations.Count - 1)];

            bool placed = false; // Track if an object was placed

            if (position.x == m_xLocation + 1) // Is left wall
            {
                Vector3Int tempPosition = new Vector3Int(position.x, position.y + 1, position.z);

                if (m_possibleTileLocations.Contains(tempPosition))
                {
                    var chosenTile = m_objectTileArrayTwoHeightLeft[Random.Range(0, m_objectTileArrayTwoHeightLeft.Length)];
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    m_possibleTileLocations.Remove(position);
                    m_possibleTileLocations.Remove(tempPosition);

                    placed = true;
                }
                else
                {
                    var chosenTile = m_objectTileArrayOneHeight[Random.Range(0, m_objectTileArrayOneHeight.Length)];
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    m_possibleTileLocations.Remove(position);

                    placed = true;
                }
            }
            else if (position.x == m_xLocation + m_width - 1) // Is right wall
            {
                Vector3Int tempPosition = new Vector3Int(position.x, position.y + 1, position.z);

                if (m_possibleTileLocations.Contains(tempPosition))
                {
                    var chosenTile = m_objectTileArrayTwoHeightRight[Random.Range(0, m_objectTileArrayTwoHeightRight.Length)];
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    m_possibleTileLocations.Remove(position);
                    m_possibleTileLocations.Remove(tempPosition);

                    placed = true;
                }
                else
                {
                    var chosenTile = m_objectTileArrayOneHeight[Random.Range(0, m_objectTileArrayOneHeight.Length)];
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    m_possibleTileLocations.Remove(position);

                    placed = true;
                }
            }
            else if (position.y == m_yLocation + m_height - 2) // Is top wall
            {
                Vector3Int tempPosition = new Vector3Int(position.x - 1, position.y, position.z);
                Vector3Int tempPosition2 = new Vector3Int(position.x + 1, position.y, position.z);

                if (m_possibleTileLocations.Contains(tempPosition) && m_possibleTileLocations.Contains(tempPosition2))
                {
                    var chosenTile = m_objectTileArrayThreeWidth[Random.Range(0, m_objectTileArrayThreeWidth.Length)];
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    m_possibleTileLocations.Remove(position);
                    m_possibleTileLocations.Remove(tempPosition);
                    m_possibleTileLocations.Remove(tempPosition2);

                    placed = true;
                }
                else
                {
                    var chosenTile = m_objectTileArrayOneHeight[Random.Range(0, m_objectTileArrayOneHeight.Length)];
                    m_propTileMapCollision.SetTile(position, chosenTile);

                    m_possibleTileLocations.Remove(position);

                    placed = true;
                }
            }
            else
            {
                Debug.Log($"Position is not top, right, or left wall. Position = {position}");
            }

            // If an object was placed, increment the iteration counter
            if (placed)
            {
                i++; // Only increment if placement succeeded
            }
            else
            {
                Debug.LogWarning("Failed to place an object. Retrying...");
            }

            // Optional timer between placements
            if (m_useTimer)
            {
                yield return StartCoroutine(DrawTimer());
            }
        }

        Debug.Log("Finished placing objects.");
        yield break;
    }

    private IEnumerator SetRockLocations()
    {
        Debug.Log("Setting Rock locations");

        Vector3Int position;

        // Loop through and place rocks
        for (int i = 0; i < m_maxNumberOfRocks; i++)
        {
            int xPosition = RandomNumberGenerator(m_xLocation + 1, m_xLocation + m_width - 1);
            int yPosition = RandomNumberGenerator(m_yLocation + 2, m_yLocation + m_height - 2);

            position = new Vector3Int(xPosition, yPosition, 0);

            while (m_alterList.Contains(position)) // Loop through and make sure it is not the same position as the altar
            {
                xPosition = RandomNumberGenerator(m_xLocation + 1, m_xLocation + m_width - 1);
                yPosition = RandomNumberGenerator(m_yLocation + 2, m_yLocation + m_height - 2);

                position = new Vector3Int(xPosition, yPosition, 0);
            }

            m_propTileMapNoCollision.SetTile(position, m_rockTile);

            if (m_useTimer) { yield return StartCoroutine(DrawTimer()); }
        }
        
    }

    private bool IsPositionValid(Vector3Int position)
    {
        return position.x >= m_xLocation + 1 && position.x <= m_xLocation + m_width - 1
            && position.y >= m_yLocation && position.y < m_yLocation + m_height;
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
        Debug.Log($"Min is: { min } Max is: {max}");
        int rando = Random.Range(min, max + 1);
        Debug.Log($"Result is: { rando }");
        return rando;
    }


}

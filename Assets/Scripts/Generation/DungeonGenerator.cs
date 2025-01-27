using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{

    private List<Room> rooms; // List to store all generated rooms
    [SerializeField] private List<Vector3Int> m_roomLocationsToGenerate; // List to store all of the room locations that still need to be generated
    [SerializeField] private bool isRightPath = true; // Bool for handling if it is a right or left path
    [SerializeField] private GameObject numberPrefab;
    [SerializeField] private bool m_useTimer = true;

    [SerializeField] private int m_numberOfRooms; // The total number of rooms
    [SerializeField] private int m_maxNumberOfRooms = 3; // The max amount of rooms
    [SerializeField] private int m_distanceBetweenRooms = 2; // The distance between the rooms
    [SerializeField] private int m_roomWidth = 19; // The rooms width
    [SerializeField] private int m_roomHeight = 12; // The rooms height

    private const int m_maxNumberOfDoors = 3; // The max amount of doors
    public GameObject m_roomGenerator; // The room generator game object
    public GameObject m_roomContentGenerator; // The room content generator game object
    public RoomGenerator m_roomGeneratorScript; // The room generatior script
    public RoomContentGenerator m_roomContentGeneratorScript; // The room content generatior script

    [SerializeField] private GameObject m_navMesh;
    public NavMeshPlus.Components.NavMeshSurface m_navMeshSurface;

    private bool m_isASideRoom = false;

    private void Awake()
    {
        if (m_navMesh == null)
        {
            Debug.LogError("NavMesh is not assigned in awake!");
            return;
        }

        m_navMeshSurface = m_navMesh.GetComponent<NavMeshPlus.Components.NavMeshSurface>();

        // If no NavMeshSurface script is found, log an error
        if (m_navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface script is missing!");
        }
    }

    private void Start()
    {
        // Initialise room generator script
        m_roomGeneratorScript = m_roomGenerator.GetComponent<RoomGenerator>();

        // Initialise room content generator script
        m_roomContentGeneratorScript = m_roomContentGenerator.GetComponent<RoomContentGenerator>();

        // Initialise Rooms list
        rooms = new List<Room>();

        // Initialise roomLocationsToGenerate list
        m_roomLocationsToGenerate = new List<Vector3Int>();

        // Initialise location of doors in first room
        List<Vector3Int> doorLocations = new List<Vector3Int> { new Vector3Int(9, -4, 0) };

        // Initialise location of walls in first room
        List<Vector3Int> wallLocations = new List<Vector3Int> { new Vector3Int(9, -4, 0) };

        // Initialise location of floor in first room
        List<Vector3Int> floorLocations = new List<Vector3Int> { new Vector3Int(9, -4, 0) };

        // Set for the start room
        AddRoom(19, 12, -10, -8, 1, doorLocations, wallLocations, floorLocations, 0, 0, false);

    }


    // Method to add a new room to the dungeon
    public void AddRoom(int width, int height, int xLocation, int yLocation, int numberOfDoors, List<Vector3Int> doorLocations, List<Vector3Int> wallLocations, List<Vector3Int>floorLocations, Direction entranceDirection, int generationCount, bool isSideRoom)
    {
        Room newRoom = new Room
        {
            width = width,
            height = height,
            xLocation = xLocation,
            yLocation = yLocation,
            numberOfDoors = numberOfDoors,
            doorLocations = doorLocations,
            wallLocations = wallLocations,
            entranceDirection = entranceDirection,
            generationCount = generationCount,
            isSideRoom = isSideRoom
        };

        rooms.Add(newRoom);
        m_numberOfRooms = rooms.Count; // Update the total number of rooms
    }

    public void InsertRoom(int width, int height, int xLocation, int yLocation, int numberOfDoors, List<Vector3Int> doorLocations, List<Vector3Int> wallLocations, List<Vector3Int> floorLocations, Direction entranceDirection, int generationCount, bool isSideRoom)
    {
        Room newRoom = new Room
        {
            width = width,
            height = height,
            xLocation = xLocation,
            yLocation = yLocation,
            numberOfDoors = numberOfDoors,
            doorLocations = doorLocations,
            wallLocations = wallLocations,
            entranceDirection = entranceDirection,
            generationCount = generationCount,
            isSideRoom = isSideRoom
        };

        int secondToLastIndex = rooms.Count - 2; // Second to last position
        rooms.Insert(secondToLastIndex, newRoom);
        m_numberOfRooms = rooms.Count; // Update the total number of rooms
    }

    // Method to get room information
    public Room GetRoom(int index)
    {
        if (index >= 0 && index < rooms.Count)
        {
            return rooms[index];
        }
        return null;
    }

    // Method for calling coroutine from a button
    public void StartGenerateDungeonCoroutine()
    {
        // Ensure the game is running
        if (!Application.isPlaying)
        {
            Debug.LogWarning("The game must be running to generate the dungeon.");
            return;
        }

        StartCoroutine(GenerateDungeonCoroutine());
    }

    // Method to generate the dungeon
    public IEnumerator GenerateDungeonCoroutine()
    {
        if (rooms.Count > 1) // Don't run code if dungeon already generate
        {
            Debug.LogWarning("Dungeon generation already started");
            yield break;
        }

        List<Vector3Int> doorLocations;
        List<Vector3Int> wallLocations;
        List<Vector3Int> floorLocations;

        // Loop through for each room
        for (int i = 0; i < m_maxNumberOfRooms - 1; i++) // Minus one for the first room
        {
            // Initialise lists for this room
            doorLocations = new List<Vector3Int>();
            wallLocations = new List<Vector3Int>();
            floorLocations = new List<Vector3Int>();

            // Initialise starting location for this room
            int xLocation = 0;
            int yLocation = 0;

            // Initilise the entrance direction of the room
            Direction entranceDirection = Direction.None;

            // Set the rooms location
            if (rooms.Count > 0) // Make sure rooms list is not empty
            {
                Room previousRoom = rooms[rooms.Count - 1];

                SetRoomLocation(ref xLocation, ref yLocation, ref entranceDirection, previousRoom);
            }

            else
            {
                HandleNoRooms(ref xLocation, ref yLocation);
            }

            // Initialise room properties
            int roomWidth = m_roomWidth;
            int roomHeight = m_roomHeight;
            int numberOfDoors = 1;
            int maxNumberOfDoorsTemp = m_maxNumberOfDoors;

            // Calculate the possible directions that rooms can be generated
            List<int> availableRoomDirections = GetAvailableRoomDirections(xLocation, yLocation, roomWidth, roomHeight);

            // Set the number of doors for the room
            SetNumberOfDoors(ref numberOfDoors, ref maxNumberOfDoorsTemp, availableRoomDirections);

            // Add entrance door for the room
            if (entranceDirection != Direction.None)
            {
                // Add entrance door based on direction
                switch (entranceDirection)
                {
                    case Direction.North:
                        doorLocations.Add(new Vector3Int(xLocation + roomWidth / 2, yLocation, 0)); // Bottom of room
                        break;
                    case Direction.South:
                        doorLocations.Add(new Vector3Int(xLocation + roomWidth / 2, yLocation + roomHeight, 0)); // Top of room
                        break;
                    case Direction.East:
                        doorLocations.Add(new Vector3Int(xLocation, yLocation + roomHeight / 2, 0)); // Left of room
                        break;
                    case Direction.West:
                        doorLocations.Add(new Vector3Int(xLocation + roomWidth, yLocation + roomHeight / 2, 0)); // Right of room
                        break;
                }
            }

            else // This is the location of the door from the first room
            {
                doorLocations.Add(new Vector3Int(xLocation, yLocation + roomHeight / 2, 0)); // Left of room
            }

            // Set additional doors
            if (numberOfDoors > 1) // Check to make sure is not the last room
            {
                if (rooms.Count != 0)
                {
                    if (availableRoomDirections.Count != 0)
                    {
                        SetAdditionalDoors(numberOfDoors, availableRoomDirections, xLocation, yLocation, roomWidth, roomHeight, ref doorLocations);
                    }

                    else
                    {
                        Debug.Log("Unable to place additional doors as available room directions equals 0");
                    }
                }

                else
                {
                    Debug.Log("Unable to place additional doors as room count is 0");
                }

            }

            else
            {
                Debug.Log("Unable to place additional doors as number of doors is NOT greater than 1");
            }


            // Add walls
            SetWallsAndFloor(roomHeight, roomWidth, xLocation, yLocation, doorLocations, ref wallLocations, ref floorLocations);

            //Add or insert room to the list
            if (m_isASideRoom)
            {
                // Insert room as second to last item in list
                InsertRoom(roomWidth, roomHeight, xLocation, yLocation, numberOfDoors, doorLocations, wallLocations, floorLocations, entranceDirection, rooms.Count, true);
            }

            else
            {
                // Add room to the list
                AddRoom(roomWidth, roomHeight, xLocation, yLocation, numberOfDoors, doorLocations, wallLocations, floorLocations, entranceDirection, rooms.Count, false);
            }

            // Generate the room visually with the room generator script
            GenerateRoom(roomWidth, roomHeight, xLocation, yLocation, numberOfDoors, doorLocations, wallLocations, floorLocations);

            // Wait until room drawing is complete
            while (m_roomGeneratorScript.m_isDrawing)
            {
                yield return null;
            }

            // Spawn a number to represent the room generation order
            if (numberPrefab != null) // Make sure that the prefab is not null
            {
                Vector3 numberLoc = new Vector3(xLocation + roomWidth / 2, yLocation + roomHeight / 2, 0);
                Quaternion spawnRotation = Quaternion.identity;
                GameObject spawnedNumber = Instantiate(numberPrefab, numberLoc, spawnRotation);
                spawnedNumber.GetComponent<RoomNumberAssignment>().SetSprite(m_numberOfRooms - 1);
            }

            // GENERATE ROOM CONTENT
            GenerateRoomContent(roomWidth, roomHeight, xLocation, yLocation, doorLocations, wallLocations, floorLocations, m_isASideRoom);
            m_isASideRoom = false;
        }

        // Rebake nav mesh
        m_navMeshSurface.BuildNavMesh();
    }

    private void SetRoomLocation(ref int xLocation, ref int yLocation, ref Direction entranceDirection, Room previousRoom)
    {
        if (rooms.Count == 1) // First generated room
        {
            SetFirstRoomLocation(ref xLocation, ref yLocation, ref entranceDirection);
        }
        else if (m_roomLocationsToGenerate.Count > 0) // Subsequent rooms
        {
            SetNextRoomLocation(ref xLocation, ref yLocation, ref entranceDirection, previousRoom);
        }
        else
        {
            HandleNoRooms(ref xLocation, ref yLocation);
        }
    }

    private void SetFirstRoomLocation(ref int xLoc, ref int yLoc, ref Direction entranceDirection)
    {
        Room firstRoom = rooms[0];
        xLoc = firstRoom.xLocation + firstRoom.width + m_distanceBetweenRooms;
        yLoc = firstRoom.yLocation;
        entranceDirection = Direction.East;
    }

    private void SetNextRoomLocation(ref int xLoc, ref int yLoc, ref Direction entranceDirection, Room previousRoom)
    {
        // Set the location based on the next room to generate
        xLoc = m_roomLocationsToGenerate[0].x;
        yLoc = m_roomLocationsToGenerate[0].y;

        // Remove the room from the to-generate list
        m_roomLocationsToGenerate.RemoveAt(0);

        // Determine the entrance direction
        entranceDirection = GetEntranceDirection(previousRoom, xLoc, yLoc);
    }

    private void HandleNoRooms(ref int xLoc, ref int yLoc)
    {
        Debug.Log("No rooms available!");
        xLoc = 5000;
        yLoc = -8000;
    }

    private void SetNumberOfDoors(ref int numberOfDoors, ref int maxNumberOfDoorsTemp, List<int> availableRoomDirections)
    {
        SetMaxDoorsTemp(ref maxNumberOfDoorsTemp, availableRoomDirections);

        if (rooms.Count != 0) // Ensure rooms list is not empty
        {
            Room previousRoom = rooms[rooms.Count - 1];
            int roomNo = rooms.Count;

            Debug.Log($"Room number is: " + roomNo);
            Debug.Log($"Previous room is: " + previousRoom.generationCount);

            // If it is the last room
            if (rooms.Count == m_maxNumberOfRooms - 1) 
            {
                Debug.Log("This is the last room");
                numberOfDoors = 1; // For the entrance door
            }

            // If there are more than one queued up rooms then the first room will be a side room
            else if (m_roomLocationsToGenerate.Count >= 2) 
            {
                Debug.Log("This is the first queued up room... Setting as side room");
                numberOfDoors = 1;
                m_isASideRoom = true;
            }

            // If the previous room has 3 doors
            else if (previousRoom.numberOfDoors == 3) 
            {

                if (m_maxNumberOfRooms - rooms.Count >= 1) // If more rooms are going to be needed
                {

                    if (m_roomLocationsToGenerate.Count >= 1)
                    {
                        numberOfDoors = 1;
                        m_isASideRoom = true;
                    }

                    else
                    {
                        numberOfDoors = 2;
                    }

                }

                else // Set to only 1 door for side room
                {
                    numberOfDoors = 1; // For the entrance door
                    m_isASideRoom = true;
                }

            }

            else if (rooms.Count == m_maxNumberOfRooms - 2) // If it is second to last room
            {
                if (m_roomLocationsToGenerate.Count >= 1)
                {
                    numberOfDoors = 1; // For the entrance door
                    m_isASideRoom = true;
                }

                else
                {
                    numberOfDoors = 2; // For the entrance door
                }
            }

            else // Max number of doors can be positioned
            {
                numberOfDoors = Random.Range(2, maxNumberOfDoorsTemp + 1);
                if (numberOfDoors == 0) { numberOfDoors = 1; }
                Debug.Log($"maxNumberofDoorsTemp = " + maxNumberOfDoorsTemp);

                if (m_maxNumberOfRooms - rooms.Count == 2)
                {
                    numberOfDoors = 2;
                }
            }

            Debug.Log($"Number of doors = " + numberOfDoors);
        }

        else
        {
            Debug.Log("Room count is empty !!");
            numberOfDoors = 1;
        }
    }

    void SetMaxDoorsTemp(ref int maxDoorsTemp, List<int> availableRoomDirections)
    {
        if (availableRoomDirections.Count != 0) // Ensure available Room Directions list is not empty
        {
            // Set the maximum number of doors to the available amount of room directions
            if (m_maxNumberOfDoors > availableRoomDirections.Count)
            {
                // This is the maximum amount of doors based off of the available directions
                maxDoorsTemp = availableRoomDirections.Count + 1; // + 1 to include the entrance door
            }
        }

        // Check to make sure that doors are not added when there is not enough rooms available to be added
        if (m_maxNumberOfRooms - rooms.Count < maxDoorsTemp)
        {
            Debug.Log($"Max number of rooms equals: " + m_maxNumberOfRooms);
            Debug.Log($"Room count equals: " + rooms.Count);

            maxDoorsTemp = m_maxNumberOfRooms - rooms.Count - 1;

            Debug.Log($"maxNumberOfDoorsTemp equals: " + maxDoorsTemp);
        }
    }

    void SetAdditionalDoors(int numberOfDoors, List<int> availableRoomDirections, int xLocation, int yLocation, int roomWidth, int roomHeight, ref List<Vector3Int> doorLocations)
    {
        // Place additional doors on specific walls
        for (int d = 0; d < numberOfDoors - 1; d++) // -1 because one door is for the entrance
        {
            int wall;
            Vector3Int doorPosition = Vector3Int.zero; // Reset the doorPosition to zero

            if (availableRoomDirections.Count > 0) // Check if the list is not empty
            {
                // Randomly select a wall: 0 = top, 1 = bottom, 2 = left, 3 = right
                wall = GetRandomWall(availableRoomDirections);
            }

            else
            {
                Debug.Log("No available directions to choose from.");
                wall = 5; // If no wall can be chosen
            }

            // Set the door location based off of the chosen wall
            SetDoorLocation(wall, xLocation, yLocation, roomWidth, roomHeight, ref doorLocations, ref doorPosition);

            // Ensure no duplicate doors
            if (!doorLocations.Contains(doorPosition))
            {
                doorLocations.Add(doorPosition);
            }
            else
            {
                d--; // Retry if door already exists
            }

        }
    }

    private int GetRandomWall(List<int> availableRoomDirections)
    {
        // Get a random index from the list
        int randomIndex = Random.Range(0, availableRoomDirections.Count);

        // Get the random number at that index
        int wall = availableRoomDirections[randomIndex];

        if (rooms.Count == 1)
        {
            // Loop to make sure door doesn't spawn on left in first generated room
            while (wall == 2)
            {
                // Get a random index from the list
                randomIndex = Random.Range(0, availableRoomDirections.Count);

                // Get the random number at that index
                wall = availableRoomDirections[randomIndex];
            }
        }

        return wall;
    }

    private void SetDoorLocation(int wall, int xLocation, int yLocation, int roomWidth, int roomHeight, ref List<Vector3Int> doorLocations, ref Vector3Int doorPosition)
    {
        Vector3Int roomPosition; // Room position for next room based off of doors location

        switch (wall)
        {
            case 0: // Top wall

                doorPosition = new Vector3Int(xLocation + roomWidth / 2, yLocation + roomHeight, 0); // Middle of the top wall
                roomPosition = new Vector3Int(xLocation, yLocation + roomHeight + m_distanceBetweenRooms, 0);

                if (!m_roomLocationsToGenerate.Contains(roomPosition))
                {
                    m_roomLocationsToGenerate.Add(roomPosition);
                    Debug.Log($"Added new room position: {roomPosition}");
                }
                else
                {
                    Debug.LogWarning($"Duplicate room position attempted: {roomPosition}");
                }
                break;

            case 1: // Bottom wall

                doorPosition = new Vector3Int(xLocation + roomWidth / 2, yLocation, 0); // Middle of the bottom wall
                roomPosition = new Vector3Int(xLocation, yLocation - roomHeight - m_distanceBetweenRooms, 0);

                if (!m_roomLocationsToGenerate.Contains(roomPosition))
                {
                    m_roomLocationsToGenerate.Add(roomPosition);
                    Debug.Log($"Added new room position: {roomPosition}");
                }
                else
                {
                    Debug.LogWarning($"Duplicate room position attempted: {roomPosition}");
                }
                break;

            case 2: // Left wall

                doorPosition = new Vector3Int(xLocation, yLocation + roomHeight / 2, 0); // Middle of the left wall
                roomPosition = new Vector3Int(xLocation - roomWidth - m_distanceBetweenRooms, yLocation, 0);

                if (!m_roomLocationsToGenerate.Contains(roomPosition))
                {
                    m_roomLocationsToGenerate.Add(roomPosition);
                    Debug.Log($"Added new room position: {roomPosition}");
                }
                else
                {
                    Debug.LogWarning($"Duplicate room position attempted: {roomPosition}");
                }
                break;

            case 3: // Right wall

                doorPosition = new Vector3Int(xLocation + roomWidth, yLocation + roomHeight / 2, 0); // Middle of the right wall
                roomPosition = new Vector3Int(xLocation + roomWidth + m_distanceBetweenRooms, yLocation, 0);

                if (!m_roomLocationsToGenerate.Contains(roomPosition))
                {
                    m_roomLocationsToGenerate.Add(roomPosition);
                    Debug.Log($"Added new room position: {roomPosition}");
                }
                else
                {
                    Debug.LogWarning($"Duplicate room position attempted: {roomPosition}");
                }
                break;
        }
    }

    void SetWallsAndFloor(int roomHeight, int roomWidth, int xLocation, int yLocation, List<Vector3Int> doorLocations, ref List<Vector3Int> wallLocations, ref List<Vector3Int> floorLocations)
    {
        for (int j = 0; j <= roomHeight; j++) // Height
        {
            for (int k = 0; k <= roomWidth; k++) // Width
            {
                int wallX = xLocation + k;
                int wallY = yLocation + j;

                if (j == 0 || j == roomHeight || k == 0 || k == roomWidth) // Edges
                {
                    if (!doorLocations.Contains(new Vector3Int(wallX, wallY, 0)))
                    {
                        wallLocations.Add(new Vector3Int(wallX, wallY, 0));
                    }
                }

                else
                {
                    if (j > 1 && j < roomHeight - 1) // Set floor position if height position is greater than one as walls are two high
                    {
                        floorLocations.Add(new Vector3Int(wallX, wallY, 0));
                    }
                }    
            }
        }
    }

    void GenerateRoom(int roomWidth, int roomHeight, int xLocation, int yLocation, int numberOfDoors, List<Vector3Int> doorLocations, List<Vector3Int> wallLocations, List<Vector3Int> floorLocations)
    {
        // Generate the room visually
        m_roomGeneratorScript.SetUseTimer(m_useTimer); // Set whether to use a timer when drawing on the room generator
        m_roomGeneratorScript.SetDistanceBetweenRooms(m_distanceBetweenRooms); // Set the distance between the rooms on the room generator
        m_roomGeneratorScript.GenerateRoom(roomWidth, roomHeight, xLocation, yLocation, numberOfDoors, doorLocations, wallLocations, floorLocations); // Generate the room
    }
    void GenerateRoomContent(int roomWidth, int roomHeight, int xLocation, int yLocation, List<Vector3Int> doorLocations, List<Vector3Int> wallLocations, List<Vector3Int> floorLocations, bool isSideRoom)
    {
        // Generate the room visually
        m_roomContentGeneratorScript.SetUseTimer(m_useTimer); // Set whether to use a timer when drawing on the room content generator
        m_roomContentGeneratorScript.GenerateRoom(roomWidth, roomHeight, xLocation, yLocation, doorLocations, wallLocations, floorLocations, isSideRoom); // Generate the room content
    }

    Direction GetEntranceDirection(Room previousRoom, int currentX, int currentY)
    {
        if (currentX > previousRoom.xLocation) return Direction.East;
        if (currentX < previousRoom.xLocation) return Direction.West;
        if (currentY > previousRoom.yLocation) return Direction.North;
        if (currentY < previousRoom.yLocation) return Direction.South;
        return Direction.None; // For cases like the first room
    }

    private List<int> GetAvailableRoomDirections(int xPos, int yPos, int width, int height)
    {
        HashSet<int> availableDirections = new HashSet<int> { 0, 1, 2, 3 }; // Initially, all directions are valid.

        foreach (Room room in rooms)
        {
            if (xPos == room.xLocation && yPos + height + m_distanceBetweenRooms == room.yLocation) availableDirections.Remove(0); // Top
            if (xPos == room.xLocation && yPos - height - m_distanceBetweenRooms == room.yLocation) availableDirections.Remove(1); // Bottom
            if (xPos - width - m_distanceBetweenRooms == room.xLocation && yPos == room.yLocation) availableDirections.Remove(2); // Left
            if (xPos + width + m_distanceBetweenRooms == room.xLocation && yPos == room.yLocation) availableDirections.Remove(3); // Right
        }

        if (isRightPath) // Remove the possibility of going left
        {
            availableDirections.Remove(2);
        }

        else // Remove the possibility of going left
        {
            availableDirections.Remove(3);
        }

        Debug.Log($"Available directions: {string.Join(", ", availableDirections)} at {xPos}, {yPos}");
        return new List<int>(availableDirections);
    }

    public int GetDistanceBetweenRooms()
    {
        return m_distanceBetweenRooms;
    }

}



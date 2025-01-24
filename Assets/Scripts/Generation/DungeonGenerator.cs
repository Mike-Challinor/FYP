using UnityEngine;
using Unity.AI.Navigation;
using System.Collections;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{

    private List<Room> rooms; // List to store all generated rooms
    [SerializeField] private List<Vector3Int> roomLocationsToGenerate; // List to store all of the room locations that still need to be generated
    [SerializeField] private bool isRightPath = true; // Bool for handling if it is a right or left path
    [SerializeField] private GameObject numberPrefab;
    [SerializeField] private bool m_useTimer = true;

    [SerializeField] private int m_numberOfRooms; // The total number of rooms
    [SerializeField] private int m_maxNumberOfRooms = 3; // The max amount of rooms
    [SerializeField] private int m_distanceBetweenRooms = 2; // The distance between the rooms

    private const int m_maxNumberOfDoors = 3; // The max amount of doors
    public GameObject m_roomGenerator; // The room generator game object
    public RoomGenerator m_roomGeneratorScript; // The room generatior script

    [SerializeField] private GameObject m_navMesh;
    public NavMeshSurface m_navMeshSurface;

    private bool m_isASideRoom = false;

    private void Awake()
    {
        if (m_navMesh == null)
        {
            Debug.LogError("NavMesh is not assigned in awake!");
            return;
        }

        m_navMeshSurface = m_navMesh.GetComponent<NavMeshSurface>();

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

        // Initialise Rooms list
        rooms = new List<Room>();

        // Initialise roomLocationsToGenerate list
        roomLocationsToGenerate = new List<Vector3Int>();

        // Initialise location of doors in first room
        List<Vector3Int> doorLocations = new List<Vector3Int> { new Vector3Int(9, -4, 0) };

        // Initialise location of walls in first room
        List<Vector3Int> wallLocations = new List<Vector3Int> { new Vector3Int(9, -4, 0) };

        // Set for the start room
        AddRoom(19, 12, -10, -8, 1, doorLocations, wallLocations, 0, 0, false);

    }


    // Method to add a new room to the dungeon
    public void AddRoom(int width, int height, int xLocation, int yLocation, int numberOfDoors, List<Vector3Int> doorLocations, List<Vector3Int> wallLocations, Direction entranceDirection, int generationCount, bool isSideRoom)
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

    public void InsertRoom(int width, int height, int xLocation, int yLocation, int numberOfDoors, List<Vector3Int> doorLocations, List<Vector3Int> wallLocations, Direction entranceDirection, int generationCount, bool isSideRoom)
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
        List<Vector3Int> doorLocations;
        List<Vector3Int> wallLocations;

        // Loop through for each room
        for (int i = 0; i < m_maxNumberOfRooms - 1; i++) // Minus one for the first room
        {
            // Initialise lists for this room
            doorLocations = new List<Vector3Int>();
            wallLocations = new List<Vector3Int>();

            // Initialise starting location for this room
            int xLocation;
            int yLocation;

            // Initilise the entrance direction of the room
            Direction entranceDirection = Direction.None;

            if (rooms.Count != 0) // Make sure rooms list is not empty
            {
                Room previousRoom = rooms[rooms.Count - 1];

                if (rooms.Count == 1) // Location for the first generated room
                {
                    xLocation = rooms[0].xLocation + rooms[0].width + m_distanceBetweenRooms;
                    yLocation = rooms[0].yLocation;
                    entranceDirection = Direction.East;
                }

                else // Set all other of the rooms location
                {
                    if (roomLocationsToGenerate.Count != 0)
                    {
                        // Set rooms location based on previous rooms door locations
                        xLocation = roomLocationsToGenerate[0].x;
                        yLocation = roomLocationsToGenerate[0].y;

                        // Remove the room from the ToGenerate list
                        roomLocationsToGenerate.RemoveAt(0);

                        entranceDirection = GetEntranceDirection(previousRoom, xLocation, yLocation);
                    }

                    else
                    {
                        Debug.Log("Rooms to generate == 0 !!!");
                        xLocation = 5000;
                        yLocation = -8000;
                    }
                    
                }
            }

            else
            {
                Debug.Log("Room count == 0 !!!");
                xLocation = 5000;
                yLocation = -8000;
            }

            // Room properties
            int roomWidth = 19;
            int roomHeight = 12;
            int numberOfDoors = 1;
            int maxNumberOfDoorsTemp = m_maxNumberOfDoors;

            // Calculate the possible directions that rooms can be generated
            List<int> availableRoomDirections = GetAvailableRoomDirections(xLocation, yLocation, roomWidth, roomHeight);

            if (availableRoomDirections.Count != 0) // Ensure available Room Directions list is not empty
            {
                // Set the maximum number of doors to the available amount of room directions
                if (m_maxNumberOfDoors > availableRoomDirections.Count)
                {
                    // This is the maximum amount of doors based off of the available directions
                    maxNumberOfDoorsTemp = availableRoomDirections.Count + 1; // + 1 to include the entrance door
                }
            }

            // Check to make sure that doors are not added when there is not enough rooms available to be added
            if (m_maxNumberOfRooms - rooms.Count < maxNumberOfDoorsTemp)
            {
                Debug.Log($"Max number of rooms equals: " + m_maxNumberOfRooms);
                Debug.Log($"Room count equals: " + rooms.Count);

                maxNumberOfDoorsTemp = m_maxNumberOfRooms - rooms.Count - 1;

                Debug.Log($"maxNumberOfDoorsTemp equals: " + maxNumberOfDoorsTemp);
            }

            if (rooms.Count != 0) // Ensure rooms list is not empty
            {
                Room previousRoom = rooms[rooms.Count - 1];

                int roomNo = rooms.Count;
                Debug.Log($"Room number is: " + roomNo);
                Debug.Log($"Previous room is: " + previousRoom.generationCount);

                if (rooms.Count == m_maxNumberOfRooms - 1) // If it is the last room
                {
                    Debug.Log("This is the last room");
                    numberOfDoors = 1; // For the entrance door
                }

                else if (roomLocationsToGenerate.Count >= 2) // If there are more than one queued up rooms then the first room will be a side room
                {
                    Debug.Log("This is the first queued up room... Setting as side room");
                    numberOfDoors = 1;
                    m_isASideRoom = true;
                }

                else if (previousRoom.numberOfDoors == 3)
                {
                    Debug.Log("Nahsadjh");
                    Debug.Log($"Room locations to generate = " + roomLocationsToGenerate.Count);

                    if (m_maxNumberOfRooms - rooms.Count == 1) // If more rooms are going to be needed
                    {
                        if (roomLocationsToGenerate.Count >= 1)
                        {
                            Debug.Log("Yeehaw");
                            numberOfDoors = 1;
                            m_isASideRoom = true;
                        }

                        else
                        {
                            Debug.Log("Yeeho");
                            numberOfDoors = 2;
                        }
                        
                    }

                    else if (m_maxNumberOfRooms - rooms.Count == 2)
                    {
                        if (roomLocationsToGenerate.Count >= 1)
                        {
                            Debug.Log("Cucumber");
                            numberOfDoors = 1; // For the entrance door
                            m_isASideRoom = true;
                        }

                        else
                        {
                            Debug.Log("Ploop");
                            numberOfDoors = 2; // For the entrance door
                        }

                    }

                    else if (m_maxNumberOfRooms - rooms.Count == 3)
                    {
                        if (roomLocationsToGenerate.Count == 1)
                        {
                            Debug.Log("Cabbage");
                            m_isASideRoom = true;
                            numberOfDoors = 1; // For the entrance door
                        }
                        else if (roomLocationsToGenerate.Count == 2)
                        {
                            Debug.Log("Beef");
                            numberOfDoors = 1; // For the entrance door
                            m_isASideRoom = true;
                        }
                        else
                        {
                            Debug.Log("Eggs");
                            numberOfDoors = 2; // For the entrance door
                        }

                    }

                    else if (m_maxNumberOfRooms - rooms.Count > 3)
                    {
                        if (roomLocationsToGenerate.Count >= 1)
                        {
                            Debug.Log("Potato");
                            m_isASideRoom = true;
                            numberOfDoors = 1; // For the entrance door
                        }

                        else
                        {
                            Debug.Log("Bacon");
                            numberOfDoors = 2; // For the entrance door
                        }

                    }

                    else // Set to only 1 door for side room
                    {
                        Debug.Log("Bloohoa");
                        numberOfDoors = 1; // For the entrance door
                        m_isASideRoom = true;
                    }
                    
                }

                else if (rooms.Count == m_maxNumberOfRooms - 2) // If it is second to last room
                {
                    Debug.Log("Nooookay");
                    numberOfDoors = 2; // For the entrance door

                    if (roomLocationsToGenerate.Count == 0)
                    {
                        Debug.Log("mi");
                        numberOfDoors = 2; // For the entrance door
                    }

                    else if (roomLocationsToGenerate.Count == 1)
                    {
                        Debug.Log("Naahhh");
                        numberOfDoors = 1; // For the entrance door
                        m_isASideRoom = true;
                    }

                    else if (roomLocationsToGenerate.Count == 2)
                    {
                        Debug.Log("ti");
                        numberOfDoors = 1; // For the entrance door
                        m_isASideRoom = true;
                    }
                    else
                    {
                        Debug.Log("si");
                        numberOfDoors = 2; // For the entrance door
                    }
                }

                // Calculate the amount of doors that can and will be placed
                else if (m_maxNumberOfRooms - rooms.Count < maxNumberOfDoorsTemp) // If the amount of rooms left is less than the max number of possible doors
                {
                    Debug.Log("cmon now");
                    if (m_maxNumberOfRooms - rooms.Count == 2) // Chack to make sure that the random range is valid
                    {
                        if (roomLocationsToGenerate.Count <= 1)
                        {
                            Debug.Log("Kah");
                            numberOfDoors = 1; // For the entrance door
                            m_isASideRoom = true;
                        }
                        else if (roomLocationsToGenerate.Count == 2)
                        {
                            Debug.Log("Mei");
                            numberOfDoors = 1; // For the entrance door
                            m_isASideRoom = true;
                        }
                        else
                        {
                            Debug.Log("Haaa");
                            numberOfDoors = 2; // For the entrance door
                        }
                    }

                    else
                    {
                        numberOfDoors = Random.Range(2, m_maxNumberOfRooms - rooms.Count + 1);
                    }
                }

                else // Max number of doors can be positioned
                {
                    numberOfDoors = Random.Range(2, maxNumberOfDoorsTemp + 1);
                    if (numberOfDoors == 0) { numberOfDoors = 1; }
                    Debug.Log($"maxNumberofDoorsTemp = " + maxNumberOfDoorsTemp);

                    if (m_maxNumberOfRooms - rooms.Count == 2)
                    {
                        Debug.Log($"plz help me");
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

            if (numberOfDoors > 1) // Check to make sure is not the last room
            {
                if (rooms.Count != 0)
                {
                    if (availableRoomDirections.Count != 0)
                    {
                        // Place additional doors on specific walls
                        for (int d = 0; d < numberOfDoors - 1; d++) // -1 because one door is for the entrance
                        {
                            int wall;

                            // Randomly select a wall: 0 = top, 1 = bottom, 2 = left, 3 = right
                            if (availableRoomDirections.Count > 0) // Check if the list is not empty
                            {
                                // Get a random index from the list
                                int randomIndex = Random.Range(0, availableRoomDirections.Count);

                                // Get the random number at that index
                                wall = availableRoomDirections[randomIndex];

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

                                Debug.Log("Random Direction: " + wall);
                            }

                            else
                            {
                                Debug.Log("No available directions to choose from.");
                                wall = 5; // If no wall can be chosen
                            }


                            Vector3Int doorPosition = Vector3Int.zero; // Reset the doorPosition to zero
                            Vector3Int roomPosition;

                            switch (wall)
                            {
                                case 0: // Top wall

                                    doorPosition = new Vector3Int(xLocation + roomWidth / 2, yLocation + roomHeight, 0); // Middle of the top wall
                                    roomPosition = new Vector3Int(xLocation, yLocation + roomHeight + m_distanceBetweenRooms, 0);

                                    if (!roomLocationsToGenerate.Contains(roomPosition))
                                    {
                                        roomLocationsToGenerate.Add(roomPosition);
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

                                    if (!roomLocationsToGenerate.Contains(roomPosition))
                                    {
                                        roomLocationsToGenerate.Add(roomPosition);
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

                                    if (!roomLocationsToGenerate.Contains(roomPosition))
                                    {
                                        roomLocationsToGenerate.Add(roomPosition);
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

                                    if (!roomLocationsToGenerate.Contains(roomPosition))
                                    {
                                        roomLocationsToGenerate.Add(roomPosition);
                                        Debug.Log($"Added new room position: {roomPosition}");
                                    }
                                    else
                                    {
                                        Debug.LogWarning($"Duplicate room position attempted: {roomPosition}");
                                    }
                                    break;
                            }

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
            for (int j = 0; j <= roomHeight; j++)
            {
                for (int k = 0; k <= roomWidth; k++)
                {
                    if (j == 0 || j == roomHeight || k == 0 || k == roomWidth) // Edges
                    {
                        int wallX = xLocation + k;
                        int wallY = yLocation + j;

                        if (!doorLocations.Contains(new Vector3Int(wallX, wallY, 0)))
                        {
                            wallLocations.Add(new Vector3Int(wallX, wallY, 0));
                        }
                    }
                }
            }

            if (m_isASideRoom)
            {
                // Insert room as second to last item in list
                InsertRoom(roomWidth, roomHeight, xLocation, yLocation, numberOfDoors, doorLocations, wallLocations, entranceDirection, rooms.Count, true);
                m_isASideRoom = false;
            }

            else
            {
                // Add room to the list
                AddRoom(roomWidth, roomHeight, xLocation, yLocation, numberOfDoors, doorLocations, wallLocations, entranceDirection, rooms.Count, false);
            }


            // Generate the room visually
            m_roomGeneratorScript.SetUseTimer(m_useTimer); // Set whether to use a timer when drawing on the room generator
            m_roomGeneratorScript.SetDistanceBetweenRooms(m_distanceBetweenRooms); // Set the distance between the rooms on the room generator
            m_roomGeneratorScript.GenerateRoom(roomWidth, roomHeight, xLocation, yLocation, numberOfDoors, doorLocations, wallLocations); // Generate the room

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

        }

        // Rebake nav mesh
        m_navMeshSurface.BuildNavMesh();
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
            availableDirections.Remove(2); // Left
        }

        else // Remove the possibility of going left
        {
            availableDirections.Remove(3); // Right
        }

        Debug.Log($"Available directions: {string.Join(", ", availableDirections)} at {xPos}, {yPos}");
        return new List<int>(availableDirections);
    }

    public int GetDistanceBetweenRooms()
    {
        return m_distanceBetweenRooms;
    }

}



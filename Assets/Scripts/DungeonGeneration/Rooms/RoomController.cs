using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

//room info
public class RoomInfo
{
    public string name;

    public int x;

    public int y;

    public GameObject roomPrefab;
}

public class RoomController : MonoBehaviour
{
    //.This
    public static RoomController instance;

    public bool EndRoomLast;

    //the name of this floor/world
    string currentWorldName = "Basement";

    //Data of this room (name, X, Y)
    RoomInfo currentLoadRoomData;

    public Room currRoom;

    //A queue is a FIFO (first in, first out) structure.
    //This will be useful when loading our scenes as we
    //want them to load in order. First instance = first loaded room
    Queue<RoomInfo> loadRoomQueue = new Queue<RoomInfo>();

    //List of the loaded room
    public List<Room> loadedRooms = new List<Room>();

    public List<GameObject> roomPrefab = new List<GameObject>();

    //is this room loading
    bool isLoadingRoom = false;

    bool spawningBossRoom = false;
    bool bossRoomSpawned= false;
    public bool doorRemoved = false;

    private void Awake()
    {
        //set instance to this
        instance = this;
    }

    private void Start()
    {
        // LoadRoom("Start", 0, 0);
        // LoadRoom("Empty", 1, 0);
        // LoadRoom("Empty", 0, 1);
        // LoadRoom("Empty", -1, 0);
        // LoadRoom("Empty", 0, -1);
    }

    private void Update()
    {
        //Constantly check for room to load
        UpdateRoomQueue();
    }

    void UpdateRoomQueue()
    {
        //If there is a room currently loading, do nothing
        if(isLoadingRoom)
        {
            return;
        }

        //If there is no more room to load, do nothing
        if(loadRoomQueue.Count == 0)
        {
            if(!bossRoomSpawned)
            {
                StartCoroutine(SpawnBossRoom());
            }
            else if(bossRoomSpawned && !doorRemoved)
            {
                foreach(Room room in loadedRooms)
                {
                    //Gives each door a random pos on their wall and pair with the adjacent door from the adjacent room
                    room.MakeDoorPair();

                    //remove unconnected doors
                    room.RemoveUnconnectedDoors();
                }

                doorRemoved = true;

                // foreach(Room room in loadedRooms)
                // {
                //     room.CreateDoorPath();
                // }

                //Remove excessive scenes from the hierarchy
                //Debug.Log("Number of scene = "+SceneManager.sceneCount);
                StartCoroutine(RemoveExcessiveScenes());
            }

            return;
        }

        //Then lets load this room!

        //Remove it from the queue
        currentLoadRoomData = loadRoomQueue.Dequeue();
        //tell us that this room is loading
        isLoadingRoom = true;
        //start the loading routine
        StartCoroutine(LoadRoomRoutine(currentLoadRoomData));
    }

    //Loads this scene (room)
    public void LoadRoom(string name, int x, int y)
    {
        //If this room already exist, do not load again
        if(DoesRoomExist(x,y))
        {
            //return nothing
            return;
        }

        //create a RoomInfo to hold room data
        RoomInfo newRoomData = new RoomInfo();

        //set data
        newRoomData.name = name;
        newRoomData.x = x;
        newRoomData.y = y;


        //put this room in the loading queue
        loadRoomQueue.Enqueue(newRoomData);
    }

    //This is a routine to make sure each scene load 1 by 1 without overlap
    IEnumerator LoadRoomRoutine(RoomInfo info)
    {
        //The name of a room is the worldname + their coordonate
        string roomName = currentWorldName + info.name;

        //Each room is a unique scene and we want them to load in a "umbrella scene" so the player can move
        //between them like if the were side by side (thus we load them as additive)
        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);

        //Loop until loadRoom operation is done
        while(loadRoom.isDone ==false)
        {
            //return nothing once per frame
            yield return null;
        }
    }

    //Spawn a Boss Room
    IEnumerator SpawnBossRoom()
    {
        //Wait 0.5f to make sure we are not loading rooms anymore
        yield return new WaitForSeconds(0.5f);
        if(loadRoomQueue.Count==0 && !spawningBossRoom)
        {
            //Makes sure that this method is called only once by making this bool true
            spawningBossRoom = true;

            //if checked in roomControl editor, makes the last room the boss room
            if(EndRoomLast)
                LoadBossRoomLast();
            //Else we will find the furthest room spawned and make it the boos room
            else
                LoadBossRoomFurthest();
        }
    }

    public void LoadBossRoomLast()
    {
        //Index of the last empty room spawned
        int lastEmptyRoomIndex = loadedRooms.Count-1;

        //Create a room(var) with the last empty room spawned coordonate
        var roomToRemove = loadedRooms.Single(r=>
            r.x == loadedRooms[lastEmptyRoomIndex].x &&
            r.y == loadedRooms[lastEmptyRoomIndex].y );

        // Room bossRoom = loadedRooms[lastEmptyRoomIndex];
        // Vector2Int tempRoom = new Vector2Int(bossRoom.x, bossRoom.y);
        // Destroy(bossRoom.gameObject);
        // var roomToRemove = loadedRooms.Single(r => r.x == bossRoom.x && r.y == bossRoom.y);

        //Remove this room the loadedroom list
        loadedRooms.Remove(roomToRemove);

        //Destroy the room gameOnject
        Destroy(roomToRemove.gameObject);

        //Load the new Boss room in its place
        LoadRoom("End", roomToRemove.x, roomToRemove.y);
    }
    public void LoadBossRoomFurthest()
    {
        //Start as the last empty room spawned
        Room furthestRoom = loadedRooms[loadedRooms.Count-1];

        //Compare each room in loadedRooms to the furthest room
        foreach(Room room in loadedRooms)
        {
            if(furthestRoom.distanceFromStart < room.distanceFromStart)
            {
                furthestRoom = room;

                // Debug.Log("Furthest Room is "+ furthestRoom.name);
            }      
        }

        //Find a potentiel pos for a deadEnd room around the furthest room
       Vector2Int bossRoom = FindDeadEnd(furthestRoom);

        // // Remove this room from the loadedRoom list
        // loadedRooms.Remove(furthestRoom);

        // Destroy(furthestRoom.gameObject);

        // // Load a Boss room in its place
        // LoadRoom("End", furthestRoom.x, furthestRoom.y);
        LoadRoom("End", bossRoom.x, bossRoom.y);

        bossRoomSpawned = true;
    }

    //Check pos around a certain room and return a deadend pos
    public Vector2Int FindDeadEnd(Room corridor)
    {
        if(!DoesRoomExist(corridor.x+1, corridor.y))
        {
            // Debug.Log("Potenitel room deadEnd at ("+(corridor.x+1)+","+(corridor.y)+")");  

            if(!DoesRoomExist(corridor.x+1, corridor.y+1))
             if(!DoesRoomExist(corridor.x+1, corridor.y-1))
              if(!DoesRoomExist(corridor.x+2, corridor.y))
                return new Vector2Int(corridor.x+1, corridor.y);
        }

        if(!DoesRoomExist(corridor.x-1, corridor.y))
        {
            // Debug.Log("Potenitel room deadEnd at ("+(corridor.x-1)+","+(corridor.y)+")"); 

            if(!DoesRoomExist(corridor.x-1, corridor.y+1))
             if(!DoesRoomExist(corridor.x-1, corridor.y-1))
              if(!DoesRoomExist(corridor.x-2, corridor.y))
                return new Vector2Int(corridor.x-1, corridor.y);
        }

        if(!DoesRoomExist(corridor.x, corridor.y+1))
        {
            // Debug.Log("Potenitel room deadEnd at ("+(corridor.x)+","+(corridor.y+1)+")");

            if(!DoesRoomExist(corridor.x+1, corridor.y+1))
             if(!DoesRoomExist(corridor.x-1, corridor.y+1))
              if(!DoesRoomExist(corridor.x, corridor.y+2))
                return new Vector2Int(corridor.x, corridor.y+1);
        }

        if(!DoesRoomExist(corridor.x, corridor.y-1))
        {
            // Debug.Log("Potenitel room deadEnd at ("+(corridor.x)+","+(corridor.y-1)+")");

            if(!DoesRoomExist(corridor.x+1, corridor.y-1))
             if(!DoesRoomExist(corridor.x-1, corridor.y-1))
              if(!DoesRoomExist(corridor.x, corridor.y-2))
                return new Vector2Int(corridor.x, corridor.y-1);
        }

        Debug.Log("Did not create a dead end with this corridor room...");
        return new Vector2Int(corridor.x, corridor.y);
    }

    //Set the room within our umbrella scene with the right coordonate
    public void RegisterRoom(Room room)
    {

        //Makes sure this room doesnt already exist to avoid duplicate
        if(!DoesRoomExist(currentLoadRoomData.x, currentLoadRoomData.y))
        {
            //Set the position of this room
            //room position =( X * width , Y * height , 0)
            room.transform.position = new Vector3(
                currentLoadRoomData.x * room.width,
                currentLoadRoomData.y * room.height,
                0);

            //place this room inside its parent and give it a clear name
            room.x = currentLoadRoomData.x;
            room.y = currentLoadRoomData.y;
            room.name = currentLoadRoomData + "-" + currentLoadRoomData.name + " " + room.x + ", " + room.y;
            room.transform.parent = transform;

            //This room is now done laoding
            isLoadingRoom = false;

            //this is the first room, so lets place the cam on it
            if(loadedRooms.Count == 0)
            {
                CameraController.instance.currRoom = room;
            }

            //Add this room to the loaded rooms list
            loadedRooms.Add(room);
        }
        else
        {
            //Else destroy the duplicated room
            Destroy(room.gameObject);
            isLoadingRoom = false;
        }
    }

    //Remove excessiveScene
    public IEnumerator RemoveExcessiveScenes()
    {
        //Wait a second to make sure all the rooms were loaded
        yield return new WaitForSeconds(1);

        int numberOfScene = SceneManager.sceneCount; 

        for(int i=0; i<numberOfScene;i++)
        {
            //Doesnt remove scene 0 because it's the playing scene
            if(i != 0)
            {
                //Debug.Log("Unloading Scene # "+ i + " " +SceneManager.GetSceneAt(i).name);

                //unload 'i' scene
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));                 
            }
        }

    }
    //Check if a room(x,y) already exists
    public bool DoesRoomExist(int x, int y)
    {
        //Scan through the loadedRooms List for a room with mathcing x and y 
        //true if match, false if null
        return loadedRooms.Find(item => item.x == x && item.y == y) != null;
    }

    //Return a specific room
    public Room FindRoom(int x, int y)
    {
        //Scan through the loadedRooms List for a room with mathcing x and y 
        //true if match, false if null
        return loadedRooms.Find(item => item.x == x && item.y == y);
    }

    public string GetRandomRoomName()
    {
        //Spawnable Common Room in dungeon
        string[] possibleRooms = new string[] {
            // "Empty",
            "Basic",
        };

        //Return a the name of one of these rooms
        return possibleRooms[Random.Range(0, possibleRooms.Length)]; 
    }

    //When player enters a room, this room should become the new current room
    public void OnPlayerEnterRoom(Room room)
    {
        CameraController.instance.currRoom = room;
        currRoom = room;
    }

}

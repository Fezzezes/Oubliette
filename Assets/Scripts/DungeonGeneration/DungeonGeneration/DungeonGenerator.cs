using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGenerationData dungeonGenerationData;

    private List<Vector2Int> dungeonRooms;

    private void Start()
    {
        //This list is equal to list of every position visited by a crawler via 'GenerateDungeon'
        dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonGenerationData);

        //Spawn a room to every position within that list
        SpawnRooms(dungeonRooms);
    }

    //Spawns every rooms for our dungeon
    private void SpawnRooms(IEnumerable<Vector2Int> rooms)
    {
        //Always spawn a StartRoom at 0,0
        RoomController.instance.LoadRoom("Start", 0,0);

        //Spawn an empty room to every position within that list
        foreach(Vector2Int roomLocation in rooms)
        {
            RoomController.instance.LoadRoom(RoomController.instance.GetRandomRoomName(), roomLocation.x, roomLocation.y);
        }

    }
}

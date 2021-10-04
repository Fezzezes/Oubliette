using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCrawler : MonoBehaviour
{
    public string Id;
    public Vector2Int Position{get; set;}

    //Set a dungeonCrawler to its starting pos (should be (0,0))
    public DungeonCrawler(Vector2Int startPos, int id)
    {
        Position = startPos;
        Id = ("crawler #"+id);
    }


    //Pick a random direction with the direction dictionary
    public Vector2Int Move(Dictionary<Direction, Vector2Int> directionMovementMap)
    {
        //Choose a random number bewteen 0 and 3
        Direction toMove = (Direction)Random.Range(0, directionMovementMap.Count);

        //add the direction to the position vector using 'toMove'
        Position += directionMovementMap[toMove];

        //return our new Position
        return Position;
    }
}

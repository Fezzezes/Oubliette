using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Direction
{
    up =0,
    left = 1,
    bottom = 2,
    right = 3
}

public class DungeonCrawlerController : MonoBehaviour
{
    //List of position visited vector converted into int
    public static List<Vector2Int>positionVisited = new List<Vector2Int>();  

    //Dictionary containing every possible direction
    private static readonly Dictionary<Direction, Vector2Int> directionMovementMap = new Dictionary<Direction, Vector2Int>
    {       
        //Vector2Int.up == (0,1)
        //Vector2Int.left == (-1,0)
        //Vector2Int.down== (0,-1)
        //Vector2Int.right == (1,0)

        {Direction.up, Vector2Int.up},
        {Direction.left, Vector2Int.left},
        {Direction.bottom, Vector2Int.down},
        {Direction.right, Vector2Int.right}
    } ;

    //return a list of rooms position
    public static List<Vector2Int> GenerateDungeon(DungeonGenerationData dungeonData)
    {
        //Make a list of all future crawlers
        List<DungeonCrawler> dungeonCrawlers = new List<DungeonCrawler>();

        //Add a number of crawler to Pos(0,0) to the list
        for (int i=0; i<dungeonData.numberOfCrawlers; i++)
        {
            
            dungeonCrawlers.Add(new DungeonCrawler(Vector2Int.zero,i));

        }
        //Create a random number between iteration min and iteration Max to decide how many move each crawler will make
        int iterations = Random.Range(dungeonData.iterationMin, dungeonData.iterationMax);
        Debug.Log("Room iterations = " + iterations);

        //+= a new direction to each crawler equals to the number of iteration,
        //every new position visited without a room will create a new room
        foreach(DungeonCrawler dungeonCrawler in dungeonCrawlers )
        {
            for(int i = 0; i<iterations; i++)
            {

                //Store the position before the crawler moves, and after it moved
                Vector2Int oldPos = dungeonCrawler.Position;
                Vector2Int newPos = dungeonCrawler.Move(directionMovementMap);

                if(positionVisited.Count == 0)
                {
                    positionVisited.Add(newPos);
                    // Debug.Log(dungeonCrawler.Id+ "| NewPos: "+newPos+" |index: "+i);
                }
                else if(positionVisited.Contains(newPos))
                {
                    //Dont count this turn because this pos was already visited
                    //Give back this loop turn, but dont let it go negative
                    if(i>0)
                        i--;
                    // Debug.Log(dungeonCrawler.Id+"| Bad NewPos: "+newPos+" |index: "+i);
                }
                else if(newPos == new Vector2Int(0,0))
                {
                    //Dont count this turn because this pos is the startpos
                    //Give back this loop turn, but dont let it go negative
                    if(i>0)
                        i--;
                    // Debug.Log(dungeonCrawler.Id+"| Bad NewPos: "+newPos+" |index: "+i);
                }
                else 
                {
                    //This is a new pos so add it to the list
                    positionVisited.Add(newPos);
                    //Debug.Log(dungeonCrawler.Id+ "| NewPos: "+newPos+" |index: "+i);
                }
            }
        }

        //Return a complete list of every position visited by every crawlers
        return positionVisited;
    }

}

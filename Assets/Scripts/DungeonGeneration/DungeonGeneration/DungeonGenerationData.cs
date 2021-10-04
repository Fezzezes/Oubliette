using UnityEngine;


[CreateAssetMenu(fileName = "DungeonGeneration.asset", menuName = "DungeonGenerationData/Dungeon Data")]

public class DungeonGenerationData : ScriptableObject
{
    //number of crawlers wanted
    public int numberOfCrawlers;

    //min and max possible room created via crawler
    public int iterationMin;
    public int iterationMax;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRoomSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct RandomSpawner
    {
        public string name;
        public SpawnerData spawnerData;
    }

    public GridController grid;
    public RandomSpawner[] spawnerData;

    void Start()
    {
        //grid = GetComponentInChildren<GridController>();
    }

    public void InitializeOnjectSpawning()
    {
        //Spawn each gameObject based their data
        foreach(RandomSpawner rs in spawnerData)
        {
            SpawnedObjects(rs);
        }
    }

    void SpawnedObjects(RandomSpawner data)
    {
        //'+1 because unity exclude the max value so (0,1) would === (0,0)...'
        int randomIteration = Random.Range(data.spawnerData.minSpawn, data.spawnerData.maxSpawn+1);

        for(int i =0; i<randomIteration; i++)
        {
            //Randomly pick a gameObject index the availablePoints list
            int randomPos = Random.Range(0, grid.avaiblePoints.Count-1);

            //Location the room hierarchy -> Grid controller/objectspawned
            Transform t = transform.GetChild(1).transform.GetChild(2).transform;

            //Instantiate a gameObject(with data.spawner), at the gridtile.position =to random index 
            GameObject go = Instantiate(data.spawnerData.itemToSpawn, grid.avaiblePoints[randomPos], Quaternion.identity, t) as GameObject;

            //Remove this gridTile from the list of availablePoints
            grid.avaiblePoints.RemoveAt(randomPos);

            // Debug.Log("Spawned an Object");
        }
    }

}

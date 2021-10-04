using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{   
    //This room
    public Room room;

    [System.Serializable]
    public struct  customGrid
    {
        public int colums, rows;

        //V: 4.51
        //H: 8.48
        public float verticalOffset, horizontalOffset;
    }

    public customGrid grid;
    public GameObject gridTile;

    bool meshCreated = false;

    //All available Position on our grid
    public List<Vector2> avaiblePoints = new List<Vector2>();

    void Awake()
    {
        //Get this room
        room = GetComponentInParent<Room>();

        //'-2' because the grid is based off the red gizmos, which is in the middle of the room walls
        //We dont want stuff to spawn inside walls
        grid.colums = room.width-3;
        grid.rows = room.height-3;

        GenerateGrid();
    }

    void Update()
    {
        if(RoomController.instance.doorRemoved && !meshCreated)
        {
                meshCreated = true;
                room.CreateDoorPath();
        }
    }

#region  GenerateGrid

    public void GenerateGrid()
    {

        //Get the local(parent) position of the room 
        grid.verticalOffset += room.transform.localPosition.y;
        grid.horizontalOffset += room.transform.localPosition.x;

        for(int y = 0; y< grid.rows; y++)
        {
            for(int x =0; x<grid.colums; x++)
            {
                //Place in the hierarchy
                Transform t = transform.GetChild(1).transform;

                //Spawn a gridTile GameOnject at the parent positon(transform)
                GameObject go = Instantiate(gridTile, t);

                //Give it a new positon based off the grid parameters
                go.transform.position = new Vector2(x-(grid.colums - grid.horizontalOffset), y - (grid.rows-grid.verticalOffset));

                //Give it a name
                go.name = "X: "+ x +" Y: "+ y;

                //Add it to the list of available position for future stuff we want to spawn
                avaiblePoints.Add(go.transform.position);  

                //Disable the grid sprite
                go.SetActive(false);
            }
        }

        // Spawn GameObjects on the grid
        GetComponentInParent<ObjectRoomSpawner>().InitializeOnjectSpawning();
    }
#endregion

#region polyPathGeneration

    //Enum of all possible polyPath link
    // public enum MeshTypeEnum
    // {
    //     vertical, horizontal
    // }

    //Name of this polyPath
    public string polyPathType;

    //Create a single polyPath GameObject between 2 door
    public void CreateSinglePPath(Door doorA, Door doorB, string meshName)
    {
        //Create the GO with needed component, name and parent
        #region Initialize GameObject PolygonTrigger
        //room -> Grid -> MeshPath
        Transform t = room.transform.GetChild(1).transform.GetChild(0).transform;

        //Create Gameobject
        GameObject pp = new GameObject();

        //Setup its parent using previous transform 't'
        pp.transform.SetParent(t);

        //Give it a name
        pp.name = meshName;

        //Need this to detect the trigger
        pp.AddComponent<MeshTrigger>();

        #endregion

        switch(polyPathType)
        {
            //SetCross == all 1221
            //SetDiamond == (TopBot == 1221, LeftRight == 1212)
            //SetZshape == all 1212

            case "Cross":
            SetCross(doorA, doorB, pp);
            break;
            case "Diamond":
            SetDiamond(doorA, doorB, pp);
            break;
            case "TRTable":
            SetDiamond(doorA, doorB, pp);
            break;
            case "TLTable":
            SetDiamond(doorA, doorB, pp);
            break;
            case "BRTable":
            SetDiamond(doorA, doorB, pp);
            break;
            case "BLTable":
            SetDiamond(doorA, doorB, pp);
            break;
            case "Zshape":
            SetZshape(doorA, doorB, pp);
            break;
            case "Sshape":
            SetCross(doorA, doorB, pp);
            break;
        }

    }

    public void SetCross(Door doorA, Door doorB, GameObject pp)
    {
        //Good for Cross and Sshape (All 1221)

        //Setup the polygon collider component
        PolygonCollider2D ppCollider = pp.AddComponent<PolygonCollider2D>();
        ppCollider.isTrigger = true;

        Vector2 point1 = new Vector2(doorA.cornerPoint1.transform.position.x,doorA.cornerPoint1.transform.position.y);  //DoorA Corner 1
        Vector2 point2 = new Vector2(doorA.cornerPoint2.transform.position.x,doorA.cornerPoint2.transform.position.y);  //DoorA Corner 2
        Vector2 point3 = new Vector2(doorB.cornerPoint1.transform.position.x,doorB.cornerPoint1.transform.position.y);  //DoorB Corner 1
        Vector2 point4 = new Vector2(doorB.cornerPoint2.transform.position.x,doorB.cornerPoint2.transform.position.y);  //DoorB Corner 2

        ppCollider.points = new[]{point1, point2, point4, point3}; //1221
    }

    public void SetDiamond(Door doorA, Door doorB, GameObject pp)
    {
        //Good for Diamond and Tables (topbot == 1221, leftright == 1212)

        PolygonCollider2D ppCollider = pp.AddComponent<PolygonCollider2D>();
        ppCollider.isTrigger = true;

            Vector2 point1 = new Vector2(doorA.cornerPoint1.transform.position.x,doorA.cornerPoint1.transform.position.y);  //DoorA Corner 1
            Vector2 point2 = new Vector2(doorA.cornerPoint2.transform.position.x,doorA.cornerPoint2.transform.position.y);  //DoorA Corner 2
            Vector2 point3 = new Vector2(doorB.cornerPoint1.transform.position.x,doorB.cornerPoint1.transform.position.y);  //DoorB Corner 1
            Vector2 point4 = new Vector2(doorB.cornerPoint2.transform.position.x,doorB.cornerPoint2.transform.position.y);  //DoorB Corner 2

        if(doorA == room.topDoor || doorA == room.bottomDoor)
            ppCollider.points = new[]{point1, point2, point4, point3}; // 1221
        
        else if(doorA == room.rightDoor || doorA == room.leftDoor)
            ppCollider.points = new[]{point1, point2, point3, point4}; //1212
    }

    public void SetZshape(Door doorA, Door doorB, GameObject pp)
    {
        //Good for Zshappe (LeftRight == 1212, TopBot == 1221)

        PolygonCollider2D ppCollider = pp.AddComponent<PolygonCollider2D>();
        ppCollider.isTrigger = true;

            Vector2 point1 = new Vector2(doorA.cornerPoint1.transform.position.x,doorA.cornerPoint1.transform.position.y);  //DoorA Corner 1
            Vector2 point2 = new Vector2(doorA.cornerPoint2.transform.position.x,doorA.cornerPoint2.transform.position.y);  //DoorA Corner 2
            Vector2 point3 = new Vector2(doorB.cornerPoint1.transform.position.x,doorB.cornerPoint1.transform.position.y);  //DoorB Corner 1
            Vector2 point4 = new Vector2(doorB.cornerPoint2.transform.position.x,doorB.cornerPoint2.transform.position.y);  //DoorB Corner 2

         if(doorA == room.rightDoor || doorA == room.leftDoor)
            ppCollider.points = new[]{point1, point2, point4, point3}; // 1221
        
        else if(doorA == room.topDoor || doorA == room.bottomDoor)
            ppCollider.points = new[]{point1, point2, point3, point4}; //1212
    }

    public void SetupPolyPath()
    {
        GetRandomPolyPathType();


        switch(polyPathType)
        {
            case "Cross":
            CreateSinglePPath(room.topDoor, room.bottomDoor,"Vertical Mesh Path");
            CreateSinglePPath(room.leftDoor, room.rightDoor,"Horizontal Mesh Path");
            break;

            case "Diamond":
            CreateSinglePPath(room.topDoor, room.leftDoor,"TopLeft PolyPath");
            CreateSinglePPath(room.bottomDoor, room.rightDoor,"BotRight PolyPath");
            CreateSinglePPath(room.leftDoor, room.bottomDoor,"BotLeft PolyPath");
            CreateSinglePPath(room.rightDoor, room.topDoor,"TopRight PolyPath");
            break;

            case "TRTable":
            CreateSinglePPath(room.topDoor, room.leftDoor,"TopLeft PolyPath");
            CreateSinglePPath(room.bottomDoor, room.rightDoor,"BotRight PolyPath");
            CreateSinglePPath(room.leftDoor, room.bottomDoor,"BotLeft PolyPath");
            break;

            case "TLTable":
            CreateSinglePPath(room.bottomDoor, room.rightDoor,"BotRight PolyPath");
            CreateSinglePPath(room.leftDoor, room.bottomDoor,"BotLeft PolyPath");
            CreateSinglePPath(room.rightDoor, room.topDoor,"TopRight PolyPath");
            break;

            case "BRTable":
            CreateSinglePPath(room.topDoor, room.leftDoor,"TopLeft PolyPath");
            CreateSinglePPath(room.leftDoor, room.bottomDoor,"BotLeft PolyPath");
            CreateSinglePPath(room.rightDoor, room.topDoor,"TopRight PolyPath");
            break;

            case "BLTable":
            CreateSinglePPath(room.topDoor, room.leftDoor,"TopLeft PolyPath");
            CreateSinglePPath(room.bottomDoor, room.rightDoor,"BotRight PolyPath");
            CreateSinglePPath(room.rightDoor, room.topDoor,"TopRight PolyPath");
            break;

            case "Zshape":
            CreateSinglePPath(room.topDoor, room.rightDoor,"TopLeft PolyPath");
            CreateSinglePPath(room.leftDoor, room.rightDoor,"BotLeft PolyPath");
            CreateSinglePPath(room.bottomDoor, room.leftDoor,"TopRight PolyPath");
            break;

            case "Sshape":
            CreateSinglePPath(room.topDoor, room.leftDoor,"TopLeft PolyPath");
            CreateSinglePPath(room.bottomDoor, room.rightDoor,"BotRight PolyPath");
            CreateSinglePPath(room.leftDoor, room.rightDoor,"TopRight PolyPath");
            break;           
            
        }
    }

    public void GetRandomPolyPathType()
    {
        //Spawnable Common Room in dungeon
        string[] possibleMeshPathType = new string[] {
            "Cross",
            "Diamond",
            "BRTable",
            "BLTable",
            "TRTable",
            "TLTable",
            "Sshape",
            "Zshape",
        };

        polyPathType= possibleMeshPathType[Random.Range(0, possibleMeshPathType.Length)];

    }





#endregion
}

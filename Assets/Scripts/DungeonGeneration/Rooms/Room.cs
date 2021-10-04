using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    //widht and height of this room
    public int width;
    public int height;

    //World Coordonate of this room
    public int x;
    public int y;

    public int distanceFromStart;

    private bool updatedDoors = false;
    private bool distanceFound = false;

    //Make a room with specific coordonates
    public Room(int X, int Y)
    {
        x = X;
        y = Y;
    }

   //Every possible door of this room + a list of all of them
    public Door leftDoor;
    public Door rightDoor;
    public Door topDoor;
    public Door bottomDoor;
    public List<Door> doors = new List<Door>();
    public List<GameObject> doorGameObject = new List<GameObject>();

    void Start()
    {

        //Make sure that we are starting in the right scene
        if(RoomController.instance == null)
        {
            Debug.Log("You pressed play in the wrong scene");
            return;
        }

        for(int i = 0; i < 4; i++)
        {

            //MODIFIED FOR TILESET ++++++++++++++++

            int structureIndex = 0;
            int doorsIndex = 1;

            doorGameObject.Add(transform.GetChild(structureIndex)
            .transform.GetChild(structureIndex)
            .transform.GetChild(doorsIndex)
            .transform.GetChild(i).gameObject);

            //MODIFIED FOR TILESET ++++++++++++++++
        }

        //Makes an array of all the doors component in the room gameobject
        //Use a switch to figure out which door is which in the array
        Door[] ds = GetComponentsInChildren<Door>();
        foreach(Door d in ds)
        {
            //Adds the door to the door list
            doors.Add(d); 
            switch(d.doorType)
            {
                case Door.DoorType.right:
                rightDoor = d;
                break;

                case Door.DoorType.left:
                leftDoor = d;
                break;

                case Door.DoorType.top:
                topDoor = d;
                break;

                case Door.DoorType.bottom:
                bottomDoor = d;
                break;
            }
        }

        //Shuffle doors position on their wall
        ShuffleDoors();

        

        //This room exist so lets register it
        RoomController.instance.RegisterRoom(this);
    }

    void Update()
    {
        if(name.Contains("End")&& !updatedDoors)
        {
            RemoveUnconnectedDoors();
            updatedDoors = true;
        }
        if(!distanceFound)
        {
            SetDistance();
            distanceFound = true;
        }
    }

    //Removes unconnected doors of a room
    public void RemoveUnconnectedDoors()
    {
        //Magic numbers are illegal
        const int DoorSprite = 0;

        foreach(Door d in doors)
        {
            //Check if there is an adjacent room, if not(null) disable this door
            switch(d.doorType)
            {
                //Turn off the door sprite and turn on the door collider if there is no adjacent room
    
                case Door.DoorType.right:
                if(GetRight()==null)
                {
                    d.transform.GetChild(DoorSprite).gameObject.SetActive(false);
                    d.doorDisabled = true;
                    d.GetComponent<BoxCollider2D>().enabled = false;
                }
                break;

                case Door.DoorType.left:
                if(GetLeft()==null)
                {
                    d.transform.GetChild(DoorSprite).gameObject.SetActive(false);
                    d.doorDisabled = true;
                    d.GetComponent<BoxCollider2D>().enabled = false;
                }
                break;

                case Door.DoorType.top:
                if(GetTop()==null)
                {
                    d.transform.GetChild(DoorSprite).gameObject.SetActive(false);
                    d.doorDisabled = true;
                    d.GetComponent<BoxCollider2D>().enabled = false;
                }
                break;

                case Door.DoorType.bottom:
                if(GetBottom()==null)
                {
                    d.transform.GetChild(DoorSprite).gameObject.SetActive(false);
                    d.doorDisabled = true;
                    d.GetComponent<BoxCollider2D>().enabled = false;
                }
                break; 
            }
        }
    }
    public void SyncDoorWith(GameObject thisDoor, GameObject otherDoor)
    {
        //Offset to not have the exact same position
        float offset = 2.5f;

        //Make the position of both door the same
        switch(thisDoor.GetComponent<Door>().doorType)
        {
            //Make thisdoor.left the same as otherdoor.right
            case Door.DoorType.left:
            thisDoor.transform.position = new Vector2(otherDoor.transform.position.x+offset, otherDoor.transform.position.y);
            // Debug.Log("Sync");
            break;

            //Make thisdoor.top the same as otherdoor.bottom
            case Door.DoorType.top:
            thisDoor.transform.position = new Vector2(otherDoor.transform.position.x, otherDoor.transform.position.y-offset);
            //Debug.Log("Sync");
            break;
        }
    }

    public void MakeDoorPair()
    {
        //Check if there is a room on each side,
        //If yes sync the 2 rooms adjacent doors together
        //Only need to check half of them (all of them would be redundant)

        //their index in the doorGameObjectList (2,1,0,3)
        const int top     = 0;
        const int right   = 1;
        const int left    = 2;
        const int bottom  = 3;

        if(GetLeft() != null)
        {
            //There is a door left of this room, so we want this room left door synched with the other room right door
            SyncDoorWith(doorGameObject[left], GetLeft().doorGameObject[right]);
        }
        if(GetTop() != null)
        {   
            //There is a door left of this room, so we want this room left door synched with the other room right door
            SyncDoorWith(doorGameObject[top],GetTop().doorGameObject[bottom]);
        }
    }

    public void ShuffleDoors()
    {
        //min and max range of movement for a door on a wall
        const float minRange = -5.5f;
        const float maxRange =  5.5f;

        foreach(Door d in doors)
        {
            d.ChangeDoorPos(Random.Range(minRange, maxRange));
        }

        
    }

#region Adjacent door check

//Check if there is a room to specific side of our current room
    public Room GetRight()
    {
        if(RoomController.instance.DoesRoomExist(x+1,y))
        {

            return RoomController.instance.FindRoom(x+1,y);
        }

        return null;
    }
    public Room GetLeft()
    {
        if(RoomController.instance.DoesRoomExist(x-1,y))
        {

            return RoomController.instance.FindRoom(x-1,y);
        }

        return null;
    }
    public Room GetTop()
    {
        if(RoomController.instance.DoesRoomExist(x,y+1))
        {

            return RoomController.instance.FindRoom(x,y+1);
        }

        return null;
    } 
    public Room GetBottom()
    {
         if(RoomController.instance.DoesRoomExist(x,y-1))
        {

            return RoomController.instance.FindRoom(x,y-1);
        }

        return null;
    }

    #endregion

    public void CreateDoorPath()
    {
        
        // MeshPath mesh = GetComponentInChildren<MeshPath>();

        // //Get the name of the MeshPathType
        // string meshPathType = mesh.GetRandomMeshPathType();

        // mesh.SetupMeshPathList(meshPathType, this);
        // //Make a list of necessary mesh
        // //create each meshpath using cornerPoint from doors

        //Create that polyPath with the type randomly chosen above
        GetComponentInChildren<GridController>().SetupPolyPath();
    
    }


    public void SetDistance()
    {
        int newX = x;
        int newY = y;

        if(x < 0)
            newX *= -1;
        if(y < 0)
            newY *= -1;

        distanceFromStart = newX + newY;
    }

    //Find Center of this room
    public Vector3 GetRoomCentre()
    {
        return new Vector3(x * width, y * height);
    }

    //Show in editor perimeter of this room using gizmos
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));


    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //if the player is inside the room
        if(other.tag == "Player")
        {
            // Debug.Log("SwitchingRoom");
            RoomController.instance.OnPlayerEnterRoom(this);
        }
    }
}

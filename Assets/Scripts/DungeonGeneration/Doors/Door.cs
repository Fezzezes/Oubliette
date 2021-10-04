using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        left,right,top,bottom
    }

    public bool doorDisabled = false;
    public GameObject cornerPoint1;
    public GameObject cornerPoint2;
    public DoorType doorType;
    public GameObject doorCollider;
    private GameObject player;
    private float doorPush = 3.3f;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ChangeDoorPos(float offset)
    {
         switch(doorType)
        {
            //Move each door with the offset parameter (sidedoors get their 'y' offset divided by 2 because smaller wall on side)

            case DoorType.bottom:
            this.transform.position = new Vector2(transform.position.x+offset, transform.position.y);
            // cornerPoint1 = new Vector3(transform.position.x-meshOffset, transform.position.y, 0);
            // cornerPoint2 = new Vector3(transform.position.x+meshOffset, transform.position.y,0);
            break;

            case DoorType.left:
            this.transform.position = new Vector2(transform.position.x, transform.position.y+(offset/2));
            // cornerPoint1 = new Vector3(transform.position.x, transform.position.y-meshOffset, 0);
            // cornerPoint2 = new Vector3(transform.position.x, transform.position.y+meshOffset, 0);
            break;

            case DoorType.right:
            this.transform.position = new Vector2(transform.position.x, transform.position.y+(offset/2));
            // cornerPoint1 = new Vector3(transform.position.x, transform.position.y-meshOffset, 0);
            // cornerPoint2 = new Vector3(transform.position.x, transform.position.y+meshOffset, 0);
            break;

            case DoorType.top:
            this.transform.position = new Vector2(transform.position.x+offset, transform.position.y);
            // cornerPoint1 = new Vector3(transform.position.x-meshOffset,transform.position.y, 0);
            // cornerPoint2 = new Vector3(transform.position.x+meshOffset, transform.position.y,0);
            break;

            default:
            Debug.Log("No door dectected to move");
            break;
        }
    }
    

    void OnTriggerEnter2D(Collider2D other)
    {
        float xPos = transform.position.x;
        float yPos = transform.position.y;

        //If a player touches a door, teleport them in a direction based on which door is touched
        if(other.tag == "Player")
        {
            switch(doorType)
            {
                case DoorType.bottom:
                player.transform.position = new Vector2(xPos, yPos - doorPush);
                break;

                case DoorType.left:
                player.transform.position = new Vector2(xPos - doorPush, yPos);
                break;

                case DoorType.right:
                player.transform.position = new Vector2(xPos + doorPush, yPos);
                break;

                case DoorType.top:
                player.transform.position = new Vector2(xPos, yPos + doorPush);
                break;

                default:
                Debug.Log("No door dectected in trigger");
                break;
            }
        }
    }
}

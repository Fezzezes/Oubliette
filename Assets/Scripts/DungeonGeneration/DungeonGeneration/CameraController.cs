using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //This camera controller
    public static CameraController instance;

    //What is the current room, we should be looking at
    public Room currRoom;

    //How fast does the camera transition between rooms
    public float moveSpeedWhenRoomChange;


    void Awake()
    {

    //Set instance to this CameraController
    instance = this;

    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        //no room to update cam so do nothing
        if(currRoom == null)
        {
            return;
        }

        //Seek a new Position for the camera
        Vector3 targetPos = GetCameraTargetPosition();

        //Change the position of the camera (gradually using the moveSpeed variable and from our current pos to the new one)
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime*moveSpeedWhenRoomChange);

    }

    //Find to center of the current room and return it as the target
    //position for our camera
    Vector3 GetCameraTargetPosition()
    {
        //no room == do nothing
        if(currRoom == null)
        {
            return Vector3.zero;
        }

        //set the targetPosition to the current room center
        Vector3 targetPos = currRoom.GetRoomCentre();
        targetPos.z = transform.position.z;

        return targetPos;

    }

    //compare the current camera position to the target camera pos
    public bool IsSwitchingScene()
    {
        //Check if the current position of the camera is == to the target position
        return transform.position.Equals(GetCameraTargetPosition()) == false;
    }

}

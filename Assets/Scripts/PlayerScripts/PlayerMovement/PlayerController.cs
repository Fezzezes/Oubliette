using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
#region Movement
    private PlayerInputActions playerInputs;
    private Rigidbody2D rb;

    [SerializeField] private float speed = 10f;

    void Awake()
    {
        playerInputs = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 moveInput = playerInputs.Movement.Move.ReadValue<Vector2>();
        rb.velocity = moveInput*speed;
    }

#endregion

#region PlayerController

//Ref to non walkable object that will obstruct movement
public LayerMask solidObjectsLayer;
private bool isWalkable(Vector3 targetPos)
{

    if(Physics2D.OverlapCircle(targetPos, 0.3f, solidObjectsLayer) != null)
    {
        return false;
    }
    else 
        return true;
}


#endregion

}

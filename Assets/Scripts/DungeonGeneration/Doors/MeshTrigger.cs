using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Terrain")
        {
            Destroy(other.gameObject);
            // Debug.Log("Destroying gameObject "+other.gameObject.name);
        }
    }
    
}

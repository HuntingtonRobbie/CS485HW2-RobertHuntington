using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach to the main camera
//Make the camera follow the player
public class CamFollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    private void Start()
    {
        if (player == null)
            Debug.LogError("Player obj need to be assigned to the camera");
        
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + offset;
    }
}

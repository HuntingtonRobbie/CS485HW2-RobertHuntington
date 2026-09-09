using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 1. The Input System "using" statement

//This example project referenced video tutorial list made by Brackeys on Youtube
//https://www.youtube.com/watch?v=Au8oX5pu5u4&list=PLPV2KyIb3jR5QFsefuO2RlAgWEz6EvVi6&index=4

public class PlayerMove : MonoBehaviour
{
    public Rigidbody rb;

    //Important: public variables values set in the Unity editor will overwrite this init value set in code
    public float forwardSpeed = 1f;
    public float sidewaySpeed = 1f;

    // 2. These variables are to hold the Action references
    InputAction moveAction;
    Vector2 moveValue;

    // Start is called before the first frame update
    void Awake()
    {
        this.transform.position = new Vector3(0f,0f,0f);//make sure the player starts at 0 

        // 3. Find the references to the "Move" and "Jump" actions
        moveAction = InputSystem.actions.FindAction("Move");
        
    }

    // Update is called once per frame
    void Update()
    {
        // 4. Read the "Move" action value, which is a 2D vector
        moveValue = moveAction.ReadValue<Vector2>();
        MovePlayer();

       

        /*Older version code that can still work with Unity 6 after system setting change
        if (Input.GetKey("w"))
            rb.AddForce(0, 0, forwardForce * Time.deltaTime);
        if (Input.GetKey("a"))
            rb.AddForce(-sidewaysForce * Time.deltaTime, 0, 0);
        if (Input.GetKey("d"))
            rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0);
        */
        
    }
    //Move player forward
    private void MovePlayer()
    {
        //forward/backward movement. moveValue.y=1 when pressing W, moveValue=-1 when pressing S
        rb.MovePosition(rb.position+transform.forward*moveValue.y * Time.deltaTime*forwardSpeed);
        //sideway movement
        rb.MovePosition(rb.position + transform.right * moveValue.x * Time.deltaTime * forwardSpeed);
    }



}

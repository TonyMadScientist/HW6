using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 1. The Input System "using" statement

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
    void OnEnable()
    {
        //Debug.Log("PlayerMove script is enabled");

        // 3. Find the references to the "Move" and "Jump" actions
        moveAction = InputSystem.actions.FindAction("Move");

    }

    // Update is called once per frame
    void Update()
    {
        // 4. Read the "Move" action value, which is a 2D vector
        moveValue = moveAction.ReadValue<Vector2>();
        MovePlayer();

    }
    //Move player forward
    private void MovePlayer()
    {
        //forward/backward movement. moveValue.y=1 when pressing W, moveValue=-1 when pressing S
        rb.MovePosition(rb.position + transform.forward * moveValue.y * Time.deltaTime * forwardSpeed);
        //sideway movement
        rb.MovePosition(rb.position + transform.right * moveValue.x * Time.deltaTime * forwardSpeed);
    }

    public void StopPlayer()
    {
        rb.linearVelocity = new Vector3(0, 0, 0);
    }

}





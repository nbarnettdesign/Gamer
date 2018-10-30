using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingController : Controller {

    Vector3 walkVelocity;
    public float forwardSpeed = 3f;
    public float strafeSpeed = 2f;

    private float adjVertvelocity;

    public float ascendSpeed = 2f;
    public float descendSpeed = 2f;

    public float ascendCooldown = 1f;
    public float ascendTimer;


    public override void ReadInput(InputData data)
    {
        ResetMovementToZero();

        //set vertical movement
        if (data.axes[0] != 0f)
        {
            walkVelocity += Vector3.forward * data.axes[0] * forwardSpeed;
        }

        //set horizontal movement
        if (data.axes[1] != 0f)
        {
            walkVelocity += Vector3.right * data.axes[1] * strafeSpeed;
        }

        //set Ascension movement, reset jump timer 
        if (data.buttons[0] == true)
        {
            ascendTimer += Time.deltaTime;
            if (ascendTimer == ascendCooldown)
            {
                ascendTimer = 0f;
                adjVertvelocity = ascendSpeed;
                

            }

            

        }
        else
        {
            ascendTimer = 0f;
        }

        //new data update
        newInput = true;
    }



    private void LateUpdate()
    {
        if (!newInput)
        {
            ResetMovementToZero();
            ascendTimer = 0f;
        }

        //move the rb
        rb.velocity = new Vector3(walkVelocity.x, adjVertvelocity + rb.velocity.y, walkVelocity.z);

        newInput = false;
    }

    void ResetMovementToZero()
    {
        walkVelocity = Vector3.zero;
        adjVertvelocity = 0;
    }
}

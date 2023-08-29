using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class NewMoveScript : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    [SerializeField] private Rigidbody sphere;

    private float acceleration;
    private float currentSpeed;
    private float steerAmount;
    private float maxSpeed;
    private float originalSpeed;

    private float realSpeed;

    private float turnDegreeRight;
    private float turnDegreeLeft;

    [SerializeField] private float speed;
    [SerializeField] private float steerSpeed;

    public TextMeshProUGUI speedmeter; // IMPORTANT !!!!!!!!!!!!!!!! CLEAN UP LATER 


    private void Start()
    {
        sphere.transform.parent = null; //Ensures that the sphere does not follow movement of the Kart by unparenting the sphere

        maxSpeed = speed + 10.0f; //sets a max speed
        originalSpeed = speed; //set an original speed so that when speed is boosted by grappler, the speed boost is temporary and will lerp back to original speed
    }

    private void Update()
    {
        transform.position = sphere.transform.position; //makes the kart parent follow the sphere. 

        acceleration = Input.GetAxis(VERTICAL);
        steerAmount = Input.GetAxis(HORIZONTAL);

        speed = Mathf.Lerp(speed, originalSpeed, 2.0f * Time.deltaTime); //Lerps back to original speed in case of speed boost


        MoveKart();

        Steering();

        SpeedMeter();
    }

    private void FixedUpdate()
    {
        sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration); //Adds Force according to current Speed which is dictated by Input value;
    }

    public void MoveKart()
    {
        if (acceleration != 0)
        {
            Accelerate();
        }
        else
        {
            ReleaseAcceleration();
        }
    }

    public void Accelerate()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, acceleration * speed, 7.0f * Time.deltaTime); //takes Input amount * speed that is set in Inspector
    }

    public void ReleaseAcceleration()
    {
        currentSpeed = 0; //Deccelerate till stop
    }

    public void Steering()
    {
        float newRotation = steerAmount * steerSpeed * (1.0f * Time.deltaTime);
        transform.Rotate(xAngle: 0, yAngle: newRotation, zAngle: 0, Space.World);

        CounterForce();
    }

    public void CounterForce() //When turning sets counter force in order to prevent gliding like a mofo
    {
        if (steerAmount > 0)
        {
            //Sets a period of time before the counter force activates. This is necessary because you don't want the car to move diagonally
            turnDegreeRight = Mathf.Lerp(turnDegreeRight, steerAmount, 1.0f * Time.deltaTime);
        }
        else
        {
            turnDegreeRight = 0;
        }

        if (steerAmount < 0)
        {
            turnDegreeLeft = Mathf.Lerp(turnDegreeLeft, steerAmount, 1.0f * Time.deltaTime);
        }
        else
        {
            turnDegreeLeft = 0;
        }


        if (steerAmount > 0 && acceleration != 0 && turnDegreeRight > 0.5f)
        {
            sphere.AddForce(transform.right * 1.5f, ForceMode.Acceleration); //Counter force to the right when turning right
        }

        if (steerAmount < 0 && acceleration != 0 && turnDegreeLeft < -0.5f)
        {
            sphere.AddForce(-transform.right * 1.5f, ForceMode.Acceleration); //Counter force to the left when turning left
        }
    }



    public void SpeedMeter() // IMPORTANT !!!!!!!!!!!!!!!! CLEAN UP LATER 
    {
        realSpeed = transform.InverseTransformDirection(sphere.velocity).z; //This is the real speed, not applied speed

        float realVelocity = realSpeed * 3;

        if (realSpeed < 0f)
        {
            realVelocity = -realSpeed * 3;
        }

        speedmeter.text = realVelocity.ToString("f2");
    }
}

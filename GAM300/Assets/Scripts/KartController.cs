using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Unity.VisualScripting;

public class KartController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    [SerializeField] private Rigidbody sphere;

    // Inputs
    private float acceleration;
    private float steerAmount;
    private float reverseTimer;
    [HideInInspector] public bool isReversing;
    private bool isBraking;
    private bool reverseCheck;
    private bool reverseStop;

    // Speed Settings 
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float slowSpeed;
    [HideInInspector] public float boostSpeed;
    [HideInInspector] public float originalSpeed;
    [HideInInspector] public float realSpeed;
    [HideInInspector] public float brakeSpeed;
    [HideInInspector] public float reverseSpeed;
    [HideInInspector] public float highSpeedSteer;
    [HideInInspector] public float oringalSteerSpeed;

    // Rotation
    private float newRotation;
    private float maxSteerAngle = 40f;

    private bool touchingGround;

    // Boosting
    private bool isBoosting;
    private float boostInitialCD;

    [Header("Speed Settings")]
    public float maxSpeed;
    public float steerSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float steerSpeed;
    [SerializeField] private float boostCountdown;

    [Header("Tires")]
    [SerializeField] private Transform frontLeftTire;
    [SerializeField] private Transform frontRightTire;
    [SerializeField] private Transform rearLeftTire;
    [SerializeField] private Transform rearRightTire;

    public TextMeshProUGUI speedmeter; // IMPORTANT !!!!!!!!!!!!!!!! CLEAN UP LATER 

    private Quaternion carRot;


    private void Start()
    {
        boostInitialCD = boostCountdown;
        sphere.transform.parent = null; //Ensures that the sphere does not follow movement of the Kart by unparenting the sphere

        SpeedSettings();
    }

    private void Update()
    {
        //Debug.Log(currentSpeed);

        ResetSpeed();
        transform.position = sphere.transform.position; //makes the kart parent follow the sphere. 

        GetInput();

        Steering();

        SpeedMeter();

        BoostTimer();
        
        TireRotation();

        GroundNormalRotation();

        carRot = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z);

    }

    private void FixedUpdate()
    {
        //sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration); //Adds Force according to current Speed which is dictated by Input value;
        MoveKart();
    }

    public void MoveKart()
    {
        Vector3 vel = transform.forward * currentSpeed; //directly affects velocity of the vehicle in the direction that the kart is facing
        vel.y = sphere.velocity.y;
        sphere.velocity = vel;
    }

    public void BoostKart()
    {
        isBoosting = true;
        maxSpeed = boostSpeed;
    }
    
    public void BoostTimer() 
    { 
        if (isBoosting && boostCountdown > 0)
        {
            boostCountdown -= Time.deltaTime;
        }
        else
        {
            boostCountdown = 2f;
            maxSpeed = Mathf.Lerp(maxSpeed, originalSpeed, 2.0f * Time.deltaTime); //Lerps back to original speed in case of speed boost
            isBoosting = false;
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void GetInput()
    {
        acceleration = Input.GetAxis(VERTICAL);
        steerAmount = Input.GetAxis(HORIZONTAL);

        if (!Traps.instance.trapHit)
        {
            if (acceleration != 0)
            {
                if (touchingGround)
                {
                    Accelerate();
                }
            }
            else
            {
                ReleaseAcceleration();
            }

            ReversingChecker();
        }
    }

    public void Accelerate()
    {
        if (acceleration < 0 && realSpeed > 0) //controlling the braking speed.
        {
            isBraking = true;
            maxSpeed = brakeSpeed;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, acceleration * maxSpeed, 1.1f * Time.deltaTime); //takes Input amount * speed that is set in Inspector
    }

    public void ReleaseAcceleration()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, 0, 0.08f * Time.deltaTime); //Deccelerate till stop
    }

    public void ResetSpeed()
    {
        if (!isReversing || !reverseStop || !isBraking)
        {
            maxSpeed = Mathf.Lerp(maxSpeed, originalSpeed, 2.0f * Time.deltaTime); //Lerps back to original speed in case of speed boost or reverse
        }
    }

    public void ReversingChecker()
    {
        //ReverseSpeed

        if (realSpeed < 0) //crosschecks with actual speed to check if it is reversing. If the vehicle is reversing, actual speed will reflect as negative.
        {
            isReversing = true;
            maxSpeed = reverseSpeed; //25% of the original movespeed. caps reversing speed.
        }
        else
        {
            isReversing = false;
        }


        //ReversePause

        if (realSpeed > 10f) 
        {
            reverseCheck = true; //if the speed reaches this point, when reversing there will be a momentary pause. checks if the reverse needs to be paused.
            reverseTimer = 0;
        }

        if (reverseCheck && realSpeed < 1.5f)
        {
            reverseStop = true; //if the check hits and the speed is less than 2, the maxspeed will cap at 2, causing the vehicle to "pause". 

            reverseTimer += 1.0f * Time.deltaTime; //timer for when to stop the pause

            maxSpeed = 2f; //so there is a momentary pause when reversing. not instant reverse. more realistic feeling

            if (reverseTimer >= 1.0f) //once this timer reaches, no more pause
            {
                reverseStop = false;
                reverseCheck = false;
            }
        }
    }

    public void Steering()
    {
        if (touchingGround)
        {
            if (realSpeed >= 0.01f || realSpeed <= -0.001) //If the kart is at rest, unable to rotate
            {
                //Steering Kart

                newRotation = steerAmount * steerSpeed * (1.0f * Time.deltaTime);

                //Steering Angle Cap

                if (realSpeed > 24) // caps steering angle if the speed is high
                {
                    steerSpeed = Mathf.Lerp(steerSpeed, highSpeedSteer, 2.0f * Time.deltaTime); 
                }
                else
                {
                    steerSpeed = Mathf.Lerp(steerSpeed, oringalSteerSpeed, 2.0f * Time.deltaTime);
                }

                //Rotate Kart according to rotation

                transform.Rotate(xAngle: 0, yAngle: newRotation, zAngle: 0, Space.World);
            }
        }
    }

    public void TireRotation()
    {
        Quaternion positiveNewAngle = Quaternion.Euler(0, maxSteerAngle, 0);
        Quaternion negativeNewAngle = Quaternion.Euler(0, -maxSteerAngle, 0);
        Quaternion defaultAngle = Quaternion.Euler(0, 0, 0);

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (!isReversing)
            {
                frontLeftTire.localRotation = Quaternion.Lerp(frontLeftTire.localRotation, positiveNewAngle, 4.0f * Time.deltaTime);
                frontRightTire.localRotation = Quaternion.Lerp(frontRightTire.localRotation, positiveNewAngle, 4.0f * Time.deltaTime);
            }
            else //if is reversing, the tire steering will swap angles
            {
                frontLeftTire.localRotation = Quaternion.Lerp(frontLeftTire.localRotation, negativeNewAngle, 4.0f * Time.deltaTime);
                frontRightTire.localRotation = Quaternion.Lerp(frontRightTire.localRotation, negativeNewAngle, 4.0f * Time.deltaTime);
            }
            
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (!isReversing)
            {
                frontLeftTire.localRotation = Quaternion.Lerp(frontLeftTire.localRotation, negativeNewAngle, 4.0f * Time.deltaTime);
                frontRightTire.localRotation = Quaternion.Lerp(frontRightTire.localRotation, negativeNewAngle, 4.0f * Time.deltaTime);
            }
            else //if is reversing, the tire steering will swap angles
            {
                frontLeftTire.localRotation = Quaternion.Lerp(frontLeftTire.localRotation, positiveNewAngle, 4.0f * Time.deltaTime);
                frontRightTire.localRotation = Quaternion.Lerp(frontRightTire.localRotation, positiveNewAngle, 4.0f * Time.deltaTime);
            }
        }
        else //reset to normal 0 angle
        {
            frontLeftTire.localRotation = Quaternion.Lerp(frontLeftTire.localRotation, defaultAngle, 4.0f * Time.deltaTime);
            frontRightTire.localRotation = Quaternion.Lerp(frontRightTire.localRotation, defaultAngle, 4.0f * Time.deltaTime);
        }

        // Tire Spin
        frontLeftTire.GetChild(0).Rotate(0, 0, -90 * realSpeed * Time.deltaTime);
        frontRightTire.GetChild(0).Rotate(0, 0, -90 * realSpeed * Time.deltaTime);
        rearLeftTire.parent.Rotate(0, 0, -90 * realSpeed * Time.deltaTime);
        rearRightTire.parent.Rotate(0, 0, -90 * realSpeed * Time.deltaTime);

    }

    public void GroundNormalRotation()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, -transform.up,out hit, 0.75f))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
            touchingGround = true;
        }
        else
        {
            touchingGround = false;
            //transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(carRot.x + 0.01f, carRot.y,carRot.z), 2.0f * Time.deltaTime);
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

    public void SpeedSettings()
    {
        originalSpeed = maxSpeed; //set an original speed so that when speed is boosted by grappler, the speed boost is temporary and will lerp back to original speed
        boostSpeed = maxSpeed + 10.0f; //sets a max speed
        reverseSpeed = originalSpeed * 0.25f;
        brakeSpeed = originalSpeed * 0.3f;
        slowSpeed = maxSpeed - 25.0f; //sets a slow debuff speed
        highSpeedSteer = steerSpeed / 2; //for capping steering angle if the speed is high
        oringalSteerSpeed = steerSpeed;
        
    }
}

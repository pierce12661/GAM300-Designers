using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Unity.VisualScripting;

public class KartController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    public Rigidbody sphere;

    private Grapple grappleScript;

    // Inputs
    [HideInInspector] public float acceleration;
    [HideInInspector] public float accelerationControl;
    [HideInInspector] public float steerAmount;
    private float reverseTimer;
    private float airTime;

    [HideInInspector] public bool isReversing;
    private bool isBraking;
    private bool reverseCheck;
    private bool reverseStop;

    // Speed Settings 
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float slowSpeed;
    //[HideInInspector] public float boostSpeed;
    [HideInInspector] public float originalSpeed;
    [HideInInspector] public float realSpeed;
    [HideInInspector] public float brakeSpeed;
    [HideInInspector] public float reverseSpeed;
    [HideInInspector] public float highSpeedSteer;
    [HideInInspector] public float boostSteer;
    [HideInInspector] public float slowSteer;
    [HideInInspector] public float oringalSteerSpeed;

    //Boost Battery
    [HideInInspector] public float currentBattery;
    [HideInInspector] public float maxBattery;
    [HideInInspector] public float batteryPercentage;

    // Rotation
    private float newRotation;
    private float maxSteerAngle = 40f;

    [HideInInspector] public bool touchingGround;

    [Header("Speed Settings")]
    public float maxSpeed;
    public float steerSpeed;

    [Header("Tires")]
    [SerializeField] private Transform frontLeftTire;
    [SerializeField] private Transform frontRightTire;
    [SerializeField] private Transform rearLeftTire;
    [SerializeField] private Transform rearRightTire;

    private Quaternion carRot;

    public Transform gravitypos;

    [Header("Boost")]
    [HideInInspector] public bool isInitialBoosting;
    [HideInInspector] public bool isFinalBoosting;
    private float boostInitialCD;
    private float initialBoostSpeed;
    private float finalBoostSpeed;
    [SerializeField] private float boostCountdown;
    [SerializeField] private float finalBoostCountdown;
    
    //Traps
    [HideInInspector] public bool trapHit;
    [HideInInspector] public bool stunned;

    public Vector3 respawnPoint;


    private float targetAirForce;

    private void Start()
    {
        sphere.transform.parent = null; //Ensures that the sphere does not follow movement of the Kart by unparenting the sphere
        boostInitialCD = boostCountdown;

        grappleScript = this.GetComponent<Grapple>();

        SpeedSettings();
    }

    private void Update()
    {
        transform.position = sphere.transform.position; //makes the kart parent follow the sphere. 


        ResetSpeed();

        GetInput();

        Steering();

        TireRotation();

        GroundNormalRotation();

        BoostTimer();

        
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

        if (!touchingGround) //Pseudo Gravity when airtime
        {
            airTime += 1.0f * Time.deltaTime;

            Debug.Log(targetAirForce);
            if (airTime > 0.15f)
            {
                targetAirForce += 20.0f * Time.deltaTime;
                sphere.AddForce(-transform.up * targetAirForce, ForceMode.Acceleration); //23
            }
        }
        else
        {
            airTime = 0;
            targetAirForce = 0;
          
            sphere.AddForce(-transform.up * 100, ForceMode.Acceleration); //fake Gravity
        }
    }

    public void GetInput()
    {
        acceleration = Input.GetAxis(VERTICAL);
        steerAmount = Input.GetAxis(HORIZONTAL);

        realSpeed = transform.InverseTransformDirection(sphere.velocity).z; //This is the real speed, not applied speed

        if (!trapHit && !stunned)
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


        if (isInitialBoosting) //if initial boosting, accelerate slightly faster
        {
            accelerationControl = 0.9f;
        }
        else if (isFinalBoosting) //if final boosting, accelerate fastest
        {
            accelerationControl = 1.1f;
        }
        else if (KartCollisionDetector.instance.isSpinning)
        {
            accelerationControl = 0.2f;
        }
        else if(acceleration > 0) //normal acceleration
        {
            accelerationControl = Mathf.Lerp(accelerationControl, 0.5f * acceleration, 1.0f * Time.deltaTime);
        }
        else if(acceleration == 0) //release accelerator speed to slowly come to a stop
        {
            accelerationControl = Mathf.Lerp(accelerationControl, 0.1f, 1.0f * Time.deltaTime);
        }
        else //reverse acceleration speed
        {
            accelerationControl = 0.6f;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, acceleration * maxSpeed, accelerationControl * Time.deltaTime); //takes Input amount * speed that is set in Inspector
    }

    public void ReleaseAcceleration()
    {
        if (touchingGround)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, 0.08f * Time.deltaTime); //Deccelerate till stop
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, 0.4f * Time.deltaTime);
        }
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

        if (realSpeed < 0 && acceleration < 0) //crosschecks with actual speed to check if it is reversing. If the vehicle is reversing, actual speed will reflect as negative.
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
                if (!isReversing)
                {
                    newRotation = steerAmount * steerSpeed * (1.0f * Time.deltaTime);
                }
                else
                {
                    newRotation = -steerAmount * steerSpeed * (1.0f * Time.deltaTime);
                }


                //Steering Angle Cap
                if (!KartCollisionDetector.instance.isSpinning)
                {
                    if (realSpeed > 24) // caps steering angle if the speed is high
                    {
                        if (!isInitialBoosting)
                        {
                            steerSpeed = Mathf.Lerp(steerSpeed, highSpeedSteer, 2.0f * Time.deltaTime);
                        }
                        else
                        {
                            steerSpeed = Mathf.Lerp(steerSpeed, boostSteer, 2.0f * Time.deltaTime);
                        }
                        
                    }
                    else
                    {
                        steerSpeed = Mathf.Lerp(steerSpeed, oringalSteerSpeed, 2.0f * Time.deltaTime);
                    }
                }
                else
                {
                    steerSpeed = Mathf.Lerp(steerSpeed, slowSteer, 5.0f * Time.deltaTime);
                }
                

                //Rotate Kart according to rotation

                transform.Rotate(xAngle: 0, yAngle: newRotation, zAngle: 0, Space.World);
            }
        }
    }

    public void RotationRevert()
    {

        if (!touchingGround)
        {

        }
    }

    public void TireRotation()
    {
        Quaternion positiveNewAngle = Quaternion.Euler(0, maxSteerAngle, 0);
        Quaternion negativeNewAngle = Quaternion.Euler(0, -maxSteerAngle, 0);
        Quaternion defaultAngle = Quaternion.Euler(0, 0, 0);

        if (Input.GetKey(KeyCode.RightArrow))
        {
            frontLeftTire.localRotation = Quaternion.Lerp(frontLeftTire.localRotation, positiveNewAngle, 4.0f * Time.deltaTime);
            frontRightTire.localRotation = Quaternion.Lerp(frontRightTire.localRotation, positiveNewAngle, 4.0f * Time.deltaTime);

            //if (!isReversing)
            //{
            //    frontLeftTire.localRotation = Quaternion.Lerp(frontLeftTire.localRotation, positiveNewAngle, 4.0f * Time.deltaTime);
            //    frontRightTire.localRotation = Quaternion.Lerp(frontRightTire.localRotation, positiveNewAngle, 4.0f * Time.deltaTime);
            //}
            //else //if is reversing, the tire steering will swap angles
            //{
            //    frontLeftTire.localRotation = Quaternion.Lerp(frontLeftTire.localRotation, negativeNewAngle, 4.0f * Time.deltaTime);
            //    frontRightTire.localRotation = Quaternion.Lerp(frontRightTire.localRotation, negativeNewAngle, 4.0f * Time.deltaTime);
            //}
            
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            frontLeftTire.localRotation = Quaternion.Lerp(frontLeftTire.localRotation, negativeNewAngle, 4.0f * Time.deltaTime);
            frontRightTire.localRotation = Quaternion.Lerp(frontRightTire.localRotation, negativeNewAngle, 4.0f * Time.deltaTime);

            //if (!isReversing)
            //{
            //    frontLeftTire.localRotation = Quaternion.Lerp(frontLeftTire.localRotation, negativeNewAngle, 4.0f * Time.deltaTime);
            //    frontRightTire.localRotation = Quaternion.Lerp(frontRightTire.localRotation, negativeNewAngle, 4.0f * Time.deltaTime);
            //}
            //else //if is reversing, the tire steering will swap angles
            //{
            //    frontLeftTire.localRotation = Quaternion.Lerp(frontLeftTire.localRotation, positiveNewAngle, 4.0f * Time.deltaTime);
            //    frontRightTire.localRotation = Quaternion.Lerp(frontRightTire.localRotation, positiveNewAngle, 4.0f * Time.deltaTime);
            //}
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

        if(Physics.Raycast(transform.position, -transform.up,out hit, 1.25f))
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

    public void SpeedSettings()
    {
        originalSpeed = maxSpeed; //set an original speed so that when speed is boosted by grappler, the speed boost is temporary and will lerp back to original speed
        initialBoostSpeed = maxSpeed * 1.10f; //sets a max speed
        finalBoostSpeed = initialBoostSpeed; //sets a max speed
        reverseSpeed = originalSpeed * 0.25f;
        brakeSpeed = originalSpeed * 0.3f;
        slowSpeed = maxSpeed * 0.3f; //sets a slow debuff speed
        highSpeedSteer = steerSpeed / 2.4f; //for capping steering angle if the speed is high
        boostSteer = steerSpeed / 1.15f; 
        slowSteer = steerSpeed / 6;
        oringalSteerSpeed = steerSpeed;

        //BatterySettings
        maxBattery = 5f;
        currentBattery = 0f;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public float GetBatteryPercentage()
    {
        float percentage = currentBattery / maxBattery;

        return percentage;
    }

    public void InitialBoostKart()
    {
        isInitialBoosting = true;
        maxSpeed = initialBoostSpeed;

        //sphere.AddForce(gameObject.transform.forward * 1000, ForceMode.Acceleration); //boost force

        CameraShake.instance.BoostShake();
        AudioManager.Instance.PlayDrift();
    }

    public void FinalBoostKart()
    {
        batteryPercentage = GetBatteryPercentage();

        isFinalBoosting = true;
        maxSpeed = finalBoostSpeed * (1 + (currentBattery/maxBattery / 2));

        Debug.Log(maxSpeed + " final boost speed");
        //sphere.AddForce(gameObject.transform.forward * 1000, ForceMode.Acceleration); //boost force

    }

    public void BoostTimer()
    {
        if (!isFinalBoosting && isInitialBoosting && boostCountdown > 0)
        {
            boostCountdown -= 1.0f * Time.deltaTime;
        }
        else if (isFinalBoosting && currentBattery > 0)
        {
            isInitialBoosting = false;
            //finalBoostCountdown -= 1.0f * Time.deltaTime;
        }
        else
        {
            boostCountdown = 2f;
            //finalBoostCountdown = 2f;
            maxSpeed = Mathf.Lerp(maxSpeed, originalSpeed, 2.0f * Time.deltaTime); //Lerps back to original speed in case of speed boost
            isInitialBoosting = false;
            isFinalBoosting = false;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [System.Serializable]
    public class CameraSettings
    {
        [Header("Camera Settings")]
        public float boostZoomSpeed;
        public float MegaBoostZoomSpeed;
        public float moveSpeed;
        public float airTimeCamSpeed;
        public float rotationSpeed;
        public float originalFOV;
        public float boostFOV;
        public float megaBoostFOV;
        public float mouseX_Sensitivity;
        public float mouseY_Sensitivity;
        public float maxVerticalClampAngle;
        public float minVerticalClampAngle;
        public float maxHorizontalClampAngle;
        public float minHorizontalClampAngle;
        
    }

    [SerializeField]
    public CameraSettings camSettings;

    [System.Serializable]
    public class CameraInputSettings
    {
        public string MouseX_Axis = "Mouse X";
        public string MouseY_Axis = "Mouse Y";
        public string AimInput = "Fire2";
        public string FireInput = "Fire1";
    }

    [SerializeField]
    public CameraInputSettings inputSettings;

    Transform center;
    Transform target;

    public Camera mainCam;

    private float camXrotation = 0;
    private float camYrotation = 0;

    [HideInInspector] public Vector3 mainCamOriginalPos;

    public GameObject playerObject;

    private float reverseElapsedTime;

    private float originalCamSpeed;

    [HideInInspector] public bool buttonPressed;

    private float pressTime;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //mainCam = Camera.main;
        center = this.gameObject.transform.GetChild(0);
        target = playerObject.transform;

        mainCamOriginalPos = mainCam.transform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;

        originalCamSpeed = camSettings.moveSpeed;
    }

    private void Update()
    {
        if (!target)
            return;

        //RotateCam();

        ReverseCam();

        BoostFOV();

        AirTime();

        mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, mainCamOriginalPos, 10.0f * Time.deltaTime);        
    }

    private void LateUpdate()
    {
        if (!target)
            return;

        FollowPlayer();
    }

    public void FollowPlayer()
    {
        Vector3 moveVector = Vector3.Lerp(this.gameObject.transform.position, target.position, camSettings.moveSpeed * Time.deltaTime);

        transform.position = moveVector;

        Quaternion rotationVector = Quaternion.Lerp(this.gameObject.transform.rotation, target.rotation, camSettings.moveSpeed * Time.deltaTime);

        transform.rotation = rotationVector;

        if (!playerObject.GetComponent<CrashFeedback>().bounceBack)
        {
            center.localPosition = Vector3.Lerp(center.localPosition, Vector3.zero, 10.0f * Time.deltaTime);

            Quaternion originalRot = Quaternion.Euler(0, 0, 0);

            if(!playerObject.GetComponent<KartController>().isReversing)
            center.localRotation = Quaternion.Lerp(center.localRotation, originalRot, 10.0f * Time.deltaTime);

        }
        else //Crash Cam
        {
            center.position = Vector3.Lerp(center.position, KartCollisionDetector.instance.crashPoint, 10.0f * Time.deltaTime);

            Quaternion targetCrashRotation = Quaternion.Euler(center.localPosition.x, center.rotation.y, center.localPosition.z);

            center.localRotation = Quaternion.Lerp(center.localRotation, targetCrashRotation,  100.0f * (playerObject.GetComponent<KartController>().currentSpeed / playerObject.GetComponent<KartController>().maxSpeed) * Time.deltaTime);

            CameraShake.instance.crashShaking = true;
        }

    }

    public void BoostFOV()
    {
        if (playerObject.GetComponent<KartController>().isInitialBoosting == true)
        {
            //if (!buttonPressed)
            //{
                mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, camSettings.boostFOV, 1 * Time.deltaTime);
                //pressTime = 0;
            //}
            //else
            //{
            //    mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, camSettings.originalFOV, 1.5f * Time.deltaTime);

            //    pressTime += 1.0f * Time.deltaTime;

            //    if(pressTime > 0.5f)
            //    {
            //        buttonPressed = false;
            //    }
            //}
            
        }
        else if(playerObject.GetComponent<KartController>().isFinalBoosting == true)
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, camSettings.megaBoostFOV, camSettings.MegaBoostZoomSpeed * Time.deltaTime);
        }
        else
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, camSettings.originalFOV, camSettings.boostZoomSpeed * Time.deltaTime);
        }
    }

    //public void RotateCam()
    //{
    //    camXrotation -= Input.GetAxis(inputSettings.MouseY_Axis) * camSettings.mouseY_Sensitivity;
    //    camYrotation += Input.GetAxis(inputSettings.MouseX_Axis) * camSettings.mouseX_Sensitivity;

    //    camXrotation = Mathf.Clamp(camXrotation, camSettings.minVerticalClampAngle, camSettings.maxVerticalClampAngle);

    //    //camYrotation = Mathf.Repeat(camYrotation, 360);

    //    camYrotation = Mathf.Clamp(camYrotation, camSettings.minHorizontalClampAngle, camSettings.maxHorizontalClampAngle);

    //    Vector3 rotatingAngle = new Vector3(camXrotation, camYrotation, 0);

    //    Quaternion rotation = Quaternion.Slerp(center.transform.localRotation, Quaternion.Euler(rotatingAngle), camSettings.rotationSpeed * Time.deltaTime);

    //    center.transform.localRotation = rotation;
    //}
    
    public void ReverseCam()
    {
        if (playerObject.GetComponent<KartController>().isReversing)
        {
            reverseElapsedTime += 1.0f * Time.deltaTime;

            if(reverseElapsedTime > 2.5f)
                center.localRotation = Quaternion.Slerp(center.localRotation, Quaternion.Euler(0, 180, 0), 3.0f * Time.deltaTime);
        }
        else
        {
            reverseElapsedTime = 0;
            center.localRotation = Quaternion.Slerp(center.localRotation, Quaternion.Euler(0, 0, 0), 6.0f * Time.deltaTime);
        }
    } 

    public void AirTime()
    {
        if (!playerObject.GetComponent<KartController>().touchingGround)
        {
            camSettings.moveSpeed = Mathf.Lerp(camSettings.moveSpeed, camSettings.airTimeCamSpeed, 3.0f * Time.deltaTime);
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, 75, camSettings.boostZoomSpeed * Time.deltaTime);
        }
        else
        {
            camSettings.moveSpeed = Mathf.Lerp(camSettings.moveSpeed, originalCamSpeed, 3.0f * Time.deltaTime);
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, camSettings.originalFOV, camSettings.boostZoomSpeed * Time.deltaTime);
        }
    }
}

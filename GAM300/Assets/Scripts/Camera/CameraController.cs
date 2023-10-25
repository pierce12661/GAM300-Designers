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

        Quaternion rotationVector = Quaternion.Lerp(this.gameObject.transform.rotation, target.rotation, camSettings.moveSpeed * Time.deltaTime);

        transform.position = moveVector;

        transform.rotation = rotationVector;
    }

    public void BoostFOV()
    {
        if (playerObject.GetComponent<KartController>().isInitialBoosting == true)
        {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, camSettings.boostFOV, 2 * Time.deltaTime);
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

    public void RotateCam()
    {
        camXrotation -= Input.GetAxis(inputSettings.MouseY_Axis) * camSettings.mouseY_Sensitivity;
        camYrotation += Input.GetAxis(inputSettings.MouseX_Axis) * camSettings.mouseX_Sensitivity;

        camXrotation = Mathf.Clamp(camXrotation, camSettings.minVerticalClampAngle, camSettings.maxVerticalClampAngle);

        //camYrotation = Mathf.Repeat(camYrotation, 360);

        camYrotation = Mathf.Clamp(camYrotation, camSettings.minHorizontalClampAngle, camSettings.maxHorizontalClampAngle);

        Vector3 rotatingAngle = new Vector3(camXrotation, camYrotation, 0);

        Quaternion rotation = Quaternion.Slerp(center.transform.localRotation, Quaternion.Euler(rotatingAngle), camSettings.rotationSpeed * Time.deltaTime);

        center.transform.localRotation = rotation;

        
    }
    
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

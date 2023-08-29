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
        public float zoomSpeed;
        public float moveSpeed;
        public float rotationSpeed;
        public float originalFOV;
        public float zoomFOV;
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
    private float shakeStrength;

    private Vector3 mainCamOriginalPos;

    public GameObject playerObject;

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
    }

    private void Update()
    {
        if (!target)
            return;

        //RotateCam();

        mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, mainCamOriginalPos, 20.0f * Time.deltaTime);
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
}

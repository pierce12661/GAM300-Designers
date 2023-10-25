using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject playerObj;


    [HideInInspector] public bool camShaking;
    [HideInInspector] public bool boostShaking;

    private float speedShake;
    private float crashShake;
    private float boostShakeStr;

    private float elapsedTime;
    private float crashElapsedTime;

    private Quaternion targetRotation;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        targetRotation = Quaternion.Euler(7, cam.transform.localRotation.y, cam.transform.localRotation.z);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) //for testing purposes
        {
            camShaking = true;
        }

        crashShake = playerObj.GetComponent<KartController>().realSpeed / 17;
        boostShakeStr = playerObj.GetComponent<KartController>().realSpeed / 30;

        HighSpeedSmallShake(speedShake, cam.transform); //when at high speeds, will have slight constant shake
        CameraShakeRotation(crashShake, 0.4f, cam.transform); //When crashing, shakes rotations a little bit.
        BoostShake(boostShakeStr, 0.3f, cam.transform);
    }

    private void LateUpdate()
    {
        Quaternion moveVector = Quaternion.Lerp(cam.transform.localRotation, targetRotation, CameraController.instance.camSettings.moveSpeed * Time.deltaTime);

        cam.transform.localRotation = moveVector;
    }

    public void CameraShakeRotation(float strength, float shakeTimer,Transform obj)
    {
        if (camShaking)
        {
            if (crashElapsedTime < shakeTimer)
            {
                crashElapsedTime += 1.0f * Time.deltaTime;

                Vector3 originalRot = obj.localEulerAngles;

                obj.localEulerAngles = originalRot + Random.insideUnitSphere * strength;
            }
            else
            {
                camShaking = false;
            }
        }
        else
        {
            crashElapsedTime = 0;
        }
    }

    public void BoostShake(float strength, float shakeTimer, Transform obj)
    {
        if (boostShaking)
        {
            if (elapsedTime < shakeTimer)
            {
                elapsedTime += 1.0f * Time.deltaTime;

                Vector3 originalRot = obj.localEulerAngles;

                obj.localEulerAngles = originalRot + Random.insideUnitSphere * strength;
            }
            else
            {
                boostShaking = false;
            }
        }
        else
        {
            elapsedTime = 0;
        }
    }

    public void HighSpeedSmallShake(float strength, Transform obj)
    {
        if(playerObj.GetComponent<KartController>().realSpeed > 23)
        {
            speedShake = Mathf.Lerp(speedShake, playerObj.GetComponent<KartController>().realSpeed / 650, 0.9f * Time.deltaTime);

            Vector3 originalRot = obj.localEulerAngles;

            obj.localEulerAngles = originalRot + Random.insideUnitSphere * strength;
        }
    }
    public void BoostShake()
    {
        boostShaking = true;
    }
}

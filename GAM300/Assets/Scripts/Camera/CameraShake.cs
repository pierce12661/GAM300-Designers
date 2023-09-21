using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject playerObj;


    [HideInInspector] public bool camShaking;

    private float speedShake;
    private float crashShake;

    private float elapsedTime;

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
        if (Input.GetKeyDown(KeyCode.J)) //for testing purposes
        {
            camShaking = true;
        }

        crashShake = playerObj.GetComponent<KartController>().realSpeed / 35;

        HighSpeedSmallShake(speedShake, cam.transform); //when at high speeds, will have slight constant shake
        CameraShakeRotation(crashShake, crashShake / 2, cam.transform); //When crashing, shakes rotations a little bit.
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
            if (elapsedTime < shakeTimer)
            {
                elapsedTime += 1.0f * Time.deltaTime;

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
        camShaking = true;
    }
}

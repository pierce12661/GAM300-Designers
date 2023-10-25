using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public KartController playerKart;

    [SerializeField] private GameObject kartObj;

    private float duration = 1.5f;
    private float elapsedTime;

    Quaternion sourceOrientation;
    float sourceAngle;
    Vector3 sourceAxis;
    float targetAngle;

    private void Start()
    {
        playerKart = GameObject.FindGameObjectWithTag("Player").GetComponent<KartController>();
        kartObj = GameObject.FindGameObjectWithTag("KartTag");

        StartOrientation();
    }

    private void Update()
    {
        SpinCar();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Sphere")
        {
            KartCollisionDetector.instance.isSpinning = true;
        }
    }

    public void SpinCar()
    {
        if (KartCollisionDetector.instance.isSpinning && elapsedTime < duration)
        {
            elapsedTime += 1.0f * Time.deltaTime;
            float progress = elapsedTime / duration;

            float currentAngle = Mathf.Lerp(sourceAngle, targetAngle, progress);

            kartObj.transform.rotation = Quaternion.AngleAxis(currentAngle, sourceAxis);

            if (playerKart.currentSpeed < playerKart.slowSpeed)
            {
                playerKart.currentSpeed = Mathf.Lerp(playerKart.currentSpeed, 0, 0.7f * Time.deltaTime);
            }
            else
            {
                playerKart.currentSpeed = Mathf.Lerp(playerKart.currentSpeed, playerKart.slowSpeed, 1.2f * Time.deltaTime);
            }
        }
        else
        {
            KartCollisionDetector.instance.isSpinning = false;
            elapsedTime = 0;

            Quaternion originalRot = Quaternion.Euler(0, -90, 0);

            kartObj.transform.localRotation = Quaternion.Lerp(kartObj.transform.localRotation, originalRot, 1.5f * Time.deltaTime);
        }
    }

    public void StartOrientation()
    {
        sourceOrientation = kartObj.transform.rotation;
        sourceOrientation.ToAngleAxis(out sourceAngle, out sourceAxis);

        targetAngle = 755f + sourceAngle;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesScript : MonoBehaviour
{
    public KartController kc;

    [Header("Particles")]
    [SerializeField] private Transform speedParticlesHolder_1;
    [SerializeField] private Transform speedParticlesHolder_2;
    [SerializeField] private GameObject blastParticlesHolder;
    [SerializeField] private GameObject blastParticlesHolder2;
    [SerializeField] private Transform FlameLeft, FlameRight;
    [SerializeField] private Transform SmokeLeft, SmokeRight;
    [SerializeField] private GameObject crashParticles;

    [SerializeField] private GameObject stunnedObject;

    [HideInInspector] public float boostParticleTimer;

    private float particleTimer;

    private bool crashCheck;


    private Vector3 boostSize = new Vector3(1.3f, 1.3f, 1.3f);
    private Vector3 initialBoostSize = new Vector3(0.45f, 0.45f, 0.45f);
    private Vector3 originalSize = new Vector3(0.3f, 0.3f, 0.3f);

    private void Update()
    {
        boostSize = new Vector3(0.7f + kc.batteryPercentage/1.15f, 0.7f + kc.batteryPercentage / 1.15f, 0.7f + kc.batteryPercentage / 1.15f);

        BoostParticles();
        SpeedParticles();
        StunnedAnim();
        CrashParticles();
    }

    public void BoostParticles()
    {
        if (kc.isInitialBoosting && !KartCollisionDetector.instance.hasCrashed)
        {
            BlastParticles();

            //exhaustParticles.transform.localScale = Vector3.Lerp(exhaustParticles.transform.localScale, Vector3.one, 20.0f * Time.deltaTime);

            FlameLeft.localScale = Vector3.Lerp(FlameLeft.localScale, initialBoostSize, 20.0f * Time.deltaTime);
            FlameRight.localScale = Vector3.Lerp(FlameRight.localScale, initialBoostSize, 20.0f * Time.deltaTime);
            SmokeLeft.localScale = Vector3.Lerp(SmokeLeft.localScale, initialBoostSize, 20.0f * Time.deltaTime);
            SmokeRight.localScale = Vector3.Lerp(SmokeRight.localScale, initialBoostSize, 20.0f * Time.deltaTime);

            boostParticleTimer = 0;
        }
        else if (kc.isFinalBoosting && !KartCollisionDetector.instance.hasCrashed)
        {
            blastParticlesHolder2.SetActive(true);
            blastParticlesHolder.SetActive(true);

            FlameLeft.localScale = Vector3.Lerp(FlameLeft.localScale, boostSize, 20.0f * Time.deltaTime);
            FlameRight.localScale = Vector3.Lerp(FlameRight.localScale, boostSize, 20.0f * Time.deltaTime);
            SmokeLeft.localScale = Vector3.Lerp(SmokeLeft.localScale, boostSize, 20.0f * Time.deltaTime);
            SmokeRight.localScale = Vector3.Lerp(SmokeRight.localScale, boostSize, 20.0f * Time.deltaTime);

            boostParticleTimer = 0;
        }
        else if (kc.acceleration > 0 && !KartCollisionDetector.instance.hasCrashed)
        {
            boostParticleTimer += 1.0f * Time.deltaTime;

            if(boostParticleTimer < 1f)
            {
                blastParticlesHolder.SetActive(true);
            }
            else
            {
                blastParticlesHolder.SetActive(false);
            }
            
            //exhaustParticles.transform.localScale = Vector3.Lerp(exhaustParticles.transform.localScale, Vector3.zero, 10.0f * Time.deltaTime);
            FlameLeft.localScale = Vector3.Lerp(FlameLeft.localScale, (kc.currentSpeed / kc.maxSpeed + 0.2f) * originalSize, 5.0f * Time.deltaTime);
            FlameRight.localScale = Vector3.Lerp(FlameRight.localScale, (kc.currentSpeed / kc.maxSpeed + 0.2f) * originalSize, 5.0f * Time.deltaTime);
            SmokeLeft.localScale = Vector3.Lerp(SmokeLeft.localScale, (kc.currentSpeed / kc.maxSpeed + 0.8f) * originalSize, 5.0f * Time.deltaTime);
            SmokeRight.localScale = Vector3.Lerp(SmokeRight.localScale, (kc.currentSpeed / kc.maxSpeed + 0.8f) * originalSize, 5.0f * Time.deltaTime);

            blastParticlesHolder2.SetActive(false); //Deactivates all exhaust blastParticles
            //blastParticlesHolder.SetActive(false);

        }
        else
        {
            //boostParticleTimer += 1.0f * Time.deltaTime;

            FlameLeft.localScale = Vector3.Lerp(FlameLeft.localScale, Vector3.zero, 5.0f * Time.deltaTime);
            FlameRight.localScale = Vector3.Lerp(FlameRight.localScale, Vector3.zero, 5.0f * Time.deltaTime);
            SmokeLeft.localScale = Vector3.Lerp(SmokeLeft.localScale, Vector3.zero, 5.0f * Time.deltaTime);
            SmokeRight.localScale = Vector3.Lerp(SmokeRight.localScale, Vector3.zero, 5.0f * Time.deltaTime);

            blastParticlesHolder2.SetActive(false);
            blastParticlesHolder.SetActive(false);

            boostParticleTimer = 0;
        }
    }

    public void SpeedParticles()
    {
        float percentage = kc.currentSpeed / kc.maxSpeed;

        Vector3 particleScale = new Vector3(percentage + 0.6f, percentage + 0.6f, percentage + 0.6f);

        speedParticlesHolder_1.localScale = Vector3.Lerp(speedParticlesHolder_1.localScale, particleScale, 4.0f * Time.deltaTime);

        if (kc.realSpeed > 20)
        {
            particleTimer += 1.0f * Time.deltaTime;

            if (particleTimer > 4 || kc.isFinalBoosting)
            {
                speedParticlesHolder_2.localScale = Vector3.Lerp(speedParticlesHolder_2.localScale, Vector3.one, 1.0f * Time.deltaTime);
            }
        }
        else
        {
            //speedParticlesHolder_1.localScale = Vector3.Lerp(speedParticlesHolder_1.localScale, Vector3.zero, 4.0f * Time.deltaTime);
            speedParticlesHolder_2.localScale = Vector3.Lerp(speedParticlesHolder_2.localScale, Vector3.zero, 1.0f * Time.deltaTime);

            particleTimer = 0;
        }
    }

    public void BlastParticles()
    {
        if (kc.currentBattery / kc.maxBattery >= 0.2f && kc.currentBattery / kc.maxBattery <= 0.4f) //Activates first Blast
        {
            blastParticlesHolder.SetActive(true); //activate particles
        }
        else if (kc.currentBattery / kc.maxBattery > 0.4f && kc.currentBattery / kc.maxBattery <= 0.45f) //Deactivates first blast
        {
            blastParticlesHolder.SetActive(false);
        }
        else if(kc.currentBattery / kc.maxBattery > 0.5f && kc.currentBattery / kc.maxBattery <= 0.6f) //Activates second Blast
        {
            blastParticlesHolder.SetActive(true);
        }
        else if(kc.currentBattery / kc.maxBattery > 0.6f && kc.currentBattery / kc.maxBattery <= 0.7f) //Deactivates second blast
        {
            blastParticlesHolder.SetActive(false);
        }
        else if(kc.currentBattery / kc.maxBattery > 0.75f) //Activates third Blast
        {
            blastParticlesHolder.SetActive(true);
        }
    }

    public void StunnedAnim()
    {
        if (kc.stunned)
        {
            stunnedObject.SetActive(true);
        }
        else
        {
            stunnedObject.SetActive(false);
        }
    }

    public void CrashParticles()
    {
        if (KartCollisionDetector.instance.hasCrashed)
        {
            if (!crashCheck) //checking if the collision is with same object, otherwise, it will constantly play
            {
                Vector3 pos = KartCollisionDetector.instance.crashPoint;

                GameObject sparks = Instantiate(crashParticles, pos, Quaternion.identity);

                crashCheck = true;
            }
        }
        else
        {
            crashCheck = false;
        }
    }
}

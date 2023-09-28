using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesScript : MonoBehaviour
{
    public KartController kc;

    [Header("Particles")]
    [SerializeField] private Transform speedParticlesHolder_1;
    [SerializeField] private Transform speedParticlesHolder_2;
    [SerializeField] private GameObject exhaustParticlesHolder;
    [SerializeField] private Transform FlameLeft, FlameRight;
    [SerializeField] private Transform SmokeLeft, SmokeRight;
    [SerializeField] private GameObject crashParticles;

    [SerializeField] private GameObject stunnedObject;

    [HideInInspector] public float boostParticleTimer;

    private float particleTimer;

    private bool crashCheck;

    private void Update()
    {
        BoostParticles();
        SpeedParticles();
        StunnedAnim();
        CrashParticles();
    }

    public void BoostParticles()
    {
        if (kc.isBoosting)
        {
            exhaustParticlesHolder.SetActive(true); //activate particles
            //exhaustParticles.transform.localScale = Vector3.Lerp(exhaustParticles.transform.localScale, Vector3.one, 20.0f * Time.deltaTime);

            FlameLeft.localScale = Vector3.Lerp(FlameLeft.localScale, Vector3.one, 20.0f * Time.deltaTime);
            FlameRight.localScale = Vector3.Lerp(FlameRight.localScale, Vector3.one, 20.0f * Time.deltaTime);
            SmokeLeft.localScale = Vector3.Lerp(SmokeLeft.localScale, Vector3.one, 20.0f * Time.deltaTime);
            SmokeRight.localScale = Vector3.Lerp(SmokeRight.localScale, Vector3.one, 20.0f * Time.deltaTime);

            boostParticleTimer = 0;
        }
        else
        {
            //exhaustParticles.transform.localScale = Vector3.Lerp(exhaustParticles.transform.localScale, Vector3.zero, 10.0f * Time.deltaTime);

            boostParticleTimer += 1.0f * Time.deltaTime;

            FlameLeft.localScale = Vector3.Lerp(FlameLeft.localScale, Vector3.zero, 5.0f * Time.deltaTime);
            FlameRight.localScale = Vector3.Lerp(FlameRight.localScale, Vector3.zero, 5.0f * Time.deltaTime);
            SmokeLeft.localScale = Vector3.Lerp(SmokeLeft.localScale, Vector3.zero, 5.0f * Time.deltaTime);
            SmokeRight.localScale = Vector3.Lerp(SmokeRight.localScale, Vector3.zero, 5.0f * Time.deltaTime);

            if (boostParticleTimer > 3f)
            {
                exhaustParticlesHolder.SetActive(false);
            }
        }
    }

    public void SpeedParticles()
    {
        if (kc.realSpeed > 25)
        {
            particleTimer += 1.0f * Time.deltaTime;

            if (particleTimer > 10)
            {
                speedParticlesHolder_2.localScale = Vector3.Lerp(speedParticlesHolder_2.localScale, Vector3.one, 1.0f * Time.deltaTime);
            }
            else if (particleTimer > 4)
            {
                speedParticlesHolder_1.localScale = Vector3.Lerp(speedParticlesHolder_1.localScale, Vector3.one, 2.0f * Time.deltaTime);
            }
        }
        else
        {
            speedParticlesHolder_1.localScale = Vector3.Lerp(speedParticlesHolder_1.localScale, Vector3.zero, 4.0f * Time.deltaTime);
            speedParticlesHolder_2.localScale = Vector3.Lerp(speedParticlesHolder_2.localScale, Vector3.zero, 1.0f * Time.deltaTime);

            particleTimer = 0;
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

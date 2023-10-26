using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashFeedback : MonoBehaviour
{
    private KartController kc;

    private float forceTime;

    private float multiplier;

    [HideInInspector] public bool bounceBack = false;

    private void Start()
    {
        kc = GameObject.FindGameObjectWithTag("Player").GetComponent<KartController>();
    }

    private void Update()
    {
        BoolCheck();

        CrashBounceBack();
    }

    public void CrashBounceBack()
    {
        if (bounceBack)
        {
            if (forceTime < 0.2f)
            {
                forceTime += 1.0f * Time.deltaTime;

                //Vector3 direction = kc.sphere.transform.position - KartCollisionDetector.instance.crashPoint;

                //Debug.Log("forceTime working");

                //if (kc.realSpeed < 12)
                //{
                //    multiplier = 10;
                //}
                //else
                //{
                //    multiplier = 5;
                //}

                //kc.sphere.AddForce(direction * multiplier, ForceMode.Acceleration);
            }
            else
            {
                forceTime = 0;
                bounceBack = false;
            }
        }
    }

    public void BoolCheck()
    {
        if (KartCollisionDetector.instance.wallCrash)
        {
            if(kc.currentSpeed > 0f)
            {
                kc.currentSpeed -= 3 * (kc.currentSpeed / kc.maxSpeed);
            }

            bounceBack = true;
        }
    }
}

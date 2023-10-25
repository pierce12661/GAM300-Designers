using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartCollisionDetector : MonoBehaviour
{
    public static KartCollisionDetector instance;

    [HideInInspector] public bool hasCrashed;
    [HideInInspector] public bool wallCrash;
    [HideInInspector] public bool isSpinning;
    [HideInInspector] public Vector3 crashPoint;


    private void Awake()
    {
        instance = this;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Road" && collision.gameObject.layer != 6) //if not road or kart
        {
            Debug.Log("Car has crashed");

            Debug.Log(collision.GetContact(0).point);

            crashPoint = collision.GetContact(0).point;

            hasCrashed = true;

            if(collision.gameObject.tag != "Trap")
            {
                wallCrash = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Road" && collision.gameObject.layer != 6)
        {
            Debug.Log("Car exited");

            hasCrashed = false;
            wallCrash = false;
        }
    }
}

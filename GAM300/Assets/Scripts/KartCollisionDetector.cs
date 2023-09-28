using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartCollisionDetector : MonoBehaviour
{
    public static KartCollisionDetector instance;

    [HideInInspector] public bool hasCrashed;
    [HideInInspector] public Vector3 crashPoint;


    private void Awake()
    {
        instance = this;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Road" && collision.gameObject.layer != 6)
        {
            Debug.Log("Car has crashed");

            Debug.Log(collision.GetContact(0).point);

            crashPoint = collision.GetContact(0).point;

            hasCrashed = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Road" && collision.gameObject.layer != 6)
        {
            Debug.Log("Car exited");

            hasCrashed = false;
        }
    }
}

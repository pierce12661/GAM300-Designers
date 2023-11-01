using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartCollisionDetector : MonoBehaviour
{
    public static KartCollisionDetector instance;
    private KartController kc;

    [HideInInspector] public bool hasCrashed;
    [HideInInspector] public bool wallCrash;
    [HideInInspector] public bool isSpinning;
    [HideInInspector] public Vector3 crashPoint;
    [HideInInspector] public Vector3 singlePoint;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        kc = GameObject.FindGameObjectWithTag("Player").GetComponent<KartController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Road" && collision.gameObject.layer != 6) //if not road or kart
        {
            crashPoint = collision.GetContact(0).point;

            hasCrashed = true;

            if(collision.gameObject.tag != "Trap")
            {
                wallCrash = true;
                Debug.Log("Car has crashed");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Road" && collision.gameObject.layer != 6)
        {
            hasCrashed = false;
            wallCrash = false;
        }
    }

    public void Respawn()
    {
        if (RespawnManager.instance) RespawnManager.instance.Respawn(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        RespawnManager.instance.GetRespawnPoint();
        if (other.CompareTag("OOB"))
        {
            Debug.Log("Out of Bounds!");
            kc.currentSpeed = 0;
            kc.transform.LookAt(other.transform);
            Respawn();

            foreach(GameObject obj in kc.GetComponent<Grapple>().deactivatedAnchors)
            {
                obj.SetActive(true);
            }
        }
    }
}

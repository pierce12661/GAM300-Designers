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

    public Transform test;

    private bool onGrass;

    public GameObject traps;

    private bool voCheck;

    private void Awake()
    {
        instance = this;
    }

    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.G))
    //    {

    //        RespawnManager.instance.SetRespawnPoint(test.position, test.rotation) ;
    //        Respawn();
    //    }
    //}

    private void Update()
    {
        VoiceOverChecker();
    }

    private void Start()
    {
        kc = GameObject.FindGameObjectWithTag("Player").GetComponent<KartController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Road" && collision.gameObject.layer != 6 && collision.gameObject.tag != "Grass") //if not road or kart
        {
            crashPoint = collision.GetContact(0).point;

            hasCrashed = true;

            if(collision.gameObject.tag != "Trap")
            {
                wallCrash = true;
                Debug.Log("Car has crashed");

                AudioManager.instance.PlayCrash();

                if (voCheck)
                {
                    int randomChance;

                    randomChance = Random.Range(0, 10);

                    if(randomChance % 2 == 0)
                    {
                        AudioManager.instance.NegativeVO();
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Road" && collision.gameObject.layer != 6 && collision.gameObject.tag != "Grass")
        {
            hasCrashed = false;
            wallCrash = false;
        }
    }

    public void Respawn()
    {
        if (RespawnManager.instance)
        {
            RespawnManager.instance.Respawn(transform);
        }

        foreach (GameObject obj in kc.GetComponent<Grapple>().deactivatedAnchors)
        {
            obj.SetActive(true);
        }
    }

    public void VoiceOverChecker()
    {
        if(kc.currentSpeed > 26f)
        {
            voCheck = true;
        }
        else
        {
            voCheck = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //RespawnManager.instance.GetRespawnPoint();
        if (other.CompareTag("OOB"))
        {
            Debug.Log("Out of Bounds!");
            kc.currentSpeed = 0;
            kc.transform.LookAt(other.transform);
            Respawn();
            AudioManager.instance.PlayFallOutMap();
        }
    }

    public void ToggleTraps()
    {
        if (traps.activeInHierarchy)
        {
            traps.SetActive(false);
        }
        else
        {
            traps.SetActive(true);
        }
    }
}

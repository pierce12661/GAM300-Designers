using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    public static Traps instance;

    public float trapID;

    [HideInInspector] public bool trapHit;
    [HideInInspector] public bool stunned;

    [SerializeField] private GameObject playerObject;

    private float elapsedTime;
    private float stunnedTime;
    private float forceTime;

    private float multiplier;


    Vector3 spinRotation;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        SlowTrap(playerObject);

        PendulumTrap(playerObject);

        //spinRotation = Quaternion.ei(playerObject.transform.rotation.x, playerObject.transform.rotation.y + 2f, playerObject.transform.rotation.z);

        WallTrap();

        //Debug.Log(spinRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player has entered");

        trapHit = true;
    }

    private void OnTriggerExit(Collider other)
    {
        trapHit = false;

        Debug.Log("Player has exit");
    }

    private void OnCollisionEnter(Collision collision)
    {
        trapHit = true;

        //playerObject.GetComponent<KartController>().sphere.AddExplosionForce(50000.0f, collision.GetContact(0).point, 2);


    }
    private void OnCollisionExit(Collision collision)
    {

    }

    public void SlowTrap(GameObject player)
    {
        if (trapID == 0)
        {
            if (trapHit)
            {
                elapsedTime = Mathf.Lerp(elapsedTime, 5.0f, 2.5f * Time.deltaTime);

                if (player.GetComponent<KartController>().currentSpeed < player.GetComponent<KartController>().slowSpeed)
                {
                    return;
                }
                else
                {
                    player.GetComponent<KartController>().currentSpeed =
                    Mathf.Lerp(player.GetComponent<KartController>().currentSpeed, player.GetComponent<KartController>().slowSpeed, elapsedTime * Time.deltaTime);

                    Debug.Log("speed = " + player.GetComponent<KartController>().currentSpeed);
                }
            }
            else
            {
                elapsedTime = 1.0f;
            }
        }
    }

    public void PendulumTrap(GameObject player)
    {
        if (trapID == 1 && trapHit)
        {
            if (forceTime < 0.15f)
            {
                forceTime += 1.0f * Time.deltaTime;

                //player.GetComponent<KartController>().sphere.AddExplosionForce(50000.0f, col.GetContact(0).point, 2);
                Vector3 direction = player.GetComponent<KartController>().sphere.position - transform.position;

                player.GetComponent<KartController>().sphere.AddForce(direction * 125, ForceMode.Acceleration);
                stunned = true;
            }
            else
            {
                trapHit = false;
                forceTime = 0;
            }
            
        }

        if (stunned)
        {
            stunnedTime += 1.0f * Time.deltaTime;

            //playerObject.transform.rotation = Quaternion.Lerp(playerObject.transform.rotation, spinRotation, 3.0f * Time.deltaTime);

            Debug.Log("isStunned");

            if (stunnedTime > 2.5f)
            {
                stunned = false;
                stunnedTime = 0;
                Debug.Log("offStun");
            }
        }
    }

    public void WallTrap()
    {
        if(trapID == 2 && trapHit)
        {
            if(forceTime < 0.15f)
            {
                forceTime += 1.0f * Time.deltaTime;

                Vector3 direction = playerObject.transform.forward;

                playerObject.GetComponent<KartController>().currentSpeed = -5;

                if (playerObject.GetComponent<KartController>().realSpeed < 12)
                {
                    multiplier = 70f;
                }
                else
                {
                    multiplier = 120f;
                }

                playerObject.GetComponent<KartController>().sphere.AddForce(-direction * multiplier, ForceMode.Acceleration);
            }
            else
            {
                trapHit = false;
                forceTime = 0;
            }
        }
    }

}

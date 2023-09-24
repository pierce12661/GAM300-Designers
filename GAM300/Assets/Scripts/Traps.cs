using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    public float trapID;
    private float IDchecker;

    [SerializeField] private GameObject playerObject;

    private GameObject objectCollided;

    private float elapsedTime;
    private float stunnedTime;
    private float forceTime;

    private float multiplier;


    Vector3 spinRotation;

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
        if(this.gameObject.GetComponent<Traps>().trapID == 0)
        {
            Debug.Log("Player has entered");

            IDchecker = this.gameObject.GetComponent<Traps>().trapID; //Switches the ID to the current trapID of the colliding trap

            playerObject.GetComponent<KartController>().trapHit = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (this.gameObject.GetComponent<Traps>().trapID == 0)
        {
            playerObject.GetComponent<KartController>().trapHit = false;

            Debug.Log("Player has exit");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDchecker = this.gameObject.GetComponent<Traps>().trapID; //Switches the ID to the current trapID of the colliding trap

        playerObject.GetComponent<KartController>().trapHit = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        
    }

    public void SlowTrap(GameObject player)
    {
        if (IDchecker == 0)
        {
            if (playerObject.GetComponent<KartController>().trapHit)
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
        if (IDchecker == 1 && playerObject.GetComponent<KartController>().trapHit)
        {
            if (forceTime < 0.15f)
            {
                forceTime += 1.0f * Time.deltaTime;

                //player.GetComponent<KartController>().sphere.AddExplosionForce(50000.0f, col.GetContact(0).point, 2);
                Vector3 direction = player.GetComponent<KartController>().sphere.position - transform.position;

                player.GetComponent<KartController>().sphere.AddForce(direction * 125, ForceMode.Acceleration);
                playerObject.GetComponent<KartController>().stunned = true;
            }
            else
            {
                playerObject.GetComponent<KartController>().trapHit = false;
                forceTime = 0;
            }
            
        }

        if (playerObject.GetComponent<KartController>().stunned)
        {
            stunnedTime += 1.0f * Time.deltaTime;

            //playerObject.transform.rotation = Quaternion.Lerp(playerObject.transform.rotation, spinRotation, 3.0f * Time.deltaTime);

            Debug.Log("isStunned");

            if (stunnedTime > 2.5f)
            {
                IDchecker = 0;
                playerObject.GetComponent<KartController>().stunned = false;
                stunnedTime = 0;
                Debug.Log("offStun");
            }
        }
    }

    public void WallTrap()
    {
        if(IDchecker == 2 && playerObject.GetComponent<KartController>().trapHit)
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
                IDchecker = 0;
                playerObject.GetComponent<KartController>().trapHit = false;
                forceTime = 0;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
    public static Traps instance;

    [HideInInspector] public bool trapHit;

    [SerializeField] private GameObject playerObject;

    private float elapsedTime;

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

    public void SlowTrap(GameObject player)
    {
        if (trapHit)
        {
            elapsedTime = Mathf.Lerp(elapsedTime, 5.0f, 2.5f * Time.deltaTime);

            if(player.GetComponent<KartController>().currentSpeed < player.GetComponent<KartController>().slowSpeed)
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

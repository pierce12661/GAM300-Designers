using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private Grapple gs;
    private void Start()
    {
        gs = GameObject.FindGameObjectWithTag("Player").GetComponent<Grapple>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Sphere")
        {
            return;
        }
        else
        {
            Debug.Log("Player passed Checkpoint!");
            RespawnManager.instance.SetRespawnPoint(transform.position);
            gs.deactivatedAnchors.Clear();
        }
    }
}
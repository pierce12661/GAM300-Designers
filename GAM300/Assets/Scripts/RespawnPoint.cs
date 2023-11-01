using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private Grapple gs;
    public Transform point;
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
            RespawnManager.instance.SetRespawnPoint(point.position,point.rotation );
            gs.deactivatedAnchors.Clear();
        }
    }
}
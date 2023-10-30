using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTest : MonoBehaviour
{
    public KartController kc;

    private void Start()
    {
        kc = GameObject.FindGameObjectWithTag("Player").GetComponent<KartController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        kc.currentSpeed = 0;
        kc.RespawnLastLocation();
        kc.transform.LookAt(gameObject.transform.forward);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTest : MonoBehaviour
{
    public KartController kc;

    private void Start()
    {
        kc = GameObject.FindGameObjectWithTag("Player").GetComponent<KartController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        kc.SetRespawnLastLocation(this.transform.position);
    }
}
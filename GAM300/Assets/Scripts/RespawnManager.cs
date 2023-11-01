using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    private KartController kc;

    private Transform player;

    private Vector3 respawnPoint = new Vector3(795f,189f,-119f);
    private Quaternion respawnRotation;

    

    

    public void Awake()
    {
       instance = this; 
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        kc = player.GetComponent<KartController>();
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint, Quaternion newRespawnRotation)
    {
        respawnPoint = newRespawnPoint;
        respawnRotation = newRespawnRotation;

    }

    public Vector3 GetRespawnPoint()
    {
        return respawnPoint;
    }

    public void Respawn(Transform resp)
    {
        //resp.transform.SetPositionAndRotation(respawnPoint, respawnRotation);
        resp.position = respawnPoint;
        player.rotation = respawnRotation;

        PlayerSpeedReset();
    }

    public void PlayerSpeedReset()
    {
        kc.currentSpeed = 0;
        kc.isInitialBoosting = false;
        kc.isFinalBoosting = false;
        kc.currentBattery = 0;
        kc.maxSpeed = kc.originalSpeed;
    }
}
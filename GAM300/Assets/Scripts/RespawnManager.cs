using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager instance;

    private Vector3 respawnPoint = new Vector3(795f,189f,-119f);

    public void Awake()
    {
       instance = this; 
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }

    public Vector3 GetRespawnPoint()
    {
        return respawnPoint;
    }

    public void Respawn(Transform resp)
    {
        resp.transform.SetPositionAndRotation(respawnPoint, Quaternion.identity);
    }
}
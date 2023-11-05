using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public float timeToExtend;

    private bool hasPassed;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "Sphere")
        {
            if (!hasPassed)
            {
                TimeAttack.instance.ExtendTime(timeToExtend);

                hasPassed = true;
            }
            else
            {
                return;
            }
        }
            
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public float timeToExtend;

    private bool hasPassed;

    private void OnTriggerEnter(Collider other)
    {
        ExtendTime();
    }

    public void ExtendTime()
    {
        if (!hasPassed)
        {
            TimeAttack.instance.timeExtension = true;
            TimeAttack.instance.extendedTime = timeToExtend;
            TimeAttack.instance.currentTime += timeToExtend;
            TimeAttack.instance.TimeExtendText();
            hasPassed = true;
        }
        else
        {
            return;
        }
        
    }
}

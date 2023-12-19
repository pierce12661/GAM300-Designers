using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Sphere")
        {
            TransitionManager.instance.WinGame();

        }
    }
}

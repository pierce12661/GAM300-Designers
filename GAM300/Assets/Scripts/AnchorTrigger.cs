using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnchorTrigger : MonoBehaviour
{
    public GameObject kartObject;
    public Grapple grappleScript;
    public GameObject popupText;

    private void Start()
    {
        grappleScript = kartObject.GetComponent<Grapple>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Sphere") popupText.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Sphere") popupText.SetActive(false);
    }
}
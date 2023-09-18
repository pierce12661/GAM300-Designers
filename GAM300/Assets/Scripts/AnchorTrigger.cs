using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnchorTrigger : MonoBehaviour
{
    public GameObject kartObject;
    public Grapple grappleScript;
    public Transform anchors;

    private void Start()
    {
        grappleScript = kartObject.GetComponent<Grapple>();
        anchors = grappleScript.anchors;
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.name);

        if (other.gameObject.name == "Sphere" && anchors == null)
        {
            anchors = GameObject.FindGameObjectWithTag("Anchors").transform;
            grappleScript.anchors = anchors;
        }
        else
        {
            anchors = null;
            grappleScript.anchors = anchors;
        }
    }
}
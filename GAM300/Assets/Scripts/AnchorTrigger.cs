using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnchorTrigger : MonoBehaviour
{
    public GameObject kartObject;
    public Grapple grappleScript;
    //public Transform anchors;

    private void Start()
    {
        grappleScript = kartObject.GetComponent<Grapple>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (grappleScript.anchors == null)
        //{
        //    grappleScript.anchors = grappleScript.closestAnchor.transform;
        //}
    }
    private void OnTriggerExit(Collider other)
    {
        //grappleScript.anchors = null;
    }
}
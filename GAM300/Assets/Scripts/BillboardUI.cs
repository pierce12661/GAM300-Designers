using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    public Transform camTransform;

 
    Quaternion originalRotation;
    private void Start()
    {
        //camTransform = FindObjectOfType<Camera>().transform;
        //originalRotation = transform.rotation * Quaternion.Euler(-1,0,0);

        originalRotation = Quaternion.Euler(0, -1, 0);
    }

    private void Update()
    {

        transform.LookAt(camTransform, Vector3.up);

        transform.rotation = camTransform.rotation * originalRotation;

        //transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0); //camTransform.rotation * originalRotation;
    }
}

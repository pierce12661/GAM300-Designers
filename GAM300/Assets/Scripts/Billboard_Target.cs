using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard_Target : MonoBehaviour
{
    public Transform camTransform;


    Quaternion originalRotation;
    Quaternion offset;

    private Transform playerObj;

    private void Start()
    {
        camTransform = FindObjectOfType<Camera>().transform;
        //originalRotation = transform.rotation * Quaternion.Euler(-1,0,0);

        originalRotation = Quaternion.Euler(0, -1, 0);
        offset = Quaternion.Euler(90, 0, 0);

        playerObj = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {

        transform.LookAt(camTransform, Vector3.forward);

        //transform.rotation = camTransform.rotation * originalRotation;

        transform.rotation = Quaternion.Euler(90, playerObj.rotation.y, 0);

        //transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0); //camTransform.rotation * originalRotation;
    }
}

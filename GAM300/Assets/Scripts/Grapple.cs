using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Grapple : MonoBehaviour
{
    [Header("References")]
    private KartController kc;
    public Transform cam;
    public Transform grappleStart;
    public LayerMask canBeGrappled;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public Transform grappleAnchor;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCD;
    private float grapplingCDTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse0;
    public KeyCode grappleRemoveKey = KeyCode.Mouse1;

    private bool grappling;

    // Start is called before the first frame update
    void Start()
    {
        kc = GetComponent<KartController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }

        if (Input.GetKeyDown(grappleRemoveKey))
        {
            StopGrapple();
        }

        // if timer > 0, keep counting down
        if (grapplingCDTimer > 0)
        {
            grapplingCDTimer -= Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        // update position of initial grapple launch to always follow the source from the car
        if (grappling)
        {
            lr.SetPosition(0, grappleStart.position);
        }   
    }

    private void StartGrapple()
    {
        if (grapplingCDTimer > 0)
        {
            return;
        }

        grappling = true;

        // shoot line renderer in straight line from grappler start point to anchor point
        RaycastHit hit;
        if (Vector3.Distance(grappleStart.position, grappleAnchor.position) < maxGrappleDistance)
        {
            if (Physics.Raycast(grappleStart.position, (grappleAnchor.position - grappleStart.position), out hit, maxGrappleDistance, canBeGrappled))
            {
                grapplePoint = hit.point;

                Invoke(nameof(ExecuteGrapple), grappleDelayTime);
            }
            else
            {
                grapplePoint = cam.position + cam.forward * maxGrappleDistance;

                Invoke(nameof(StopGrapple), grappleDelayTime);
            }
            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);
        }
    }

    private void ExecuteGrapple()
    {
        kc.BoostKart();
    }

    private void StopGrapple()
    {
        grappling = false;
        grapplingCDTimer = grapplingCD;

        lr.enabled = false;
    }
}
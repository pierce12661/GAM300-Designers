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
    private float maxSpeed;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;

    public GameObject grapplerObj;
    public GameObject grappleLookAt;
    public Transform anchors;
    //public Transform anchorLeft;
    //public Transform anchorMid;
    //public Transform anchorRight;
    public Transform grappleAnchor;
    public SpringJoint joint;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCD;
    private float grapplingCDTimer;

    [Header("Inputs")]
    public KeyCode grappleKeyLeft = KeyCode.J;
    public KeyCode grappleKeyMid = KeyCode.K;
    public KeyCode grappleKeyRight = KeyCode.L;
    public KeyCode grappleRemoveKey = KeyCode.Space;

    private bool grappling;

    // Start is called before the first frame update
    void Start()
    {
        kc = GetComponent<KartController>();
    }

    // Update is called once per frame
    void Update()
    {
        #region Grapple Inputs
        if (Input.GetKeyDown(grappleKeyLeft) && Vector3.Distance(grappleStart.position, anchors.position) < maxGrappleDistance)
        {
            grappleAnchor = GameObject.FindGameObjectWithTag("LeftAnchor").transform;
            StartGrappleAnchor();
        }
        if (Input.GetKeyDown(grappleKeyMid) && Vector3.Distance(grappleStart.position, anchors.position) < maxGrappleDistance)
        {
            grappleAnchor = GameObject.FindGameObjectWithTag("MidAnchor").transform;
            StartGrappleBoost();
        }

        if (Input.GetKeyDown(grappleKeyRight) && Vector3.Distance(grappleStart.position, anchors.position) < maxGrappleDistance)
        {
            grappleAnchor = GameObject.FindGameObjectWithTag("RightAnchor").transform;
            StartGrappleAnchor();
        }

        maxSpeed = kc.GetMaxSpeed();
        if (maxSpeed > 30f && maxSpeed < 33f)
        {
            StopGrapple();
        }

        if (Input.GetKeyDown(grappleRemoveKey))
        {
            StopAnchor();
        }
        #endregion

        // if timer > 0, keep counting down
        if (grapplingCDTimer > 0)
        {
            grapplingCDTimer -= Time.deltaTime;
        }

        //Debug.Log("Max distance: " + joint.maxDistance);
        //Debug.Log("Min distance: " + joint.minDistance);
    }

    void LateUpdate()
    {
        if (grappling && grappleAnchor != null)
        {
            // update position of initial grapple launch to always follow the source grappler from the car
            lr.SetPosition(0, grappleStart.position);

            grapplerObj.transform.LookAt(grappleAnchor.position);
        }
        else grapplerObj.transform.LookAt(grappleLookAt.transform);
    }

    private void StartGrappleBoost()
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

                Invoke(nameof(GrappleBoost), grappleDelayTime);
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

    private void StartGrappleAnchor()
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

                Invoke(nameof(GrappleAnchor), grappleDelayTime);
            }
            else
            {
                grapplePoint = cam.position + cam.forward * maxGrappleDistance;

                Invoke(nameof(StopAnchor), grappleDelayTime);
            }
            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);
        }
    }

    private void GrappleAnchor()
    {
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(grappleStart.position, grappleAnchor.position);

        // distance that grapple will try to keep from grapple point
        joint.maxDistance = distanceFromPoint * 0.1f;
        joint.minDistance = distanceFromPoint * 0.05f;

        // values
        joint.spring = 1f;
        joint.damper = 2f;
        joint.massScale = 2.5f;
    }

    private void GrappleBoost()
    {
        kc.BoostKart();
    }

    private void StopGrapple()
    {
        grappling = false;
        grapplingCDTimer = grapplingCD;
        grappleAnchor = null;

        lr.enabled = false;
    }

    private void StopAnchor()
    {
        Destroy(joint);
        Destroy(gameObject.GetComponent<Rigidbody>());

        grappling = false;
        grapplingCDTimer = grapplingCD;
        grappleAnchor = null;

        lr.enabled = false;
    }
}
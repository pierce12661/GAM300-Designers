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
    public Transform grappleAnchor;
    public SpringJoint joint;
    public GameObject closestAnchor;

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
        if (anchors != null)
        {
            if (joint == null && Input.GetKeyDown(grappleKeyLeft) && Vector3.Distance(grappleStart.position, closestAnchor.transform.position) < maxGrappleDistance)
            {
                grappleAnchor = FindClosestLeftAnchor().transform;
                StartGrappleAnchor();
                StartGrappleBoost();

                FeedbackHUD.instance.boosted = true;
            }
            if (Input.GetKeyDown(grappleKeyMid) && Vector3.Distance(grappleStart.position, closestAnchor.transform.position) < maxGrappleDistance)
            {
                grappleAnchor = FindClosestMidAnchor().transform;
                StartGrappleBoost();
            }

            if (joint == null && Input.GetKeyDown(grappleKeyRight) && Vector3.Distance(grappleStart.position, closestAnchor.transform.position) < maxGrappleDistance)
            {
                grappleAnchor = FindClosestRightAnchor().transform;
                StartGrappleAnchor();
                StartGrappleBoost();

                FeedbackHUD.instance.boosted = true;
            }
        }
        #endregion

        #region StopAnchor/StopGrapple
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

        closestAnchor = FindClosestAnchor();
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
        if (Vector3.Distance(grappleStart.position, closestAnchor.transform.position) < maxGrappleDistance)
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

        // distance that grapple will try to keep from grapple point, increaing max will increase length of the grapple
        joint.maxDistance = distanceFromPoint * 0.1f;
        joint.minDistance = distanceFromPoint * 0f;

        // values
        joint.spring = 12f;
        joint.damper = 10f;
        joint.massScale = 5f;
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

        joint = null;
        grappling = false;
        grapplingCDTimer = grapplingCD;
        grappleAnchor = null;

        lr.enabled = false;
    }

    public GameObject FindClosestAnchor()
    {
        GameObject[] GOs = GameObject.FindGameObjectsWithTag("Anchors");
        GameObject closest = null;
        float dist = Mathf.Infinity;
        Vector3 pos = transform.position;
        foreach (GameObject GO in GOs)
        {
            Vector3 diff = GO.transform.position - pos;
            float currentDist = diff.sqrMagnitude;
            if (currentDist < dist)
            {
                closest = GO;
                dist = currentDist;
            }
        }
        return closest;
    }
    public GameObject FindClosestLeftAnchor()
    {
        GameObject[] GOs = GameObject.FindGameObjectsWithTag("LeftAnchor");
        GameObject closest = null;
        float dist = Mathf.Infinity;
        Vector3 pos = transform.position;
        foreach (GameObject GO in GOs)
        {
            Vector3 diff = GO.transform.position - pos;
            float currentDist = diff.sqrMagnitude;
            if (currentDist < dist)
            {
                closest = GO;
                dist = currentDist;
            }
        }
        return closest;
    }
    public GameObject FindClosestMidAnchor()
    {
        GameObject[] GOs = GameObject.FindGameObjectsWithTag("MidAnchor");
        GameObject closest = null;
        float dist = Mathf.Infinity;
        Vector3 pos = transform.position;
        foreach (GameObject GO in GOs)
        {
            Vector3 diff = GO.transform.position - pos;
            float currentDist = diff.sqrMagnitude;
            if (currentDist < dist)
            {
                closest = GO;
                dist = currentDist;
            }
        }
        return closest;
    }
    public GameObject FindClosestRightAnchor()
    {
        GameObject[] GOs = GameObject.FindGameObjectsWithTag("RightAnchor");
        GameObject closest = null;
        float dist = Mathf.Infinity;
        Vector3 pos = transform.position;
        foreach (GameObject GO in GOs)
        {
            Vector3 diff = GO.transform.position - pos;
            float currentDist = diff.sqrMagnitude;
            if (currentDist < dist)
            {
                closest = GO;
                dist = currentDist;
            }
        }
        return closest;
    }
}
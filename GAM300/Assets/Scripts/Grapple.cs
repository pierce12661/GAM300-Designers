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
    //public Transform anchors;
    public SpringJoint joint;
    public Transform grappleAnchor;
    public GameObject closestAnchor;
    public bool hasGrappled;
    private Vector3 grapplePoint;
    //public bool grappleStopped;

    [Header("Cooldown")]
    public float grapplingCD;
    private float grapplingCDTimer;

    [Header("Inputs")]
    //public KeyCode grappleKeyLeft = KeyCode.Z;
    //public KeyCode grappleKeyMid = KeyCode.X;
    //public KeyCode grappleKeyRight = KeyCode.C;
    public KeyCode grappleKey;
    public KeyCode grappleRemoveKey;

    private bool grappling;
    private bool sideGrapples;

    // Start is called before the first frame update
    void Start()
    {
        kc = GetComponent<KartController>();
        grappleKey = KeyCode.Space;
        grappleRemoveKey = KeyCode.None;
    }

    // Update is called once per frame
    void Update()
    {
        #region Grapple Inputs
        if (closestAnchor != null)
        {
            if (joint == null && Input.GetKeyDown(grappleKey) && Vector3.Distance (grappleStart.position, closestAnchor.transform.position) < maxGrappleDistance)
            {
                grappleRemoveKey = grappleKey;
                grappleAnchor = FindClosestAnchor().transform;
                StartGrappleAnchor();
                
                FeedbackHUD.instance.boosted = true;
                StartGrappleBoost();
            }
            
            //if (joint == null && Input.GetKeyDown(grappleKeyLeft) && Vector3.Distance(grappleStart.position, closestAnchor.transform.position) < maxGrappleDistance)
            //{
            //    grappleRemoveKey = grappleKeyLeft;
            //    grappleAnchor = FindClosestLeftAnchor().transform;
            //    StartGrappleAnchor();

            //    sideGrapples = true;

            //    StartGrappleBoost();
            //}
            if (Input.GetKeyDown(grappleKey) && Vector3.Distance(grappleStart.position, closestAnchor.transform.position) < maxGrappleDistance)
            {
                grappleRemoveKey = grappleKey;
                grappleAnchor = FindClosestMidAnchor().transform;
                StartGrappleBoost();
            }

            //if (joint == null && Input.GetKeyDown(grappleKeyRight) && Vector3.Distance(grappleStart.position, closestAnchor.transform.position) < maxGrappleDistance)
            //{
            //    grappleRemoveKey = grappleKeyRight;
            //    grappleAnchor = FindClosestRightAnchor().transform;
            //    StartGrappleAnchor();

            //    sideGrapples = true;
                
            //    StartGrappleBoost();
            //}
            
        }
        #endregion

        #region StopAnchor/StopGrapple
        //maxSpeed = kc.GetMaxSpeed();
        //if (maxSpeed > 30f && maxSpeed < 33f)
        //{
        //    StopGrapple();
        //}

        if (Input.GetKeyUp(grappleRemoveKey))
        {
            /*
            if (joint != null)
            {
                StopAnchor();
            }
            else StopGrapple();
            */
            StopAnchor();
        }
        #endregion

        BoostBattery();

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

                Invoke(nameof(InitialGrappleBoost), grappleDelayTime);
            }
            //else
            //{
            //    grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            //    Invoke(nameof(StopGrapple), grappleDelayTime);
            //}
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
            //else
            //{
            //    grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            //    Invoke(nameof(StopAnchor), grappleDelayTime);
            //}
            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);

            CameraShake.instance.BoostShake();
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
        joint.spring = 15f;
        joint.damper = 10f;
        joint.massScale = 5f;
    }

    private void InitialGrappleBoost()
    {
        kc.InitialBoostKart();
    }

    private void FinalGrappleBoost()
    {
        kc.FinalBoostKart();

        if (sideGrapples)
        {
            FeedbackHUD.instance.boosted = true;
            sideGrapples = false;
        }
    }

    private void BoostBattery()
    {
        if (grappling)
        {
            if(kc.currentBattery < kc.maxBattery)
                kc.currentBattery += 2.0f * Time.deltaTime;
        }
        else
        {
            if (kc.currentBattery > 0)
                kc.currentBattery -= 2.0f * Time.deltaTime;
        }
    }
    /*private void StopGrapple()
    {
        grappling = false;
        grapplingCDTimer = grapplingCD;
        grappleAnchor = null;

        lr.enabled = false;
    }*/

    private void StopAnchor()
    {
        Destroy(joint);
        Destroy(gameObject.GetComponent<Rigidbody>());

        joint = null;
        grappling = false;
        grapplingCDTimer = grapplingCD;
        grappleAnchor = null;

        FinalGrappleBoost();

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

    //public GameObject FindClosestLeftAnchor()
    //{
    //    GameObject[] GOs = GameObject.FindGameObjectsWithTag("LeftAnchor");
    //    GameObject closest = null;
    //    float dist = Mathf.Infinity;
    //    Vector3 pos = transform.position;
    //    foreach (GameObject GO in GOs)
    //    {
    //        Vector3 diff = GO.transform.position - pos;
    //        float currentDist = diff.sqrMagnitude;
    //        if (currentDist < dist)
    //        {
    //            closest = GO;
    //            dist = currentDist;
    //        }
    //    }
    //    return closest;
    //}
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
    //public GameObject FindClosestRightAnchor()
    //{
    //    GameObject[] GOs = GameObject.FindGameObjectsWithTag("RightAnchor");
    //    GameObject closest = null;
    //    float dist = Mathf.Infinity;
    //    Vector3 pos = transform.position;
    //    foreach (GameObject GO in GOs)
    //    {
    //        Vector3 diff = GO.transform.position - pos;
    //        float currentDist = diff.sqrMagnitude;
    //        if (currentDist < dist)
    //        {
    //            closest = GO;
    //            dist = currentDist;
    //        }
    //    }
    //    return closest;
    //}

}
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
    public GameObject closestMidAnchor;
    public bool hasGrappled;
    private Vector3 grapplePoint;
    //public bool grappleStopped;

    [Header("Cooldown")]
    public float grapplingCD;
    private float grapplingCDTimer;

    //Destroy prev anchor
    [SerializeField] private float destroyAnchorCountdown;
    private float initialDestroyAnchorCD;
    private bool prevAnchorIsDestroying = false;
    [SerializeField] private float destroyMidAnchorCountdown;
    private float initialDestroyMidAnchorCD;
    private bool prevMidAnchorIsDestroying = false;

    [Header("Inputs")]
    public KeyCode grappleKey;
    public KeyCode grappleRemoveKey;

    private bool grappling;
    private bool midGrappleBoost = false;
    private bool sideGrapples;

    // Start is called before the first frame update
    void Start()
    {
        kc = GetComponent<KartController>();
        initialDestroyAnchorCD = destroyAnchorCountdown;
        initialDestroyAnchorCD = destroyMidAnchorCountdown;

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
                
                if (grappling && grappleAnchor != null)
                {
                    FeedbackHUD.instance.boosted = true;
                    InitialGrappleBoost();
                }
            }
        }
        if (closestMidAnchor != null)
        {
            if (Input.GetKeyDown(grappleKey) && Vector3.Distance(grappleStart.position, closestMidAnchor.transform.position) < maxGrappleDistance)
            {
                grappleRemoveKey = grappleKey;
                grappleAnchor = FindClosestMidAnchor().transform;
                StartGrappleBoost();

                if (grappling && grappleAnchor != null)
                {
                    FeedbackHUD.instance.boosted = true;
                    InitialGrappleBoost();
                }
            }
        }
        #endregion

        #region StopAnchor/StopGrapple
        if (Input.GetKeyUp(grappleRemoveKey))
        {
            if (joint != null && Vector3.Distance (grappleStart.position, closestAnchor.transform.position) < maxGrappleDistance)
            {
                StopAnchor();
                FinalGrappleBoost();
                StartAnchorDestroyCD();
            }

            if (Vector3.Distance(grappleStart.position, closestMidAnchor.transform.position) < maxGrappleDistance)
            {
                StopGrapple();
                FinalGrappleBoost();
                StartMidAnchorDestroyCD();
            }
        }
        #endregion

        BoostBattery();

        // if timer > 0, keep counting down
        if (grapplingCDTimer > 0)
        {
            grapplingCDTimer -= Time.deltaTime;
        }

        if (destroyAnchorCountdown < 0 && prevAnchorIsDestroying)
        {
            prevAnchorIsDestroying = false;
            destroyAnchorCountdown = initialDestroyAnchorCD;
            Destroy(closestAnchor);
        }

        if (destroyMidAnchorCountdown < 0 && prevMidAnchorIsDestroying)
        {
            prevMidAnchorIsDestroying = false;
            destroyMidAnchorCountdown = initialDestroyMidAnchorCD;
            Destroy(closestMidAnchor);
        }

        closestAnchor = FindClosestAnchor();
        closestMidAnchor = FindClosestMidAnchor();
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
        if (Vector3.Distance(grappleStart.position, closestMidAnchor.transform.position) < maxGrappleDistance)
        {
            if (Physics.Raycast(grappleStart.position, (grappleAnchor.position - grappleStart.position), out hit, maxGrappleDistance, canBeGrappled))
            {
                grapplePoint = hit.point;

                //Invoke(nameof(InitialGrappleBoost), grappleDelayTime);
            }

            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);

            CameraShake.instance.BoostShake();

            prevMidAnchorIsDestroying = true;
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

            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);

            CameraShake.instance.BoostShake();
            
            prevAnchorIsDestroying = true;
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
        joint.spring = 30f;
        joint.damper = 15f;
        joint.massScale = 5f;
    }

    private void InitialGrappleBoost()
    {
        kc.InitialBoostKart();
    }

    private void FinalGrappleBoost()
    {
        kc.FinalBoostKart();

        // if (sideGrapples)
        // {
        //     FeedbackHUD.instance.boosted = true;
        //     sideGrapples = false;
        // }
    }

    private void BoostBattery()
    {
        if (grappling)
        {
            if(kc.currentBattery < kc.maxBattery)
                kc.currentBattery += 2f * Time.deltaTime;
        }
        else
        {
            if (kc.currentBattery > 0)
                kc.currentBattery -= 1.5f * Time.deltaTime;
        }
    }
    /*private void StopGrapple()
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

     public void StartAnchorDestroyCD()
    {
        if (prevAnchorIsDestroying) 
        {
            destroyAnchorCountdown -= Time.deltaTime;
        }
    }

    public void StartMidAnchorDestroyCD()
    {
        if (prevMidAnchorIsDestroying)
        {
            destroyMidAnchorCountdown -= Time.deltaTime;
        }
    }
}
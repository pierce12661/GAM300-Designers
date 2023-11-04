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
    public Transform grappleStart;
    public LayerMask canBeGrappled;
    public LineRenderer lr;
    private float maxSpeed;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;

    public GameObject grapplerObj;
    public GameObject grappleLookAt;
    public SpringJoint joint;
    public Transform grappleAnchor;
    public GameObject closestAnchor;
    public GameObject closestMidAnchor;
    public bool hasGrappled;
    private bool isGrapplingMid;
    private Vector3 grapplePoint;

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

    private bool grappling;
    private bool midGrappleBoost = false;
    [HideInInspector] public bool sideGrapples;

    [HideInInspector] public List<GameObject> deactivatedAnchors;

    // Start is called before the first frame update
    void Start()
    {
        kc = GetComponent<KartController>();
        initialDestroyAnchorCD = destroyAnchorCountdown;
        initialDestroyAnchorCD = destroyMidAnchorCountdown;

        grappleKey = KeyCode.Space;
    }

    // Update is called once per frame
    void Update()
    {
        #region Find Closest Anchor
        closestAnchor = FindClosestAnchor();
        closestMidAnchor = FindClosestMidAnchor();
        #endregion

        #region Grapple Inputs
        // Used to grapple to side anchors
        if(!TransitionManager.instance.isGameOver && !TransitionManager.instance.gameWin)
        {
            if (closestAnchor != null)
            {
                //SideAnchors
                if (Input.GetKeyDown(grappleKey) && Vector3.Distance(grappleStart.position, closestAnchor.transform.position) < maxGrappleDistance)
                {
                    grappleAnchor = closestAnchor.transform;
                    StartGrappleAnchor();

                    AudioManager.instance.PlayShootGrappler();


                    if (grappling && grappleAnchor != null)
                    {
                        sideGrapples = true;
                        InitialGrappleBoost();
                        AudioManager.instance.PlayDrift();
                    }
                }
            }
            // Used to grapple to mid anchors
            if (closestMidAnchor != null) //MidAnchor
            {
                if (Input.GetKeyDown(grappleKey) && Vector3.Distance(grappleStart.position, closestMidAnchor.transform.position) < maxGrappleDistance && !TransitionManager.instance.isMainMenu)
                {
                    grappleAnchor = closestMidAnchor.transform;
                    isGrapplingMid = true;
                    StartGrappleBoost();

                    AudioManager.instance.PlayShootGrappler();

                    if (grappling && grappleAnchor != null)
                    {
                        InitialGrappleBoost();
                    }
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////// FOR MAIN MENU
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                if (TransitionManager.instance.isMainMenu && Fade.instance.titleFinish && !Fade.instance.isAtMenu && !Fade.instance.howToPlay)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        grappling = true;

                        grappleAnchor = FindClosestMidAnchor().transform;

                        RaycastHit hit;

                        if (Physics.Raycast(grappleStart.position, (grappleAnchor.position - grappleStart.position), out hit, maxGrappleDistance, canBeGrappled))
                        {
                            grapplePoint = hit.point;

                            //Invoke(nameof(GrappleAnchor), grappleDelayTime);
                        }

                        lr.enabled = true;
                        lr.SetPosition(1, grapplePoint);

                        Invoke(nameof(StopGrapple), 2f);
                    }
                }
            }
        }
        
        #endregion

        #region StopAnchor/StopGrapple
        if (Input.GetKeyUp(grappleKey) && !TransitionManager.instance.isMainMenu)
        {
            // if joint is active, and within distance of closest anchor,
            // releasing space will disable line renderer, perform final boost and destroy that anchor
            if (joint != null && Vector3.Distance(grappleStart.position, closestAnchor.transform.position) < maxGrappleDistance)
            {
                StopAnchor();
                FinalGrappleBoost();
                StartAnchorDestroyCD();
            }
            // if within distance of closest mid anchor,
            // releasing space will disable line renderer, perform final boost and destroy that anchor
            if (Vector3.Distance(grappleStart.position, closestMidAnchor.transform.position) < maxGrappleDistance)
            {
                StopGrapple();
                FinalGrappleBoost();
                StartMidAnchorDestroyCD();
                isGrapplingMid = false;
            }
        }
        #endregion

        BoostBattery();

        // if timer > 0, keep counting down
        if (grapplingCDTimer > 0)
        {
            grapplingCDTimer -= Time.deltaTime;
        }

        if(joint == null)
        {
            if(GetComponent<SpringJoint>()  != null)
            {
                Destroy(GetComponent<SpringJoint>());
                Destroy(GetComponent<Rigidbody>());
            }
        }

        #region Destroy Previous Anchor
        // destroying anchor and mid anchor after releasing grapple
        if (destroyAnchorCountdown < 0 && prevAnchorIsDestroying)
        {
            prevAnchorIsDestroying = false;
            destroyAnchorCountdown = initialDestroyAnchorCD;
            closestAnchor.SetActive(false);

            deactivatedAnchors.Add(closestAnchor);
        }
        if (destroyMidAnchorCountdown < 0 && prevMidAnchorIsDestroying)
        {
            prevMidAnchorIsDestroying = false;
            destroyMidAnchorCountdown = initialDestroyMidAnchorCD;
            closestMidAnchor.SetActive(false);

            deactivatedAnchors.Add(closestMidAnchor);
        }
        #endregion

        // if space is still being held after exiting maxGrappleDistance, detach anchor and performs boost (Side Anchor)
        if (joint != null && Vector3.Distance(grappleStart.position, closestAnchor.transform.position) > maxGrappleDistance)
        {
            StopAnchor();
            FinalGrappleBoost();
            StartAnchorDestroyCD();
        }

        // if space is still being held after exiting maxGrappleDistance, detach anchor and performs boost (Mid Anchor)
        if (isGrapplingMid && Vector3.Distance(grappleStart.position, closestMidAnchor.transform.position) > maxGrappleDistance)
        {
            isGrapplingMid = false;
            StopGrapple();
            FinalGrappleBoost();
            StartMidAnchorDestroyCD();
        }
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
        // instantiates a joint component
        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        float distanceFromPoint = Vector3.Distance(grappleStart.position, grappleAnchor.position);

        // distance that grapple will try to keep from grapple point, increaing max will increase length of the grapple
        joint.maxDistance = distanceFromPoint * 0.2f;
        joint.minDistance = distanceFromPoint * 0f;

        // values
        joint.spring = 20f;
        joint.damper = 40f;
        joint.massScale = 5f;
    }

    private void InitialGrappleBoost()
    {
        kc.InitialBoostKart();
    }

    public void FinalGrappleBoost()
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
        if (!TransitionManager.instance.isMainMenu)
        {
            if (grappling)
            {
                if (kc.currentBattery < kc.maxBattery)
                    kc.currentBattery += 2f * Time.deltaTime;
            }
            else
            {
                if (kc.currentBattery > 0)
                    kc.currentBattery -= 1.5f * Time.deltaTime;
            }
        }
    }

    public void StopGrapple()
    {
        grappling = false;
        //grapplingCDTimer = grapplingCD;
        grappleAnchor = null;

        lr.enabled = false;

        if (Input.GetKey(grappleKey)) Input.GetKeyUp(grappleKey);
    }

    public void StopAnchor()
    {
        Destroy(joint);
        Destroy(gameObject.GetComponent<Rigidbody>());

        
        grappling = false;
        grapplingCDTimer = grapplingCD;
        grappleAnchor = null;

        lr.enabled = false;
        joint = null;
        if (Input.GetKey(grappleKey)) Input.GetKeyUp(grappleKey);
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

            if (diff.y <= 14)
            {
                float currentDist = diff.sqrMagnitude;
                if (currentDist < dist)
                {
                    closest = GO;
                    dist = currentDist;
                }
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

            //Debug.Log(diff + "diff value");

            if(diff.y <= 70)
            {
                float currentDist = diff.sqrMagnitude;
                if (currentDist < dist)
                {
                    closest = GO;
                    dist = currentDist;
                }
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
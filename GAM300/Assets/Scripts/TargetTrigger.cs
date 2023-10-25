using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetTrigger : MonoBehaviour
{
    public AnchorTrigger anchorTrigger;

    public Transform target;

    public Image targetImage;

    private Vector3 originalTargetScale;
    private Vector3 pulseTargetScale_1;
    private Vector3 pulseTargetScale_2;
    private Vector3 lockOnScale;

    private float elapsedTime;
    private float scaleTimer;
    private float colorTimer;

    private bool lerpCheck;
    private bool colorCheck;
    private bool isLockOn;


    public Color colorWhite, colorLockOn, colorLockOn2;

    private void Start()
    {
        originalTargetScale = new Vector3(1.2f,1.2f,1.2f);
        pulseTargetScale_1 = new Vector3(0.7f, 0.7f, 0.7f);
        pulseTargetScale_2 = new Vector3(0.85f, 0.85f, 0.85f);
        lockOnScale = new Vector3(0.4f, 0.4f, 0.4f);
    }

    private void Update()
    {
        TargetScale();

        BoolSwitch();

        ColorChange();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Sphere")  isLockOn = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Sphere")  isLockOn = false;
    }

    public void TargetScale()
    {
        if (anchorTrigger.isAnchorTrigger)
        {
            target.gameObject.SetActive(true);

            if(elapsedTime < 2)
            {
                elapsedTime += 1.0f * Time.deltaTime;
            }

            if (!isLockOn)
            {
                if (!lerpCheck)
                {
                    target.localScale = Vector3.Lerp(target.localScale, pulseTargetScale_1, 2.0f * Time.deltaTime);
                }
                else
                {
                    target.localScale = Vector3.Lerp(target.localScale, pulseTargetScale_2, 2.0f * Time.deltaTime);
                }
            }
            else
            {
                target.localScale = Vector3.Lerp(target.localScale, lockOnScale, 8.0f * Time.deltaTime);
            }
            
        }
        else
        {
            if(elapsedTime > 0)
            {
                elapsedTime -= 1.0f * Time.deltaTime;
            }
            
            target.localScale = Vector3.Lerp(target.localScale, originalTargetScale, 10.0f * Time.deltaTime);
        }

        //SetActive false
        if(elapsedTime <= 0)
        {
            target.gameObject.SetActive(false);
        }
    }

    public void BoolSwitch()
    {
        if (anchorTrigger.isAnchorTrigger)
        {
            if(scaleTimer <= 0)
            {
                lerpCheck = false;
            }
            else if(scaleTimer >= 0.7f)
            {
                lerpCheck = true;
            }
            

            if (!lerpCheck)
            {
                scaleTimer += 1.0f * Time.deltaTime;
            }
            else
            {
                scaleTimer -= 1.0f * Time.deltaTime;
            }
        }
        else { return; }


        if (isLockOn)
        {
            if (colorTimer <= 0)
            {
                colorCheck = false;
            }
            else if (colorTimer >= 0.2f)
            {
                colorCheck = true;
            }

            if (!colorCheck)
            {
                colorTimer += 1.0f * Time.deltaTime;
            }
            else
            {
                colorTimer -= 1.0f * Time.deltaTime;
            }
        }
    }

    public void ColorChange()
    {
        if (isLockOn)
        {
            if (colorCheck)
            {
                targetImage.color = Color.Lerp(targetImage.color, colorLockOn, 4.0f * Time.deltaTime);
            }
            else
            {
                targetImage.color = Color.Lerp(targetImage.color, colorLockOn2, 4.0f * Time.deltaTime);
            }
        }
        else
        {
            targetImage.color = Color.Lerp(targetImage.color, colorWhite, 2.0f * Time.deltaTime);
        }
    }
}

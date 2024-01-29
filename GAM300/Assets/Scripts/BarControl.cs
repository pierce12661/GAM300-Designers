using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarControl : MonoBehaviour
{
    public KartController kc;
    public Grapple grappleScript;

    public Transform fillControl;
    public Image fill;

    public Color colorTarget, notColorTarget;

    private void Update()
    {
        BarCharge();
    }

    private void BarCharge()
    {
        fillControl.localScale = new Vector3(kc.currentBattery / kc.maxBattery, 1f, 1f);

        if (grappleScript.isWithinTarget)
        {
            fill.color = Color.Lerp(fill.color, colorTarget, 15.0f * Time.unscaledDeltaTime);
        }
        else
        {
            fill.color = Color.Lerp(fill.color, notColorTarget, 15.0f * Time.unscaledDeltaTime);
        }
    }
}

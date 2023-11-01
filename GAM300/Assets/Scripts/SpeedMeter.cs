using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedMeter : MonoBehaviour
{
    [SerializeField] private KartController kc;

    public Transform overlay;

    private Vector3 targetOverlay;

    public CanvasGroup cg;
    public Transform keys;


    private float timer;

    private void Update()
    {
        SpeedSlider();

        ControlsPrompt();
    }

    public void SpeedSlider()
    {
        targetOverlay = new Vector3(1 - kc.currentBattery / kc.maxBattery, overlay.localScale.y, overlay.localScale.z);

        overlay.localScale = Vector3.Lerp(overlay.localScale, targetOverlay, 8.0f * Time.deltaTime);
    }

    public void ControlsPrompt()
    {
        Vector3 target = new Vector3(0.007f, 0.007f, 0.007f);

        if(kc.realSpeed < 2.0f && kc.acceleration == 0 && kc.steerAmount == 0 && !TransitionManager.instance.isGameOver)
        {
            timer += 1.0f * Time.deltaTime;
        }
        else
        {
            timer -= 1.0f * Time.deltaTime;
        }

        if(timer >= 6.0f)
        {
            timer = 6.0f;

            //cg.alpha = Mathf.Lerp(cg.alpha, 1, 3.0f * Time.deltaTime);
            cg.gameObject.transform.localScale = Vector3.Lerp(cg.transform.localScale, target, 7.0f * Time.deltaTime);
            //keys.localScale = Vector3.Lerp(keys.localScale, Vector3.one, 3.0f * Time.deltaTime);
        }
        else if(timer < 3.0f)
        {
            //cg.alpha = Mathf.Lerp(cg.alpha, 0, 6.0f * Time.deltaTime);

            cg.gameObject.transform.localScale = Vector3.Lerp(cg.transform.localScale, Vector3.zero, 12.0f * Time.deltaTime);
            //keys.localScale = Vector3.Lerp(keys.localScale, Vector3.zero, 3.0f * Time.deltaTime);

            if (timer <= 0)
            {
                timer = 0;
            }
        }
    }
}

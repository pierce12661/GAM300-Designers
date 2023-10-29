using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedMeter : MonoBehaviour
{
    [SerializeField] private KartController kc;

    public Transform sliderFill;

    private Vector3 targetFill;

    public CanvasGroup cg;

    private float timer;

    private void Update()
    {
        SpeedSlider();

        ControlsPrompt();
    }

    public void SpeedSlider()
    {
        targetFill = new Vector3(kc.currentBattery / kc.maxBattery, sliderFill.localScale.y, sliderFill.localScale.z);

        sliderFill.localScale = Vector3.Lerp(sliderFill.localScale, targetFill, 8.0f * Time.deltaTime);
    }

    public void ControlsPrompt()
    {
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

            cg.alpha = Mathf.Lerp(cg.alpha, 1, 3.0f * Time.deltaTime);
        }
        else if(timer < 3.0f)
        {
            cg.alpha = Mathf.Lerp(cg.alpha, 0, 6.0f * Time.deltaTime);

            if(timer <= 0)
            {
                timer = 0;
            }
        }
    }
}

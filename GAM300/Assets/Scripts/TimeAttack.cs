using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeAttack : MonoBehaviour
{
    public static TimeAttack instance;

    [SerializeField] private float timeStart;

    [HideInInspector] public float currentTime;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        TimeCountdown();

    }
    public void TimeCountdown()
    {
        if (!TransitionManager.instance.isGameOver)
        {
            currentTime -= 1.0f * Time.deltaTime;
            ColorChange();
        }
        else
        {
            currentTime = 0f;
        }

        if(currentTime <= 0)
        {
            TransitionManager.instance.isGameOver = true;
        }
    }

    public void ColorChange()
    {
        if(currentTime < 10.0f)
        {
            //change color of text to red and play text pulsate anim
        }
    }
}

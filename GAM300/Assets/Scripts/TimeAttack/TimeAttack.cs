using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeAttack : MonoBehaviour
{
    public static TimeAttack instance;

    [SerializeField] private float timeStart;

    [HideInInspector] public float currentTime;

    [HideInInspector] public float extendedTime;

    [HideInInspector] public bool timeExtension;

    [HideInInspector] public bool isDisabled;

    private float elapsedTime;

    public TextMeshProUGUI timerHUD;

    public TextMeshProUGUI extensionText;

    private Vector3 extensionTextPos;

    private bool hasPlayed;


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        currentTime = timeStart;

        hasPlayed = false;

        extensionTextPos = new Vector3(timerHUD.transform.position.x + 150f, timerHUD.transform.position.y, timerHUD.transform.position.z);
    }

    private void Update()
    {
        TimeCountdown();

        ShowTimer();

        TimeExtendAnim();
    }
    public void TimeCountdown()
    {
        if (!isDisabled)
        {
            if (!TransitionManager.instance.isGameOver && !TransitionManager.instance.gameWin)
            {
                currentTime -= 1.0f * Time.deltaTime;
                ColorChange();
            }
            else
            {
                if (TransitionManager.instance.isGameOver)
                {
                    currentTime = 0f;
                }
            }

            if (currentTime <= 0)
            {
                if (!hasPlayed)
                {
                    hasPlayed = true;
                    TransitionManager.instance.GameOver();
                }
            }
        }
    }

    public void ToggleTimeAttack()
    {
        if (!isDisabled)
        {
            isDisabled = true;
        }
        else
        {
            isDisabled = false;
        }
    }

    public void ColorChange()
    {
        if(currentTime < 10.0f)
        {
            //change color of text to red and play text pulsate anim
        }
    }

    public void TimeExtendAnim()
    {
        if (timeExtension)
        {
            elapsedTime += 1.0f * Time.deltaTime;

            if(elapsedTime < 1f)
            {
                Vector3 sizeIncrease = new Vector3(1.2f, 1.2f, 1.2f);
                timerHUD.transform.localScale = Vector3.Lerp(timerHUD.transform.localScale, sizeIncrease, 3.0f * Time.deltaTime);
            }
            else
            {
                timeExtension = false;
            }
        }
        else
        {
            elapsedTime = 0;
            timerHUD.transform.localScale = Vector3.Lerp(timerHUD.transform.localScale, Vector3.one, 3.0f * Time.deltaTime);
        }
    }

    public void TimeExtendText()
    {
        extensionText.text = "+ " + extendedTime;

        //GameObject timeObj = Instantiate(extensionHUD.gameObject, extensionTextPos, Quaternion.identity);
        GameObject timeObj = Instantiate(extensionText.gameObject,timerHUD.transform.parent);

    }

    public void ShowTimer()
    {
        timerHUD.text = currentTime.ToString("f2");
    }

    public void ExtendTime(float timer)
    {
        timeExtension = true;
        extendedTime = timer;
        currentTime += timer;
        TimeExtendText();

        AudioManager.instance.PlayCheckpoint();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public static Fade instance;

    private float elapsedTime;
    public float duration;

    private float titleTimer;

    [HideInInspector] public float fadeInTimer;

    public CanvasGroup FadeGroup;
    public GameObject startScreenCanvas;

    [HideInInspector] public bool titleFinish;
    [HideInInspector] public bool isAtMenu;
    [HideInInspector] public bool playGame;
    [HideInInspector] public bool howToPlay;


    //HOW TO PLAY
    public Transform selector;
    private float selectorTime;
    private float failsafe;
    private bool selectorCheck;
    


    private bool startGame;

    private void Start()
    {
        FadeGroup.alpha = 1.0f;
        titleTimer = 0;
        elapsedTime = 0;
        fadeInTimer = 0;
        startGame = false;

        instance = this;
    }

    void Update()
    {
        WhiteFade();
        TitleLerp();
        WhiteFadeIn();
        HowToPlay();
    }

    public void WhiteFade()
    {
        if(elapsedTime < duration && !startGame)
        {
            elapsedTime += 1.0f * Time.deltaTime;

            float progress = elapsedTime / duration;

            FadeGroup.alpha = Mathf.Lerp(FadeGroup.alpha, 0, progress);
        }
    }

    public void WhiteFadeIn()
    {
        if (playGame)
        {
            startGame = true;

            FadeGroup.alpha = Mathf.Lerp(FadeGroup.alpha, 1, 3.0f * Time.deltaTime);

            fadeInTimer += 1.0f * Time.deltaTime;

            if (fadeInTimer > 2f)
            {
                TransitionManager.instance.OpenScene("Main");
            }
        }
    }

    public void TitleLerp()
    {
        if(titleTimer < 4f)
        {
            titleTimer += 1.0f * Time.deltaTime;
        }

        if(titleTimer >= 1.75f)
        {
            startScreenCanvas.SetActive(true);
        }

        if(titleTimer >= 2.9f)
        {
            titleFinish = true;
        }
    }

    public void HowToPlay()
    {
        if (howToPlay)
        {
            if(failsafe < 2f)
            {
                failsafe += 1.0f * Time.deltaTime;
            }

            Vector3 target = new Vector3(1.2f, 1.2f, 1.2f);

            if (!selectorCheck)
            {
                selectorTime += 1.0f * Time.deltaTime;

                selector.localScale = Vector3.Lerp(selector.localScale, target, 2.0f * Time.deltaTime);
            }
            else
            {
                selectorTime -= 1.0f * Time.deltaTime;

                selector.localScale = Vector3.Lerp(selector.localScale, Vector3.one, 2.0f * Time.deltaTime);
            }
            
            if(selectorTime < 0)
            {
                selectorCheck = false;
            }
            else if (selectorTime > 1.2f)
            {
                selectorCheck = true;
            }

            if(failsafe >= 2f)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Back to main menu");
                    howToPlay = false;
                    failsafe = 0;

                    Invoke("MenuCheck", 2.2f);
                }
            }
        }
    }

    public void MenuCheck()
    {
        isAtMenu = true;
    }
}

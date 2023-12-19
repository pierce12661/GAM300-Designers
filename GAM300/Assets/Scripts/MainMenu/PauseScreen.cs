using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public static PauseScreen instance;

    [SerializeField] private int selectionID;
    [SerializeField] private int confirmID;

    public List<Transform> highlights;
    public List<Transform> restartConfirmHighlights;
    public List<Transform> quitConfirmHighlights;
    public List<Transform> howToPlayHighlights;
    public List<Transform> howToPlayHighlights_2;

    [HideInInspector] public bool isQuitConfirm;
    [HideInInspector] public bool isRestartConfirm;
    [HideInInspector] public bool isHowToPlay;
    [HideInInspector] public bool buffer;

    private bool isPage2;

    public AudioSource bgmSource;

    public CanvasGroup pauseScreen;
    public CanvasGroup overlay;

    public Transform quitConfimationScreen;
    public Transform restartConfimationScreen;
    public Transform howToPlayScreen_1;
    public Transform howToPlayScreen_2;

    private Vector3 point_1 = new Vector3(-1920, 0, 0);
    private Vector3 point_2 = new Vector3(1920, 0, 0);
    private Vector3 point_mid = new Vector3(0, 0, 0);

    private float pageTimer;
    private float failsafe;
    private float duration;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        duration = 0.2f;
    }

    private void Update()
    {
        ActivatePause();

        InputControl();

        ConfirmationInputs();

        SelectorLerp();

        ButtonEffect();

        ScreenLerps();

        HowToPlay();

        Restart();

        QuitConfirm();

        HowToPlayScreen();

        BufferTime();
    }

    public void ActivatePause()
    {

        if (TransitionManager.instance.gameIsPaused)
        {
            pauseScreen.alpha = Mathf.Lerp(pauseScreen.alpha, 1, 10.0f * Time.unscaledDeltaTime);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                AudioManager.instance.PlayButtonSelect();
            }
        }
        else
        {
            pauseScreen.alpha = Mathf.Lerp(pauseScreen.alpha, 0, 20.0f * Time.unscaledDeltaTime);

            selectionID = 0;

            ResetConfirmations();

        }
    }

    public void InputControl()
    {
        /* 0 = Resume
         * 1 = How To Play
         * 2 = Restart Level/Section
         * 3 = Quit Confirmation */


        if (TransitionManager.instance.gameIsPaused && !isQuitConfirm && !isRestartConfirm && !isHowToPlay)
        {
            bgmSource.volume = Mathf.Lerp(bgmSource.volume, 0.02f, 1.0f * Time.unscaledDeltaTime);

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (selectionID < 3)
                {
                    selectionID++;
                }
                else
                {
                    selectionID = 0;
                }

                AudioManager.instance.PlayButtonHover();

            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (selectionID > 0)
                {
                    selectionID--;
                }
                else
                {
                    selectionID = 3;
                }

                AudioManager.instance.PlayButtonHover();
            }
        }
        else
        {
            if (TransitionManager.instance.isGameOver || TransitionManager.instance.gameWin)
            {
                bgmSource.volume = Mathf.Lerp(bgmSource.volume, 0.02f, 1.0f * Time.unscaledDeltaTime);
            }
            else
            {
                bgmSource.volume = Mathf.Lerp(bgmSource.volume, 0.08f, 1.0f * Time.unscaledDeltaTime);
            }
            
        }
    }

    public void ConfirmationInputs()
    {
        if (TransitionManager.instance.gameIsPaused && (isQuitConfirm || isRestartConfirm || isHowToPlay))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                confirmID = 1;
                AudioManager.instance.PlayButtonHover();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                confirmID = 0;
                AudioManager.instance.PlayButtonHover();
            }
        }
    }


    public void SelectorLerp()
    {
        for (int i = 0; i < highlights.Count; i++)
        {
            highlights[selectionID].transform.localScale = Vector3.Lerp(highlights[selectionID].transform.localScale, Vector3.one, 10.0f * Time.unscaledDeltaTime);

            if (i != selectionID)
            {
                highlights[i].transform.localScale = Vector3.Lerp(highlights[i].transform.localScale, Vector3.zero, 15.0f * Time.unscaledDeltaTime);
            }
        }

        for (int x = 0; x < restartConfirmHighlights.Count; x++)
        {
            if (isRestartConfirm)
            {
                restartConfirmHighlights[confirmID].transform.localScale = Vector3.Lerp(restartConfirmHighlights[confirmID].transform.localScale, Vector3.one, 10.0f * Time.unscaledDeltaTime);

                if (x != confirmID)
                {
                    restartConfirmHighlights[x].transform.localScale = Vector3.Lerp(restartConfirmHighlights[x].transform.localScale, Vector3.zero, 15.0f * Time.unscaledDeltaTime);
                }
            }
            else
            {
                restartConfirmHighlights[x].transform.localScale = Vector3.Lerp(restartConfirmHighlights[x].transform.localScale, Vector3.zero, 20.0f * Time.unscaledDeltaTime);
            }
        }

        for (int z = 0; z < quitConfirmHighlights.Count; z++)
        {
            if (isQuitConfirm)
            {
                quitConfirmHighlights[confirmID].transform.localScale = Vector3.Lerp(quitConfirmHighlights[confirmID].transform.localScale, Vector3.one, 10.0f * Time.unscaledDeltaTime);

                if (z != confirmID)
                {
                    quitConfirmHighlights[z].transform.localScale = Vector3.Lerp(quitConfirmHighlights[z].transform.localScale, Vector3.zero, 15.0f * Time.unscaledDeltaTime);
                }
            }
            else
            {
                quitConfirmHighlights[z].transform.localScale = Vector3.Lerp(quitConfirmHighlights[z].transform.localScale, Vector3.zero, 20.0f * Time.unscaledDeltaTime);
            }
        }

        for (int y = 0; y < howToPlayHighlights.Count; y++)
        {
            if (isHowToPlay)
            {
                howToPlayHighlights[confirmID].transform.localScale = Vector3.Lerp(howToPlayHighlights[confirmID].transform.localScale, Vector3.one, 10.0f * Time.unscaledDeltaTime);
                howToPlayHighlights_2[confirmID].transform.localScale = Vector3.Lerp(howToPlayHighlights_2[confirmID].transform.localScale, Vector3.one, 10.0f * Time.unscaledDeltaTime);

                if (y != confirmID)
                {
                    howToPlayHighlights[y].transform.localScale = Vector3.Lerp(howToPlayHighlights[y].transform.localScale, Vector3.zero, 15.0f * Time.unscaledDeltaTime);
                    howToPlayHighlights_2[y].transform.localScale = Vector3.Lerp(howToPlayHighlights_2[y].transform.localScale, Vector3.zero, 15.0f * Time.unscaledDeltaTime);
                }
            }
            else
            {
                howToPlayHighlights[y].transform.localScale = Vector3.Lerp(howToPlayHighlights[y].transform.localScale, Vector3.zero, 20.0f * Time.unscaledDeltaTime);
                howToPlayHighlights_2[y].transform.localScale = Vector3.Lerp(howToPlayHighlights_2[y].transform.localScale, Vector3.zero, 20.0f * Time.unscaledDeltaTime);
            }
        }
    }

    public void ButtonEffect()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isQuitConfirm && TransitionManager.instance.gameIsPaused && !isRestartConfirm && !isHowToPlay)
        {
            switch (selectionID)
            {
                case 0:
                    TransitionManager.instance.ResumeGame();
                    break;

                case 1:
                    isHowToPlay = true;
                    buffer = true;
                    break;

                case 2:
                    isRestartConfirm = true;
                    buffer = true;
                    break;

                case 3:
                    isQuitConfirm = true;
                    buffer = true;
                    break;
            }
        }

        if(ConfirmationCheck() == true && Input.GetKey(KeyCode.Escape))
        {
            ResetConfirmations();

            AudioManager.instance.PlayButtonSelect();
        }

        if(pageTimer <= 0)
        {
            isPage2 = false;
        }
    }

    public void ScreenLerps()
    {
        if(isHowToPlay || isRestartConfirm || isQuitConfirm)
        {
            if (failsafe < duration)
            {
                failsafe += 1.0f * Time.unscaledDeltaTime;
                pageTimer += 1.0f * Time.unscaledDeltaTime;
            }
        }
        else
        {
            if(failsafe > 0)
            {
                failsafe -= 1.0f * Time.unscaledDeltaTime;
            }

            if(pageTimer > 0)
            {
                pageTimer -= 1.0f * Time.unscaledDeltaTime;
            }
        }

        if (isHowToPlay)
        {
            howToPlayScreen_1.localScale = Vector3.Lerp(howToPlayScreen_1.localScale, Vector3.one, 20.0f * Time.unscaledDeltaTime);
            howToPlayScreen_2.localScale = Vector3.Lerp(howToPlayScreen_1.localScale, Vector3.one, 20.0f * Time.unscaledDeltaTime);
        }
        else
        {
            howToPlayScreen_1.localScale = Vector3.Lerp(howToPlayScreen_1.localScale, Vector3.zero, 30.0f * Time.unscaledDeltaTime);
            howToPlayScreen_2.localScale = Vector3.Lerp(howToPlayScreen_1.localScale, Vector3.zero, 30.0f * Time.unscaledDeltaTime);
        }


        //Restart Confirmation Screen
        if (isRestartConfirm)
        {
            restartConfimationScreen.localScale = Vector3.Lerp(restartConfimationScreen.localScale, Vector3.one, 20.0f * Time.unscaledDeltaTime);
        }
        else
        {
            restartConfimationScreen.localScale = Vector3.Lerp(restartConfimationScreen.localScale, Vector3.zero, 30.0f * Time.unscaledDeltaTime);
        }


        //Quit Confirmation Screen
        if (isQuitConfirm)
        {
            quitConfimationScreen.localScale = Vector3.Lerp(quitConfimationScreen.localScale, Vector3.one, 20.0f * Time.unscaledDeltaTime);
        }
        else
        {
            quitConfimationScreen.localScale = Vector3.Lerp(quitConfimationScreen.localScale, Vector3.zero, 30.0f * Time.unscaledDeltaTime);
        }

        if(isRestartConfirm || isQuitConfirm)
        {
            overlay.alpha = Mathf.Lerp(overlay.alpha, 1, 10.0f * Time.unscaledDeltaTime);
        }
        else if(!isRestartConfirm && !isQuitConfirm)
        {
            overlay.alpha = Mathf.Lerp(overlay.alpha, 0, 20.0f * Time.unscaledDeltaTime);
        }
    }

    public void HowToPlay()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isHowToPlay)
        {
            AudioManager.instance.PlayButtonSelect();

            switch (confirmID)
            {
                case 0:
                    if(failsafe > duration)
                    {
                        confirmID = 0;
                        isHowToPlay = false;
                    }
                    break;

                case 1:
                    if(failsafe > duration)
                    {
                        if (isPage2)
                        {
                            isPage2 = false;
                            confirmID = 1;
                        }
                        else
                        {
                            isPage2 = true;
                            confirmID = 1;
                        }
                    }
                    
                    break;
            }
        }
    }

    public void Restart()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isRestartConfirm)
        {

            switch (confirmID)
            {
                case 0:
                    if(failsafe > duration)
                    {
                        isRestartConfirm = false;

                        AudioManager.instance.PlayButtonSelectClose();
                    }
                    break;

                case 1:
                    confirmID = 0;
                    isRestartConfirm = false;
                    AudioManager.instance.PlayButtonSelect();

                    TransitionManager.instance.RestartGame();
                    break;
            }
        }
    }

    public void QuitConfirm()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isQuitConfirm)
        {
            

            switch (confirmID)
            {
                case 0:
                    if (failsafe > duration)
                    {
                        isQuitConfirm = false;
                        AudioManager.instance.PlayButtonSelectClose();
                    }
                    break;

                case 1:
                    AudioManager.instance.PlayButtonSelect();
                    TransitionManager.instance.QuitGame();

                    Debug.Log("QUIT GAME");
                    break;
            }
        }
    }

    public void HowToPlayScreen()
    {
        if (!isPage2)
        {
            howToPlayScreen_1.localPosition = Vector3.Lerp(howToPlayScreen_1.localPosition, point_mid, 12.0f * Time.unscaledDeltaTime);
            howToPlayScreen_2.localPosition = Vector3.Lerp(howToPlayScreen_2.localPosition, point_2, 12.0f * Time.unscaledDeltaTime);
        }
        else
        {
            howToPlayScreen_1.localPosition = Vector3.Lerp(howToPlayScreen_1.localPosition, point_1, 12.0f * Time.unscaledDeltaTime);
            howToPlayScreen_2.localPosition = Vector3.Lerp(howToPlayScreen_2.localPosition, point_mid, 12.0f * Time.unscaledDeltaTime);
        }
    }

    public void ResetConfirmations()
    {
        isQuitConfirm = false;
        isRestartConfirm = false;
        isHowToPlay = false;
    }

    public void BufferTime()
    {
        if(failsafe <= 0)
        {
            buffer = false;
        }
        else
        {
            buffer = true;
        }
    }

    public bool ConfirmationCheck()
    {
        if(isHowToPlay || isQuitConfirm || isRestartConfirm)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

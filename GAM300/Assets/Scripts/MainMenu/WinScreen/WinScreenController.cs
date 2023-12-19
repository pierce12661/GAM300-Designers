using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinScreenController : MonoBehaviour
{
    private int selectionID;

    public List<Transform> highlights;

    public CanvasGroup winScreen;
    public CanvasGroup buttons;

    public Transform winContents;
    public Transform winText;

    public TextMeshProUGUI coin;
    public TextMeshProUGUI score;

    private bool buttonsShown;

    private float winTimer;
    private float winScore;
    private float coinCount;
    private float duration;
    private float buttonCounter;

    


    private void Start()
    {
        duration = 30.0f;
    }

    private void Update()
    {
        ActivateScreens();

        InputControl();

        SelectorLerp();

        ButtonEffect();

        LerpScore();

        ShowScore();

        ScoreText();

        ResetScore();
    }


    public void ActivateScreens()
    {

        if (TransitionManager.instance.gameWin)
        {
            winScreen.alpha = Mathf.Lerp(winScreen.alpha, 1, 10.0f * Time.unscaledDeltaTime);

            if(winTimer < duration)
            {
                winTimer += 1.0f * Time.unscaledDeltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (buttonsShown)
                {
                    AudioManager.instance.PlayButtonSelect();
                }
                else
                {
                    winTimer = duration;
                    coinCount = CoinManager.instance.GetCoinCount();
                    winScore = CoinManager.instance.finalScore;
                    buttonCounter = 0.9f;
                }
            }
        }
        else
        {
            winScreen.alpha = Mathf.Lerp(winScreen.alpha, 0, 20.0f * Time.unscaledDeltaTime);

            selectionID = 0;
        }
    }

    public void InputControl()
    {
        /* 0 = Restart
         * 1 = MainMenu
         * 2 = Quit */

        if (TransitionManager.instance.gameWin)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (selectionID < 2)
                {
                    selectionID++;
                }
                else
                {
                    selectionID = 0;
                }

                AudioManager.instance.PlayButtonHover();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (selectionID > 0)
                {
                    selectionID--;
                }
                else
                {
                    selectionID = 2;
                }

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
    }

    public void ButtonEffect()
    {
        if (Input.GetKeyDown(KeyCode.Space) && buttonsShown)
        {
            switch (selectionID)
            {
                case 0:
                    TransitionManager.instance.RestartGame();
                    break;

                case 1:
                    TransitionManager.instance.MainMenuOpen();
                    break;

                case 2:
                    TransitionManager.instance.QuitGame();
                    break;
            }
        }
    }

    public void LerpScore()
    {
        if (TransitionManager.instance.gameWin)
        {
            if(winTimer < 0.2f)
            {
                winText.localScale = Vector3.Lerp(winText.localScale, new Vector3(1.2f,1.2f,1.2f), 12.0f * Time.unscaledDeltaTime);
            }
            else
            {
                winText.localScale = Vector3.Lerp(winText.localScale, Vector3.one, 8.0f * Time.unscaledDeltaTime); //for bounce
            }

            if (winTimer > 1.5f)
            {
                winContents.localScale = Vector3.Lerp(winContents.localScale, Vector3.one, 8.0f * Time.unscaledDeltaTime);
            }
        }
        else
        {
            winContents.localScale = Vector3.Lerp(winContents.localScale, Vector3.zero, 15.0f * Time.unscaledDeltaTime);
            winText.localScale = Vector3.Lerp(winContents.localScale, Vector3.zero, 15.0f * Time.unscaledDeltaTime);
        }
    }

    public void ShowScore()
    {
        if(winTimer > 2.1f)
        {

            if(coinCount < CoinManager.instance.GetCoinCount())
            {
                coinCount += 0.1f;
            }
            else
            {
                coinCount = CoinManager.instance.GetCoinCount();
            }
        }

        if(coinCount == CoinManager.instance.GetCoinCount() && winTimer > 3.1f)
        {
            if(winScore < CoinManager.instance.finalScore)
            {
                winScore += 1f;
            }
            else
            {
                winScore = CoinManager.instance.finalScore;
            }
        }

        if(winScore == CoinManager.instance.finalScore)
        {
            if(buttonCounter < 1.3f)
            {
                buttonCounter += 1.0f * Time.unscaledDeltaTime;
            }

            if(buttonCounter > 1.0f)
            {
                buttons.alpha = Mathf.Lerp(buttons.alpha, 1, 3.0f * Time.unscaledDeltaTime);

                if(buttonCounter >= 1.3f)
                {
                    buttonsShown = true;
                }
            }
        }
        else
        {
            buttons.alpha = Mathf.Lerp(buttons.alpha, 0, 12.0f * Time.unscaledDeltaTime);
        }
    }

    public void ScoreText()
    {
        coin.text = coinCount.ToString("F0");
        score.text = winScore.ToString("F0");
    }

    public void ResetScore()
    {
        if (!TransitionManager.instance.gameWin)
        {
            coinCount = 0;
            winScore = 0;
            buttons.alpha = Mathf.Lerp(buttons.alpha, 0, 12.0f * Time.unscaledDeltaTime);
            selectionID = 1;
            buttonCounter = 0;
            buttonsShown = false;
        }
    }
}

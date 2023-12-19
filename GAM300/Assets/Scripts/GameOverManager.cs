using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    private int selectionID;

    public List<Transform> highlights;

    public CanvasGroup gameOverScreen;
    public CanvasGroup title;
    public CanvasGroup buttons;

    public Transform coins;

    private bool buttonsShown;

    private float timer;
    private float duration;
    private float coinCount;
    private float buttonCounter;

    public TextMeshProUGUI coinTMP;

    private void Start()
    {
        duration = 5f;
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
        if (TransitionManager.instance.isGameOver)
        {
            gameOverScreen.alpha = Mathf.Lerp(gameOverScreen.alpha, 1, 10.0f * Time.unscaledDeltaTime);

            if (timer < duration)
            {
                timer += 1.0f * Time.unscaledDeltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (buttonsShown)
                {
                    AudioManager.instance.PlayButtonSelect();
                }
                else
                {
                    timer = duration;
                    coinCount = CoinManager.instance.GetCoinCount();
                    buttonCounter = 0.7f;
                }
            }
        }
        else
        {
            gameOverScreen.alpha = Mathf.Lerp(gameOverScreen.alpha, 0, 20.0f * Time.unscaledDeltaTime);

            selectionID = 1;
        }
    }

    public void InputControl()
    {
        /* 0 = Restart
         * 1 = MainMenu
         * 2 = Quit */

        if (TransitionManager.instance.isGameOver)
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
                    TransitionManager.instance.MainMenuOpen();
                    break;

                case 1:
                    TransitionManager.instance.RestartGame();
                    break;

                case 2:
                    TransitionManager.instance.QuitGame();
                    break;
            }
        }
    }

    public void LerpScore()
    {
        if (TransitionManager.instance.isGameOver)
        {
            title.alpha = Mathf.Lerp(title.alpha, 1, 3.0f * Time.unscaledDeltaTime);

            if (timer < 0.2f)
            {
                title.transform.localScale = Vector3.Lerp(title.transform.localScale, new Vector3(0.8f, 0.8f, 0.8f), 12.0f * Time.unscaledDeltaTime);
            }
            else
            {
                title.transform.localScale = Vector3.Lerp(title.transform.localScale, Vector3.one, 8.0f * Time.unscaledDeltaTime); //for bounce
            }

            if (timer > 1.1f)
            {
                coins.localScale = Vector3.Lerp(coins.localScale, Vector3.one, 8.0f * Time.unscaledDeltaTime);
            }
        }
        else
        {
            coins.localScale = Vector3.Lerp(coins.localScale, Vector3.zero, 15.0f * Time.unscaledDeltaTime);
            title.transform.localScale = Vector3.Lerp(title.transform.localScale, new Vector3(2.2f, 2.2f, 2.2f), 15.0f * Time.unscaledDeltaTime);
            title.alpha = Mathf.Lerp(title.alpha, 0, 3.0f * Time.unscaledDeltaTime);
        }
    }

    public void ShowScore()
    {
        if (timer > 1.6f)
        {

            if (coinCount < CoinManager.instance.GetCoinCount())
            {
                coinCount += 0.1f;
            }
            else
            {
                coinCount = CoinManager.instance.GetCoinCount();
            }
        }

        if (coinCount == CoinManager.instance.GetCoinCount() && timer > 1.6f)
        {
            if (buttonCounter < 1.1f)
            {
                buttonCounter += 1.0f * Time.unscaledDeltaTime;
            }

            if (buttonCounter >0.8f)
            {
                buttons.alpha = Mathf.Lerp(buttons.alpha, 1, 3.0f * Time.unscaledDeltaTime);

                if (buttonCounter >= 1.1f)
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
        coinTMP.text = coinCount.ToString("F0");
    }

    public void ResetScore()
    {
        if (!TransitionManager.instance.isGameOver)
        {
            coinCount = 0;
            buttons.alpha = Mathf.Lerp(buttons.alpha, 0, 12.0f * Time.unscaledDeltaTime);
            selectionID = 1;
            buttonCounter = 0;
            buttonsShown = false;
        }
    }
}

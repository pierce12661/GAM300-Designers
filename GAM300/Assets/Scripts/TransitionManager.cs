using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    [HideInInspector] public bool gameIsPaused;
    [HideInInspector] public bool isGameOver;
    [HideInInspector] public bool gameWin;

    [HideInInspector] public bool isMainMenu;

    private void Awake()
    {
        instance = this;

        isMainMenu = true;
    }

    private void Update()
    {
        if (!isMainMenu)
        {
            PauseAndResume();

            GameOverScreen();

            WinScreen();

            if (!isGameOver && Input.GetKey(KeyCode.R))
            {
                //RestartGame();

                KartCollisionDetector.instance.Respawn();
            }
        }

        if(SceneManager.GetActiveScene().buildIndex == 0)
        {

            isMainMenu = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else { isMainMenu = false; }


        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            GameOver();
        }
    }

    public void RestartGame()
    {
        OpenScene("Main");
        Time.timeScale = 1;
        CoinManager.instance.ResetCoinCount();
    }

    public void GameOver()
    {
        isGameOver = true;

        Cursor.lockState = CursorLockMode.None;
    }

    public void WinGame()
    {
        CoinManager.instance.CalculateFinalScore();
        Debug.Log(CoinManager.instance.finalScore);
        gameWin = true;
    }

    public void WinScreen()
    {
        if (!isGameOver)
        {
            if (gameWin)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0.1f, 4.0f * Time.unscaledDeltaTime);
            }
        }
    }

    public void GameOverScreen()
    {
        if (isGameOver)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.1f, 4.0f * Time.unscaledDeltaTime);
        }
    }
    public void PauseAndResume()
    {
        if (!isGameOver && !gameWin)
        {
            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (!gameIsPaused)
                {

                    PauseGame();
                }
                else
                {
                    if (PauseScreen.instance.ConfirmationCheck() == false && !PauseScreen.instance.buffer)
                    {
                        ResumeGame();
                        AudioManager.instance.PlaySelect();
                    }
                }
            }
            
        }
    }

    public void PauseGame()
    {
        gameIsPaused = true;

        AudioManager.instance.PlaySelect();

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        gameIsPaused = false;

        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MainMenuOpen()
    {
        OpenScene("Menu");
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenScene(string sceneName) //Load by scene name
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        Time.timeScale = 1;
    }
    
}

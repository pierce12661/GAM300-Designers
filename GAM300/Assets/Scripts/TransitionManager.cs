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

    public CanvasGroup gameOverScreen;
    public CanvasGroup winScreenCanvasGroup;
    public GameObject pauseScreen;
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
                RestartGame();
            }
        }

        if(SceneManager.GetActiveScene().buildIndex == 0)
        {

            isMainMenu = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else { isMainMenu = false; }
    }

    public void RestartGame()
    {
        OpenScene("Main");
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        isGameOver = true;

        Cursor.lockState = CursorLockMode.None;
    }

    public void WinGame()
    {
        Coins.instance.CalculateFinalScore();
        Debug.Log(Coins.instance.finalScore);
        gameWin = true;

        Cursor.lockState = CursorLockMode.None;
    }

    public void WinScreen()
    {
        if (!isGameOver)
        {
            if (gameWin)
            {
                winScreenCanvasGroup.gameObject.SetActive(true);

                winScreenCanvasGroup.alpha = Mathf.Lerp(winScreenCanvasGroup.alpha, 1, 3.0f * Time.unscaledDeltaTime);


                Time.timeScale = Mathf.Lerp(Time.timeScale, 0.1f, 3.0f * Time.unscaledDeltaTime);
            }
            else
            {
                winScreenCanvasGroup.alpha = 0;

                winScreenCanvasGroup.gameObject.SetActive(false);
            }
        }
    }

    public void GameOverScreen()
    {
        if (isGameOver)
        {
            gameOverScreen.gameObject.SetActive(true);

            gameOverScreen.alpha = Mathf.Lerp(gameOverScreen.alpha, 1, 3.0f * Time.unscaledDeltaTime);

            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.1f, 3.0f * Time.unscaledDeltaTime);
        }
        else
        {
            gameOverScreen.alpha = 0;
            gameOverScreen.gameObject.SetActive(false);
        }
    }
    public void PauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameIsPaused)
            {
                
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        gameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0;

        pauseScreen.SetActive(true);
    }

    public void ResumeGame()
    {
        gameIsPaused = false;

        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;

        pauseScreen.SetActive(false);
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

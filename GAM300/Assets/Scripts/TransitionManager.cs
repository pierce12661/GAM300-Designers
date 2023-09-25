using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    [HideInInspector] public bool gameIsPaused;
    [HideInInspector] public bool isGameOver;

    public CanvasGroup gameOverScreen;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        PauseAndResume();

        GameOverScreen();

        if (!isGameOver && Input.GetKey(KeyCode.R))
        {
            RestartGame();
        }
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

    public void GameOverScreen()
    {
        if (isGameOver)
        {
            gameOverScreen.alpha = Mathf.Lerp(gameOverScreen.alpha, 1, 3.0f * Time.unscaledDeltaTime);

            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.1f, 3.0f * Time.unscaledDeltaTime);
        }
        else
        {
            gameOverScreen.alpha = 0;
        }
    }
    public void PauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.P))
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

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        gameIsPaused = false;

        Time.timeScale = 1;
    }

    public void OpenScene(string sceneName) //Load by scene name
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}

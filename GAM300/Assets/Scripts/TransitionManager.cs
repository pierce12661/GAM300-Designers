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

    private bool isCheating;

    public Transform cheatScreen;

    private void Awake()
    {
        instance = this;

        isMainMenu = true;

        isCheating = true;
    }

    private void Update()
    {
        if (!isMainMenu)
        {
            PauseAndResume();

            GameOverScreen();

            WinScreen();

            ShowCheats();

            if (!isGameOver && !gameWin && !gameIsPaused && Input.GetKey(KeyCode.R))
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


        //////////    Cheat Codes    //////////////

        if(!isGameOver && !gameWin && !gameIsPaused)
        {
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                GameOver();
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                WinGame();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                CoinManager.instance.AddCoinCount();
                CoinManager.instance.UpdateCoinsHUD();
                AudioManager.instance.PlayCoinPickUp();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TimeAttack.instance.ToggleTimeAttack();
                AudioManager.instance.PlayHover();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TimeAttack.instance.ExtendTime(5);
                AudioManager.instance.PlayCheckpoint();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                KartCollisionDetector.instance.ToggleTraps();
                AudioManager.instance.PlayHover();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ToggleCheats();
                AudioManager.instance.PlaySpikeTrapHit();
            }
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
        AudioManager.instance.PlayFallOutMap();
        Cursor.lockState = CursorLockMode.None;
    }

    public void WinGame()
    {
        CoinManager.instance.CalculateFinalScore();
        Debug.Log(CoinManager.instance.finalScore);
        gameWin = true;
        AudioManager.instance.PlayCoinPickUp();
        AudioManager.instance.PlayDrift();
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

    public void ToggleCheats()
    {
        if (!isCheating)
        {
            isCheating = true;
        }
        else
        {
            isCheating = false;
        }
    }

    public void ShowCheats()
    {
        if (isCheating)
        {
            cheatScreen.localScale = Vector3.Lerp(cheatScreen.localScale, new Vector3(0.007f, 0.007f, 0.007f), 14.0f * Time.deltaTime);
        }
        else
        {
            cheatScreen.localScale = Vector3.Lerp(cheatScreen.localScale,Vector3.zero, 18.0f * Time.deltaTime);
        }
    }
    
}

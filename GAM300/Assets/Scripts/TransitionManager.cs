using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    [HideInInspector] public bool gameIsPaused;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        PauseAndResume();

        if (Input.GetKeyDown(KeyCode.I) && Time.timeScale >= 1.0f)
        {
            Time.timeScale = 0.5f;
        }
        else if(Input.GetKeyDown(KeyCode.I) && Time.timeScale < 1)
        {
            Time.timeScale = 1.0f;
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

    public void OpenScene(string buildNumber) //Load by scene name
    {
        SceneManager.LoadScene(buildNumber, LoadSceneMode.Single);
    }
}

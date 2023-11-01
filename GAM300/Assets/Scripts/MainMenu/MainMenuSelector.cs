using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuSelector : MonoBehaviour
{
    public int id;

    public int quitID;

    public List<Image> selectors;

    public List<Transform> quitSelect;


    private bool lerpCheck;

    private bool confirmCheck;

    private float timer;

    public float timerTarget;

    private Vector3 LerpTarget = new Vector3(1.1f, 1.1f, 1.1f);

    public Transform quitConfirmation;

    public CanvasGroup confirmationCG;

    private void Start()
    {
        id = 0;

        AudioManager.instance.InvokeMenuSkid(1.3f);
        AudioManager.instance.InvokeMenuRev(0.2f);

    }
    void Update()
    {
        InputControl();

        SelectorLerp();

        ButtonEffect();

        ConfirmationScreen();
    }

    public void InputControl()
    {
        if (Fade.instance.isAtMenu && !confirmCheck)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if(id > 0)
                {
                    id--;
                }
                else
                {
                    id = 2;
                }

                AudioManager.instance.ButtonHover();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (id < 2)
                {
                    id++;
                }
                else
                {
                    id = 0;
                }

                AudioManager.instance.ButtonHover();
            }
        }
        else
        {
            return;
        }
    }

    public void SelectorLerp()
    {
        //if (!lerpCheck)
        //{
        //    timer += 1.0f * Time.deltaTime;
        //}
        //else
        //{
        //    timer -= 1.0f * Time.deltaTime;
        //}

        //if(timer < 0)
        //{
        //    lerpCheck = false;
        //}
        //else if(timer > timerTarget)
        //{
        //    lerpCheck = true;
        //}


        for(int i = 0; i < selectors.Count; i++)
        {
            //if (!lerpCheck)
            //{
                
            //}
            //else
            //{
            //    selectors[id].transform.localScale = Vector3.Lerp(selectors[id].transform.localScale, LerpTarget, 1.0f * Time.deltaTime) ;
            //}


            selectors[id].transform.localScale = Vector3.Lerp(selectors[id].transform.localScale, Vector3.one, 5.0f * Time.deltaTime);

            if (i != id)
            {
                selectors[i].transform.localScale = Vector3.Lerp(selectors[i].transform.localScale, Vector3.zero, 8.0f * Time.deltaTime);
            }
        }
        
    }
    public void ButtonEffect()
    {
        if(Fade.instance.isAtMenu && Input.GetKeyDown(KeyCode.Space) && !confirmCheck && !Fade.instance.howToPlay)
        {
            if (id == 0)
            {
                Fade.instance.playGame = true;
                //AudioManager.instance.MenuGrappler();
                AudioManager.instance.MenuStartGame();
            }
            else if(id == 1)
            {
                //OpenHowToPlay
                Fade.instance.howToPlay = true;
                Debug.Log("Open How To Play Screen");
                AudioManager.instance.InvokeMenuRev(0.5f);
                AudioManager.instance.InvokeMenuSkid(1.5f);
            }
            else
            {
                confirmCheck = true;
            }

            AudioManager.instance.Select();
        }
        
    }

    public void ConfirmationScreen()
    {
        if (confirmCheck)
        {

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                quitID = 1;

                AudioManager.instance.ButtonHover();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                quitID = 0;

                AudioManager.instance.ButtonHover();
            }

            for(int i = 0; i < quitSelect.Count; i++)
            {
                quitSelect[quitID].transform.localScale = Vector3.Lerp(quitSelect[quitID].transform.localScale, Vector3.one, 15.0f * Time.deltaTime);

                if (i != quitID)
                {
                    quitSelect[i].transform.localScale = Vector3.Lerp(quitSelect[i].transform.localScale, Vector3.zero, 20.0f * Time.deltaTime);
                }
            }

            quitConfirmation.localScale = Vector3.Lerp(quitConfirmation.localScale, Vector3.one, 5.0f * Time.deltaTime);

            confirmationCG.alpha = Mathf.Lerp(confirmationCG.alpha, 1, 3.0f * Time.deltaTime);

            timer += 1.0f * Time.deltaTime;

            if(timer >= 0.1f)
            {
                QuitButtons();
            }
        }
        else
        {
            quitConfirmation.localScale = Vector3.Lerp(quitConfirmation.localScale, Vector3.zero, 16.0f * Time.deltaTime);
            confirmationCG.alpha = Mathf.Lerp(confirmationCG.alpha, 0, 6.0f * Time.deltaTime);
            timer = 0;
        }
    }

    public void QuitButtons()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioManager.instance.Select();

            if (quitID == 0)
            {
                confirmCheck = false;

                
            }
            else
            {
                TransitionManager.instance.QuitGame();

                Debug.Log("quitGame");
            }
        }
    }
}

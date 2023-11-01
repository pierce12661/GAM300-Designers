using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamControl : MonoBehaviour
{
    public Camera cam;

    private Animator anim;

    private Animator playerAnim;

    private bool gameStarted;

    private bool howToPlayOpen;

    private float timer;

    private void Start()
    {
        anim = cam.GetComponent<Animator>();
        playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    private void Update()
    {
        PressStart();
        HowToPlayAnim();
    }

    public void PressStart()
    {
        if (Fade.instance.titleFinish && !gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //startGame
                gameStarted = true;
                anim.SetBool("GameStart", true);
                playerAnim.SetBool("GameStart", true);
                Fade.instance.startScreenCanvas.GetComponent<Animator>().SetBool("GameStart", true);

                Invoke("CheckingMenu", 3f);
                AudioManager.instance.Select();
                AudioManager.instance.MenuGrappler();
                AudioManager.instance.InvokeMenuRev(0.5f);
                AudioManager.instance.InvokeMenuSkid(3.2f);
            }
        }
        else
        {
            return;
        }
    }

    public void CheckingMenu()
    {
        Fade.instance.MenuCheck();
    }

    public void HowToPlayAnim()
    {
        if (Fade.instance.howToPlay)
        {
            anim.SetBool("HowToPlay", true);
            playerAnim.SetBool("HowToPlay", true);

            Debug.Log("Opening HowToPlay");

            howToPlayOpen = true;

            if(timer < 1.0f)
            {
                timer += 1.0f * Time.deltaTime;
            }
            if(timer > 1)
            {
                Fade.instance.isAtMenu = false;
            }
            
        }

        if(howToPlayOpen && !Fade.instance.howToPlay)
        {
            anim.SetBool("HowToPlay", false);
            playerAnim.SetBool("HowToPlay", false);

            howToPlayOpen = false;
            timer = 0;
        }
    }
}

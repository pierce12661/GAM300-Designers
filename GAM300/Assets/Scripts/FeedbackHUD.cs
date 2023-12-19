using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FeedbackHUD : MonoBehaviour
{
    public static FeedbackHUD instance;

    [SerializeField] private KartController kc;

    public GameObject FeedbackAnim;
    public TextMeshProUGUI AnimText;

    [HideInInspector] public bool boosted;
    private float timer;

    private int id;

    public List<string> wordVariation;

    private bool hasPlayed;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        FeedbackControl();
    }

    public void FeedbackControl()
    {
        if (boosted)
        {
            timer += 1.0f * Time.deltaTime;

            if(FeedbackAnim.activeInHierarchy == false)
            {
                id = Random.Range(0, wordVariation.Count - 1);
                AnimText.text = wordVariation[id];
                hasPlayed = false;
            }

            FeedbackAnim.SetActive(true);

            if (!hasPlayed)
            {
                switch (id)
                {
                    case 0:

                        AudioManager.instance.VO_Amazing();

                            break;

                    case 1:

                        AudioManager.instance.VO_Awesome();

                        break;

                    case 2:

                        AudioManager.instance.VO_Fantastic();

                        break;

                    case 3:

                        AudioManager.instance.VO_Great();

                        break;

                    case 4:

                        AudioManager.instance.VO_Stunning();

                        break;

                }

                hasPlayed = true;
            }

            if(timer > 3.0f)
            {
                boosted = false;
                timer = 0;
            }
        }
        else
        {
            FeedbackAnim.SetActive(false);
        }
    }
}

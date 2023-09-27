using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackHUD : MonoBehaviour
{
    public static FeedbackHUD instance;

    [SerializeField] private KartController kc;

    public GameObject FeedbackAnim;

    [HideInInspector] public bool boosted;
    private float timer;

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

            FeedbackAnim.SetActive(true);

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

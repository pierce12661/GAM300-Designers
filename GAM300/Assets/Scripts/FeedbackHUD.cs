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

    public List<string> wordVariation;

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
                AnimText.text = wordVariation[Random.Range(0, wordVariation.Count - 1)];
            }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    public TextMeshProUGUI coinHUD;
    public GameObject coinFly;
    private Animator anim;

    public static int coinCount;
    public float finalScore;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        anim = coinFly.GetComponent<Animator>();
    }

    public void AddCoinCount()
    {
        coinCount += 1;
        anim.SetTrigger("coinCollected");
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    public void ResetCoinCount()
    {
        coinCount = 0;
    }

    public void CalculateFinalScore()
    {
        finalScore = coinCount * (TimeAttack.instance.currentTime / 2);
    }

    public void UpdateCoinsHUD()
    {
        coinHUD.text = GetCoinCount().ToString();
    }
}
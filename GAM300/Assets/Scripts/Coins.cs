using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public static Coins instance;

    public static float coinCount;
    public float finalScore;

    // Start is called before the first frame update
    void Start()
    {
        coinCount = 0;
    }

    // Update is called once per frame
    void Update()
    {   

    }

    public void AddCoinCount()
    {
        coinCount += 1;
    }

    public void CalculateFinalScore()
    {
        finalScore = coinCount * (TimeAttack.instance.currentTime / 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        AddCoinCount();
        Debug.Log(coinCount);
        Destroy(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        AddCoinCount();
        AudioManager.instance.PlayCoinPickUp();
        Debug.Log(coinCount);
        CoinManager.instance.AddCoinCount();
        CoinManager.instance.UpdateCoinsHUD();
        Destroy(gameObject);

        
    }
}
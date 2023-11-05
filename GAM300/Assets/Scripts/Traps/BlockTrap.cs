using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTrap : MonoBehaviour
{
    private Rigidbody rb;

    public KartController playerKart;

    private bool hasHit;

    private float timer;
    private float slowSpeed;

    private GameObject hitObject;

    private void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        playerKart = GameObject.FindGameObjectWithTag("Player").GetComponent<KartController>();
    }

    private void Update()
    {
        DestroyObject();
        SlowPlayer();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            CrashForce();
            CameraShake.instance.camShaking = true;

            AudioManager.instance.PlayBlockTrap();
        }
    }

    public void CrashForce()
    {
            rb.AddExplosionForce(1.5f * playerKart.realSpeed, KartCollisionDetector.instance.crashPoint, 5, 1.5f, ForceMode.Impulse);

            slowSpeed = 0.65f * playerKart.currentSpeed;

            hitObject = this.gameObject;
            hasHit = true;
            timer = 0;
    }

    public void SlowPlayer()
    {
        if (hasHit && timer < 0.35f)
            playerKart.currentSpeed = Mathf.Lerp(playerKart.currentSpeed, slowSpeed, 10.0f * Time.deltaTime);
    }

    public void DestroyObject()
    {
        if (hasHit)
        {
            timer += 1.0f * Time.deltaTime;
            
            if(timer >= 4.0f)
            {
                Destroy(hitObject.transform.parent.gameObject);
                timer = 0;
                hasHit = false;
            }
        }
    }
}

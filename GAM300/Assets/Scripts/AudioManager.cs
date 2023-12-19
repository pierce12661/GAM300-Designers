using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Audio Manager is NULL");
            }
            return _instance;
        }
    }

    private AudioSource _aSource;
    public AudioClip[] _aClips;

    void Awake()
    {
        _instance = this;
    }

    // Drag drop audio file into the list in AudioManager in inspector
    // Add function to play sound
    // Use index to play specific sound
    // Example in PlayDrift(), do:
    // _aSource.PlayOneShot(_aClips[0]);
    // Go to where ever you want to play SFX, then do:
    // AudioManager.Instance.[function];
    // Example AudioManager.Instance.PlayDrift();

    void Start()
    {
        _aSource = GetComponent<AudioSource>();
    }

    public void PlayGrappleShoot()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(0,3)]);
    }

    public void PlayCarDrop()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(4, 6)]);
    }

    public void PlayCrash()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(7, 9)]);
    }

    public void PlayBlockTrap()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(10, 11)]);
    }

    public void PlaySpikeTrapHit()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(12, 13)]);
    }

    public void PlayFallOutMap()
    {
        _aSource.PlayOneShot(_aClips[14]);
    }

    public void PlayRespawn()
    {
        _aSource.PlayOneShot(_aClips[15]);
    }

    public void PlayExhaustBurst()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(16, 17)]);
    }

    public void PlayExhaustBoost()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(18, 19)]);
    }
    public void PlayDrift()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(20, 21)]);
    }

    public void PlayBraking()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(22, 23)]);
    }

    public void PlayWinGame()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(24, 26)]);
    }

    public void PlayLoseGame()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(27, 29)]);
    }

    public void PlayCountdown()
    {
        _aSource.PlayOneShot(_aClips[30]);
    }

    public void PlayCountdownStart()
    {
        _aSource.PlayOneShot(_aClips[31]);
    }

    public void PlayTimerGentleWarning()
    {
        _aSource.PlayOneShot(_aClips[32]);
    }

    public void PlayLowTimerCountdown()
    {
        _aSource.PlayOneShot(_aClips[33]);
    }
    public void PlayLowTimerCountdownSevere()
    {
        _aSource.PlayOneShot(_aClips[34]);
    }

    public void PlayButtonHover()
    {
        _aSource.PlayOneShot(_aClips[35]);
    }

    public void PlayButtonSelect()
    {
        _aSource.PlayOneShot(_aClips[36]);
    }

    public void PlayButtonSelectClose()
    {
        _aSource.PlayOneShot(_aClips[37]);
    }

    public void PlayRev()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(38, 39)]);
    }

    public void PlayCoinPickUp()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(40, 41)]); 
    }

    public void PlayCheckpoint()
    {
        _aSource.PlayOneShot(_aClips[42]);
    }

    public void VO_Amazing()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(43, 44)]);
    }

    public void VO_Awesome()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(45, 46)]);
    }

    public void VO_Fantastic()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(47, 48)]);
    }

    public void VO_Great()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(49, 50)]);
    }

    public void VO_Stunning()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(51, 52)]);
    }

    public void NegativeVO()
    {
        _aSource.PlayOneShot(_aClips[Random.Range(53, 58)]);
    }


    //public void PlayDrift()
    //{
    //     _aSource.PlayOneShot(_aClips[0]);
    //}

    //public void PlayExhaustBurst()
    //{
    //    _aSource.PlayOneShot(_aClips[1]);
    //}
    //public void PlayBoostBurst()
    //{
    //    _aSource.PlayOneShot(_aClips[2]);
    //}
    //public void PlayRev_1()
    //{
    //    _aSource.PlayOneShot(_aClips[3]);
    //}
    //public void PlayBraking()
    //{
    //    _aSource.PlayOneShot(_aClips[4]);
    //}
    //public void PlayShootGrappler()
    //{
    //    _aSource.PlayOneShot(_aClips[5]);
    //}
    //public void PlayFallOutMap()
    //{
    //    _aSource.PlayOneShot(_aClips[6]);
    //}
    //public void PlaySpikeTrapHit()
    //{
    //    _aSource.PlayOneShot(_aClips[7]);
    //}
    //public void PlayCheckpoint()
    //{
    //    _aSource.PlayOneShot(_aClips[8]);
    //}
    //public void PlayCoinPickUp()
    //{
    //    _aSource.PlayOneShot(_aClips[9]);
    //}
    //public void PlayFlame()
    //{
    //    _aSource.PlayOneShot(_aClips[53]);
    //}
    //public void PlayHover()
    //{
    //    _aSource.PlayOneShot(_aClips[11]);
    //}
    //public void PlaySelect()
    //{
    //    _aSource.PlayOneShot(_aClips[12]);
    //}
    //public void PlayDrop()
    //{
    //    _aSource.PlayOneShot(_aClips[13]);
    //}
    //public void PlayBlockTrap()
    //{
    //    _aSource.PlayOneShot(_aClips[14]);
    //}



    #region MenuSounds
    public void ButtonHover()
    {
        _aSource.PlayOneShot(_aClips[0]);
    }

    public void Select()
    {
        _aSource.PlayOneShot(_aClips[1]);
    }

    public void MenuGrappler()
    {
        _aSource.PlayOneShot(_aClips[2]);
    }

    public void MenuSkid()
    {
        _aSource.PlayOneShot(_aClips[3]);
    }
    public void InvokeMenuSkid(float timer)
    {
        Invoke("MenuSkid", timer);
    }

    public void MenuRev()
    {
        _aSource.PlayOneShot(_aClips[4]);
    }

    public void InvokeMenuRev(float timer)
    {
        Invoke("MenuRev", timer);
    }

    public void MenuStartGame()
    {
        _aSource.PlayOneShot(_aClips[5]);
    }

    #endregion Menu
}

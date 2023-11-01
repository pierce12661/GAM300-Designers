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

    public void PlayDrift()
    {
         _aSource.PlayOneShot(_aClips[0]);
    }

    public void PlayExhaustBurst()
    {
        _aSource.PlayOneShot(_aClips[1]);
    }
    public void PlayBoostBurst()
    {
        _aSource.PlayOneShot(_aClips[2]);
    }
    public void PlayRev_1()
    {
        _aSource.PlayOneShot(_aClips[3]);
    }
    public void PlayBraking()
    {
        _aSource.PlayOneShot(_aClips[4]);
    }
    public void PlayShootGrappler()
    {
        _aSource.PlayOneShot(_aClips[5]);
    }
    public void PlayFallOutMap()
    {
        _aSource.PlayOneShot(_aClips[6]);
    }
    public void PlaySpikeTrapHit()
    {
        _aSource.PlayOneShot(_aClips[7]);
    }
    public void PlayCheckpoint()
    {
        _aSource.PlayOneShot(_aClips[8]);
    }
    public void PlayCoinPickUp()
    {
        _aSource.PlayOneShot(_aClips[9]);
    }
    public void PlayFlame()
    {
        _aSource.PlayOneShot(_aClips[10]);
    }



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

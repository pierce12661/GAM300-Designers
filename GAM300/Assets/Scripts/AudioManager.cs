using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
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
}

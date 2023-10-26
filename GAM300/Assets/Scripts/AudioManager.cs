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

    void Start()
    {
        _aSource = GetComponent<AudioSource>();
    }

    public void PlayDrift()
    {
         _aSource.PlayOneShot(_aClips[0]);
    }
}

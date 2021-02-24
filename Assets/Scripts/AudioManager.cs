using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    // ################
    // ##  Singleton ##
    // ################
    public static AudioManager Instance {get; private set;} = null;

    // #############################
    // ## Unity Script Parameters ##
    // #############################

    public AudioClip background;

    public AudioClip shooting;

    public AudioClip replaceColor;

    public AudioClip collapse;

    public AudioClip scoreIncrease;

    public AudioClip select;

    public AudioClip start;

    
    // ##############
    // ## Privates ##
    // ##############
    private AudioSource backgroundAudioSource;
    private AudioSource effectAudioSource;
    private AudioClip previousClip;
    private Queue<AudioClip> clipsToPlay = new Queue<AudioClip>();


    void Awake()
    {
        InitSingleton();
        
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource.loop = true;
        backgroundAudioSource.clip = background;
        backgroundAudioSource.Play();

        effectAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void InitSingleton()
    {
        // ########################
        // ## Singleton Handling ##
        // ########################
        
        if (Instance != null)
        {
            throw new Exception("Can't have two AudioManager objects in the scene!");
        }
        
        Instance = this;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(clipsToPlay.Count > 0 && !effectAudioSource.isPlaying)
        {
            effectAudioSource.clip = clipsToPlay.Dequeue();
            effectAudioSource.Play();
        }

        if(clipsToPlay.Count == 0)
        {
            previousClip = null;
        }
    }

    public void PlayBackground()
    {
        backgroundAudioSource.Play();
    }

    public void PlayShooting()
    {
        PlayNext(shooting);
    }

    public void PlayCollapse()
    {
        PlayNext(collapse);
    }

    public void PlayScoreIncrease()
    {
        PlayNext(scoreIncrease);
    }

    public void PlayReplace()
    {
        PlayNext(replaceColor);
    }

    public void PlaySelect()
    {
        PlayNext(select);
    }

    public void PlayStart()
    {
        PlayNext(start);
    }

    public void PlayNext(AudioClip clip)
    {
        if(clipsToPlay.Count == 2 || previousClip == clip)
        {
            return;
        }
        clipsToPlay.Enqueue(clip);
        previousClip = clip;
    }
}

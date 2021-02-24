using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class is the main audio manager that all other classes should use to play
///  music and sound effects.
///
///     The way it's currently implemented is to support up to 2 request of clip playing
///  in order not to lose consistensy with time and to eliminate the choas effect that
///  multiple play requests in-game can cause. Example  of sequence that can generate
///  choas is: shooting, replacing cubes, removing cubes then shooting again. So here
///  we take only first 2. This doesn't happen often, but for the timeframe of this project
///     
///     I believe this was the best for our case. There are other improvements can be done
///  to improve this class, like playing in parallel, or shortening the effects and etc. 
/// </summary>
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

        DontDestroyOnLoad(this);
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

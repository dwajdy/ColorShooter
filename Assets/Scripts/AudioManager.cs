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

    // Each type of sound effect has a dedicated variable so it can be easily visible in editor, and easy to replace
    // For bigger projects, this should not be like this, but for this project I found it to be more convenient and straight forward.
    public AudioClip background;

    public AudioClip shooting;

    public AudioClip replaceColor;

    public AudioClip collapse;

    public AudioClip scoreIncrease;

    public AudioClip select;

    public AudioClip start;

    // ########################
    // ## Sound Effects Enum ##
    // ########################
    
    // This is used when other classes request to play effect from the AudioManager. 
    // See PlayEffect(...) below.
    public enum SoundEffect{
        SHOOT,
        REPLACE,
        COLLAPSE,
        SCORE,
        SELECT,
        START,
    }

    // ##############
    // ## Privates ##
    // ##############
    
    // audio source for playing background music.
    private AudioSource backgroundAudioSource;

    // audio source for playing sound effects
    private AudioSource effectAudioSource;

    // this variable will hold previously played clip.
    // we use it to make sure we don't play two consequent effects in small time fraction.
    private AudioClip previousClip;

    // this queue holds sound effects to play next.
    // the use of it is for not losing sounds when need to be played.
    private Queue<AudioClip> clipsToPlay = new Queue<AudioClip>();

    // ###############
    // ## Constants ##
    // ###############
    private const uint MAX_NUM_SOUND_EFFECTS_WAITING = 2;

    void Awake()
    {
        InitSingleton();
        SetupMusicSource();
        SetupEffectsSource();

        // to keep music playing when moving between scenes.
        DontDestroyOnLoad(this);
    }

    private void SetupMusicSource()
    {
        // Initilized and start playing background music (looping).
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        backgroundAudioSource.loop = true;
        backgroundAudioSource.clip = background;
        backgroundAudioSource.Play();
    }

    private void SetupEffectsSource()
    {
        // Initilized effects audio source.
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
    
    /// Update will make sure audio manager playing the next effect on queue.
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

    /// <summary>
    /// This function will handle a request to play a sound effect. If playing queue is full, sound clip will be not be played at all.
    /// </summary>
    public void PlayNext(AudioClip clip)
    {
        if(clipsToPlay.Count == MAX_NUM_SOUND_EFFECTS_WAITING || previousClip == clip)
        {
            return;
        }
        clipsToPlay.Enqueue(clip);
        previousClip = clip;
    }

    /// <summary>
    /// Request for playing a sound effect. No guarantee that it's going to be played, It's based on queue size at the moment of the request.
    /// </summary>
    public void PlayEffect(SoundEffect effect)
    {
        AudioClip clip = null;
        switch(effect)
        {
            case SoundEffect.SHOOT: 
                clip = shooting;
                break;
            case SoundEffect.REPLACE: 
                clip = replaceColor;
                break;
            case SoundEffect.COLLAPSE: 
                clip = collapse;
                break;
            case SoundEffect.SCORE: 
                clip = scoreIncrease;
                break;
            case SoundEffect.SELECT: 
                clip = select;
                break;
            case SoundEffect.START: 
                clip = start;
                break;
        }

        PlayNext(clip);
    }
    
}

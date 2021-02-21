using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    
    public AudioSource audioSource;

    public AudioClip shooting;

    public AudioClip replaceColor;

    public AudioClip collapse;

    public AudioClip scoreIncrease;

    private Queue<AudioClip> clipsToPlay = new Queue<AudioClip>();

    private AudioClip previousClip;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(clipsToPlay.Count > 0 && !audioSource.isPlaying)
        {
            audioSource.clip = clipsToPlay.Dequeue();
            audioSource.Play();
        }

        if(clipsToPlay.Count == 0)
        {
            previousClip = null;
        }
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

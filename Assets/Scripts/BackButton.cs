using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    private SoundEffectsManager soundEffectsManager;

    // Start is called before the first frame update
    void Awake()
    {
      soundEffectsManager = GameObject.FindGameObjectWithTag("SoundEffects").GetComponent<SoundEffectsManager>();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickBack()
    {
        soundEffectsManager.PlaySelect();
        SceneManager.LoadScene("GameScene");
    }
}

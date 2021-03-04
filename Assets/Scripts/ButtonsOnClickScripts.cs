using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class holds all "onClick" functions for the in-game buttons in game scene (not including how-to-play scene).
/// </summary>
public class ButtonsOnClickScripts : MonoBehaviour
{
    // ##############
    // ## Privates ##
    // ##############

    // this will hold the game objects that should be enabled after clicking on 'start game' button.
    private GameObject[] objectToEnableAfterStartGame;

    // this will hold the game objects that should be disabled after clicking on 'start game' button.
    private GameObject[] objectToDisableAfterStartGame;
    
    // ###############
    // ## Constants ##
    // ###############
    private const string CREDITS_SCENE_NAME = "CreditsScene";
    private const string HOW_TO_PLAY_SCENE_NAME = "HowToPlayScene";
    private const string ENABLE_ON_START_GAME_TAG = "EnableOnStartGame";
    private const string DISABLE_ON_START_GAME_TAG = "DisableOnStartGame";

    // ###############
    // ## Methods   ##
    // ###############

    /// <summary>
    /// It performs the following: 
    /// (1) Disables and enables relevant UI elements
    /// (2) plays "select" sound effect
    /// (3) start FPS camera mode if eneabled in config.
    /// (4) requests from GameManager to create cubes wall.
    /// </summary>
    public void onClickStartButton()
    {
        foreach(GameObject obj in objectToEnableAfterStartGame)
        {
            obj.SetActive(true);
        }

        foreach(GameObject obj in objectToDisableAfterStartGame)
        {
            obj.SetActive(false);
        }

        AudioManager.Instance.PlayEffect(AudioManager.SoundEffect.SELECT);

        if(GameManager.Instance.GameConfigs.FirstPersonCameraEffect)
        {
            // No need to store it, we just add it and forget about it :) . No further use of it.
            Camera.main.gameObject.AddComponent<CameraMovement>();
        }

        // start creating wall (it's coroutine because it might some time to finish so we allow game to continue updating until done)
        IEnumerator  coroutine = GameManager.Instance.CreateCubesWall();
        StartCoroutine(coroutine);
        
    }

    /// <summary>
    /// It performs the following: 
    /// (1) plays "select" sound effect
    /// (2) requests from GameManager to create cubes wall again.
    /// </summary>
    public void onClickRestartButton()
    {
        AudioManager.Instance.PlayEffect(AudioManager.SoundEffect.SELECT);
        IEnumerator  coroutine = GameManager.Instance.CreateCubesWall();
        StartCoroutine(coroutine);
        
    }

    /// <summary>
    /// It performs the following: 
    /// (1) plays "select" sound effect
    /// (2) loads "how to play" scene.
    /// </summary>
    public void onClickHowToPlay()
    {
        AudioManager.Instance.PlayEffect(AudioManager.SoundEffect.SELECT);
        SceneManager.LoadScene(HOW_TO_PLAY_SCENE_NAME);
    }

    /// <summary>
    /// It performs the following: 
    /// (1) plays "select" sound effect
    /// (2) loads "credits" scene.
    /// </summary>
    public void onClickCredits()
    {
        AudioManager.Instance.PlayEffect(AudioManager.SoundEffect.SELECT);
        SceneManager.LoadScene(CREDITS_SCENE_NAME);
    }

    /// <summary>
    /// In this function we get all game objects that should be enabled/disable when user clicks on "start game"
    ///
    /// Important note:
    ///   We do this at Start() and not Awake() because GameManager might create objects at Awake() we want to make sure we catch all of them.
    /// </summary>
    
    void Start()
    {
        // disable the UI elements that we need to show after clicking on starting game
        // note: in unity it's hard to find inactive objects in a proper way. So I tag UI elements with special tag, and I set in-active.
        //       on OnClick function, we will enable those UI elements.
        objectToEnableAfterStartGame = GameObject.FindGameObjectsWithTag(ENABLE_ON_START_GAME_TAG);
        foreach(GameObject obj in objectToEnableAfterStartGame)
        {
            obj.SetActive(false);
        }

        // prepare them ahead. Will be disabled after user starts game.
        objectToDisableAfterStartGame = GameObject.FindGameObjectsWithTag(DISABLE_ON_START_GAME_TAG);

    }
}

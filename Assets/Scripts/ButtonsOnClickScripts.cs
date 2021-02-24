using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///     This class holds all "onClick" functions for the in-game buttons in game scene (not including how-to-play scene).
/// </summary>
public class ButtonsOnClickScripts : MonoBehaviour
{

    private GameObject[] objectToEnableAfterStartGame;
    private GameObject[] objectToDisableAfterStartGame;
    
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

        AudioManager.Instance.PlaySelect();

        // start creating wall (it's coroutine because it might some time to finish so we allow game to continue updating until done)
        IEnumerator  coroutine = GameManager.Instance.CreateCubesWall();
        StartCoroutine(coroutine);
        
    }

    public void onClickRestartButton()
    {
        AudioManager.Instance.PlaySelect();
        IEnumerator  coroutine = GameManager.Instance.CreateCubesWall();
        StartCoroutine(coroutine);
        
    }

    public void onClickHowToPlay()
    {
        AudioManager.Instance.PlaySelect();
        SceneManager.LoadScene("HowToPlayScene");
    }

    // We do this at Start() and not Awake() because GameManager might create objects at Awake() we want to make sure we catch all of them.
    void Start()
    {
        // disable the UI elements that we need to show after clicking on starting game
        // note: in unity it's hard to get inactive objects in a proper way. So tag UI elements with special tag, and we set in-active.
        //       on OnClick function, we will enable those UI elements.
        objectToEnableAfterStartGame = GameObject.FindGameObjectsWithTag("EnableOnStartGame");
        foreach(GameObject obj in objectToEnableAfterStartGame)
        {
            obj.SetActive(false);
        }

        // prepare them ahead. Will be disabled after user starts game.
        objectToDisableAfterStartGame = GameObject.FindGameObjectsWithTag("DisableOnStartGame");

    }
}

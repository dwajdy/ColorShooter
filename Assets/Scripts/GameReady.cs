using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class responsible of the 'wait' UI element. It knows how to update the text, and remove it when game starts.
/// The 'wait' element, is a text that shows when the cubes wall is still being prepared and the user cannot interact.
/// </summary>
public class GameReady : MonoBehaviour
{

    // ##############
    // ## Privates ##
    // ##############

    private CubesWallHandler cubesWallHandler = null;

    private float secondPassed = 0.0f;

    private bool previousLastGameIsReady = true;
    private Text textComp;

    // ###############
    // ## Constants ##
    // ###############
    private const float SECONDS_TO_UPDATE_TEXT = 1.5f;
    
    // ###############
    // ## Methods   ##
    // ###############

    // get text component and CubeWallHandler.
    void Start()
    {
        textComp = GetComponent<Text>();
        cubesWallHandler = GameManager.Instance.GetCubesWallHandler();
    }

    // Update 'wait' UI element based on game state.
    void Update()
    {
        if(! cubesWallHandler.IsGameReady && (cubesWallHandler.IsGameStartedFirstTime || previousLastGameIsReady == true))
        {
            UpdateWaitText();
        }
        else if(cubesWallHandler.IsGameReady && (previousLastGameIsReady == false))
        {
            StopWaiting();
        }
    }

    // Will update wait text (it animates it by adding simply '.')
    // other calculations done on this functions is to update time and flags.
    private void UpdateWaitText()
    {
        textComp.enabled = true;
        previousLastGameIsReady = cubesWallHandler.IsGameReady;
        secondPassed += Time.deltaTime;
        if (secondPassed > SECONDS_TO_UPDATE_TEXT)
        {
            secondPassed = 0.0f;
            if (textComp.text.Length < 16)
            {
                textComp.text += ".";
            }
            else
            {
                textComp.text = "WAIT..";
            }

            return;
        }
    }

    // will reset text status and disable it. And will play 'start' sound effect because waiting is over and cubes wall is ready!
    private void StopWaiting()
    {
        textComp.enabled = false;
            textComp.text = "WAIT..";
            previousLastGameIsReady = true;
            AudioManager.Instance.PlayEffect(AudioManager.SoundEffect.START);
    }
}

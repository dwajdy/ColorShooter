using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class implements the behavior of the in-game energy bar.
/// </summary>
public class EnergyScript : MonoBehaviour
{

    // ##############
    // ## Privates ##
    // ##############

    private uint maxEnergy = 6;
    private uint currEnergy = 6;

    private float energyDecreaseTime = 1.5f;

    private float timePassedFromLastDecrease = 0;

    private DateTime lastCubeHitTime = DateTime.Now;
    
    private Text textComp;

    private CubesWallHandler cubesWallHandler = null;

    // ###############
    // ## Methods   ##
    // ###############

    // Gets the energy text component, and initialize the CubesWallHandler
    void Start()
    {
        textComp = GetComponent<Text>();
        cubesWallHandler = GameManager.Instance.GetCubesWallHandler();
    }

    // Updates energy bar
    void Update()
    {
        // If game is over or game is not ready, reset everthing and return.
        if(cubesWallHandler.IsGameOver || !cubesWallHandler.IsGameReady)
        {
            Reset();
            return;
        }

        // if enery reached 0, then user failed. this is a special condition. we need to keep returning in order not to update.
        if(currEnergy == 0)
        {
            return;
        }

        // if user has a shot a new cube, increase energy.
        IncreaseEnergyBar();
        
        // update energy bar text to reflect new energy value.
        UpdateText();

        // decrease energy bar and set gameover=on if evergy reached 0
        DecreaseEnergyBar();
        
    }

    // reset energy bar state and flags.
    private void Reset()
    {
        lastCubeHitTime = cubesWallHandler.TimeOfPreviousClick;
        timePassedFromLastDecrease = 0.0f;
        currEnergy = maxEnergy;
    }

    // this will increase energy bar if conditions are met.
    private void IncreaseEnergyBar()
    {
        // if there's a new cube shot by user, increase energy bar.
        DateTime currCubeHitTime = cubesWallHandler.TimeOfPreviousClick;
        if(currCubeHitTime != lastCubeHitTime &&
           currEnergy < maxEnergy)
        {
            currEnergy++;
            lastCubeHitTime = currCubeHitTime;
        }
    }

    // this will decrease energy bar if conditions are met. 
    //  and updates GameOver state if energy reached 0.
    private void DecreaseEnergyBar()
    {
        timePassedFromLastDecrease += Time.deltaTime;
        if(timePassedFromLastDecrease < energyDecreaseTime)
        {
            return;
        }
        else
        {
            timePassedFromLastDecrease = 0.0f;
            currEnergy--;
            UpdateText();
        }

        if(0 == currEnergy)
        {
            cubesWallHandler.IsGameOver = true;
        }
    }

    // updates energy bar text to reflect current energy value.
    void UpdateText()
    {
        textComp.text = "";

        for(uint i=0; i < maxEnergy; ++i)
        {
            if(i < (maxEnergy-currEnergy))
            {
                textComp.text += "\n";
            }
            else
            {
                textComp.text += "_\n";
            }
        }
    }
}

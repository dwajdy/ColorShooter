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

    private uint maxEnergy = 6;
    private uint currEnergy = 6;

    private float energyDecreaseTime = 1.5f;

    private float timePassedFromLastDecrease = 0;

    private DateTime lastCubeHitTime = DateTime.Now;
    
    private Text textComp;

    private CubesWallHandler cubesWallHandler = null;

    // Start is called before the first frame update
    void Start()
    {
        textComp = GetComponent<Text>();
        cubesWallHandler = GameManager.Instance.GetCubesWallHandler();
    }

    // Update is called once per frame
    void Update()
    {

        if(cubesWallHandler.IsGameOver || !cubesWallHandler.IsGameReady)
        {
            lastCubeHitTime = cubesWallHandler.TimeOfPreviousClick;
            timePassedFromLastDecrease = 0.0f;
            currEnergy = maxEnergy;
            return;
        }

        if(currEnergy == 0)
        {
            return;
        }

        DateTime currCubeHitTime = cubesWallHandler.TimeOfPreviousClick;
        if(currCubeHitTime != lastCubeHitTime &&
           currEnergy < maxEnergy)
        {
            currEnergy++;
            lastCubeHitTime = currCubeHitTime;
        }
        
        UpdateText();

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

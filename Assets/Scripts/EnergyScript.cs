using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyScript : MonoBehaviour
{

    private uint maxEnergy = 6;
    private uint currEnergy = 6;

    private float energyDecreaseTime = 2.0f;

    private float timePassedFromLastDecrease = 0;

    GameDynamics gameDynamics;

    private DateTime lastCubeHitTime = DateTime.Now;
    
    private Text textComp;

    // Start is called before the first frame update
    void Start()
    {
        textComp = GetComponent<Text>();
        gameDynamics = GameObject.Find("Game").GetComponent<Settings>().GetGameDynamics();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameDynamics.GetIsGameOver() || !gameDynamics.IsGameReady())
        {
            lastCubeHitTime = gameDynamics.GetLastCubeHitTime();
            timePassedFromLastDecrease = 0.0f;
            currEnergy = maxEnergy;
            return;
        }

        if(currEnergy == 0)
        {
            return;
        }

        DateTime currCubeHitTime = gameDynamics.GetLastCubeHitTime();
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
            gameDynamics.SetIsGameOver(true);
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

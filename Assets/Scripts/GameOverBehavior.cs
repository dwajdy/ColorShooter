﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverBehavior : MonoBehaviour
{
    GameDynamics gameDynamics;

    // Start is called before the first frame update
    void Start()
    {
        gameDynamics = GameObject.Find("Game").GetComponent<Settings>().GetGameDynamics();
        //textComp = GetComponent<Text>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
       if(gameDynamics.GetIsGameOver())
       {
           SetIsActiveOnChilds(true);
       }
       else
       {
           SetIsActiveOnChilds(false);
       }
    }

    void SetIsActiveOnChilds(bool value)
    {
        int numChilds = this.transform.childCount;
        for (int childIndex = 0; childIndex < numChilds; ++childIndex)
        {
            transform.GetChild(childIndex).gameObject.SetActive(value);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverBehavior : MonoBehaviour
{
    Text textComp;
    GameDynamics gameDynamics;

    // Start is called before the first frame update
    void Start()
    {
        gameDynamics = GameObject.Find("Game").GetComponent<Settings>().GetGameDynamics();
        textComp = GetComponent<Text>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
       if(gameDynamics.GetIsGameOver())
       {
           textComp.enabled = true;
       }
       else
       {
           textComp.enabled = false;
       }
    }
}

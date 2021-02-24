using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameReady : MonoBehaviour
{
    public GameObject gameManager;

    public GameObject soundManager;

    private CubesWallHandler gameDynamics;

    private float secondPassed = 0.0f;

    private float secondsToUpdateText = 1.5f;
    private bool previousLastGameIsReady = true;
    private Text textComp;
    
    // Start is called before the first frame update
    void Start()
    {
        gameDynamics = gameManager.GetComponent<GameManager>().GetGameDynamics();
        textComp = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(! gameDynamics.IsGameReady() && (gameDynamics.IsGameStartedFirstTime() || previousLastGameIsReady == true))
        {
            textComp.enabled = true;
            previousLastGameIsReady = gameDynamics.IsGameReady();
            secondPassed += Time.deltaTime;
            if(secondPassed > secondsToUpdateText)
            {   
                secondPassed = 0.0f;
                if(textComp.text.Length < 16)
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
        else if(gameDynamics.IsGameReady() && (previousLastGameIsReady == false))
        {
            textComp.enabled = false;
            textComp.text = "WAIT..";
            previousLastGameIsReady = true;
            AudioManager.Instance.PlayStart();
        }
    }
}

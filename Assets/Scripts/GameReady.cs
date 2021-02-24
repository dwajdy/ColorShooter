using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class responsible of the wait UI element. It knows how to update the text, and remove it when game starts.
/// </summary>
public class GameReady : MonoBehaviour
{
    public GameObject gameManager;

    public GameObject soundManager;

    private CubesWallHandler cubesWallHandler = null;

    private float secondPassed = 0.0f;

    private float secondsToUpdateText = 1.5f;
    private bool previousLastGameIsReady = true;
    private Text textComp;
    
    // Start is called before the first frame update
    void Start()
    {
        textComp = GetComponent<Text>();
        cubesWallHandler = GameManager.Instance.GetCubesWallHandler();
    }

    // Update is called once per frame
    void Update()
    {
        if(! cubesWallHandler.IsGameReady() && (cubesWallHandler.GetIsGameStartedFirstTime() || previousLastGameIsReady == true))
        {
            textComp.enabled = true;
            previousLastGameIsReady = cubesWallHandler.IsGameReady();
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
        else if(cubesWallHandler.IsGameReady() && (previousLastGameIsReady == false))
        {
            textComp.enabled = false;
            textComp.text = "WAIT..";
            previousLastGameIsReady = true;
            AudioManager.Instance.PlayStart();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class responsible for handling the score UI element.
/// </summary>
public class ScoreBehavior : MonoBehaviour
{
    // #############################
    // ## Public Unity Parameters ##
    // #############################

    public Text textComp;
    
    // ##############
    // ## Privates ##
    // ##############

    private uint previousScore = 0;
    private CubesWallHandler cubesWallHandler = null;

    // ##############
    // ## Methods ##
    // ##############

    // get text component, and populate cubeWallHandler.
    void Start()
    {
        textComp = GetComponent<Text>();
        cubesWallHandler = GameManager.Instance.GetCubesWallHandler();
    }

    // Update score text if it got changed.
    void LateUpdate()
    {
        if(previousScore == cubesWallHandler.Score)
        {
            return;
        }

        GetComponent<Animator>().SetTrigger("IncreaseScore"); //triggers a colourful animation.
        textComp.text = "SCORE: " + cubesWallHandler.Score;
        previousScore = cubesWallHandler.Score;
    }
}

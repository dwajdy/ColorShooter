using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class responsible of the end-game UI elements. It shows the relevant texts and buttons based on win/lose situation.
/// </summary>
public class GameOverBehavior : MonoBehaviour
{
    // #############################
    // ## Public Unity Parameters ##
    // #############################
    
    // this is being populated on the editor side.
    public Text InfoText;

    // ##############
    // ## Privates ##
    // ##############

    private CubesWallHandler cubesWallHandler = null;

    // ###############
    // ## Methods   ##
    // ###############

    // gets cubesWallHandler.
    void Start() {
        cubesWallHandler = GameManager.Instance.GetCubesWallHandler();
    }

    /// Updates score text UI element when score changes.
    void LateUpdate()
    {
       if(GameManager.Instance.GetCubesWallHandler().IsGameOver)
       {
           SetIsActiveOnChilds(true);
           SetText(GameManager.Instance.GetCubesWallHandler().IsNoCubesLeft);
       }
       else
       {
           SetIsActiveOnChilds(false);
       }
    }

    /// Setting active child nodes which include: status text and button.
    void SetIsActiveOnChilds(bool value)
    {
        int numChilds = this.transform.childCount;
        for (int childIndex = 0; childIndex < numChilds; ++childIndex)
        {
            transform.GetChild(childIndex).gameObject.SetActive(value);
        }
    }

    /// Sets the text based on win/lose condition. If player finished all cubes without losing, it updates text to "GOOD GAME!", and "GAME OVER!" otherwise.
    void SetText(bool isNoCubesLeft)
    {
        if(isNoCubesLeft)
        {
            InfoText.text = "GOOD GAME!";
        }
        else
        {
            InfoText.text = "GAME OVER!";
        }
    }
}

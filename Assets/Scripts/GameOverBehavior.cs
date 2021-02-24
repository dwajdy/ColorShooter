using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class responsible of the end-game UI elements. It shows the relevant texts and buttons based on win/lose situation.
/// </summary>
public class GameOverBehavior : MonoBehaviour
{
    public Text InfoText;

    private CubesWallHandler cubesWallHandler = null;

    void Start() {
        cubesWallHandler = GameManager.Instance.GetCubesWallHandler();
    }

    /// <summary>
    /// Updates score text UI element when score changes.
    /// </summary>
    void LateUpdate()
    {
       if(GameManager.Instance.GetCubesWallHandler().GetIsGameOver())
       {
           SetIsActiveOnChilds(true);
           SetText(GameManager.Instance.GetCubesWallHandler().IsNoCubesLeft());
       }
       else
       {
           SetIsActiveOnChilds(false);
       }
    }

    /// <summary>
    /// Setting active child nodes which include: status text and button.
    /// </summary>
    void SetIsActiveOnChilds(bool value)
    {
        int numChilds = this.transform.childCount;
        for (int childIndex = 0; childIndex < numChilds; ++childIndex)
        {
            transform.GetChild(childIndex).gameObject.SetActive(value);
        }
    }

    /// <summary>
    /// Sets the text based on win/lose condition. If player finished all cubes without losing, it updates text to "GOOD GAME!", and "GAME OVER!" otherwise.
    /// </summary>
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

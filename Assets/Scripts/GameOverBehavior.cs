using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverBehavior : MonoBehaviour
{
    public Text InfoText;

    CubesWallHandler gameDynamics;

    // Start is called before the first frame update
    void Start()
    {
        gameDynamics = GameObject.Find("Game").GetComponent<GameManager>().GetGameDynamics();
        //textComp = GetComponent<Text>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
       if(gameDynamics.GetIsGameOver())
       {
           SetIsActiveOnChilds(true);
           SetText(gameDynamics.IsNoCubesLeft());
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

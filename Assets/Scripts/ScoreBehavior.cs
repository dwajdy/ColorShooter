using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBehavior : MonoBehaviour
{
    Text textComp;
    uint previousScore = 0;
    CubesWallHandler gameDynamics;

    // Start is called before the first frame update
    void Start()
    {
        textComp = GetComponent<Text>();
        gameDynamics = GameObject.Find("Game").GetComponent<GameManager>().GetGameDynamics();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(previousScore == gameDynamics.GetScore())
        {
            return;
        }

        GetComponent<Animator>().SetTrigger("IncreaseScore");
        textComp.text = "SCORE: " + gameDynamics.GetScore();
        previousScore = gameDynamics.GetScore();
    }
}

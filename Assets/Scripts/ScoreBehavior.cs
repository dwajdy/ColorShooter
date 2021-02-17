using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBehavior : MonoBehaviour
{
    Text textComp;

    GameDynamics gameDynamics;

    // Start is called before the first frame update
    void Start()
    {
        textComp = GetComponent<Text>();
        gameDynamics = GameObject.Find("Game").GetComponent<Settings>().GetGameDynamics();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        textComp.text = "SCORE: " + gameDynamics.GetScore();
    }
}

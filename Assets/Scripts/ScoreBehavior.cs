using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class responsible for handling the score UI element.
/// </summary>
public class ScoreBehavior : MonoBehaviour
{
    Text textComp;
    uint previousScore = 0;
    private CubesWallHandler cubesWallHandler = null;

    // Start is called before the first frame update
    void Start()
    {
        textComp = GetComponent<Text>();
        cubesWallHandler = GameManager.Instance.GetCubesWallHandler();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(previousScore == cubesWallHandler.Score)
        {
            return;
        }

        GetComponent<Animator>().SetTrigger("IncreaseScore");
        textComp.text = "SCORE: " + cubesWallHandler.Score;
        previousScore = cubesWallHandler.Score;
    }
}

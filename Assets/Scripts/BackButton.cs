using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class holds "onClick" function for the back button on how-to-play scene.
/// </summary>
public class BackButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickBack()
    {
        AudioManager.Instance.PlaySelect();
        SceneManager.LoadScene("GameScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class holds "onClick" function for the back button on how-to-play scene.
/// </summary>
public class BackButton : MonoBehaviour
{
    /// <summary>
    /// Plays 'select' sound effect and returns to main screen scene.
    /// </summary>
    public void onClickBack()
    {
        AudioManager.Instance.PlayEffect(AudioManager.SoundEffect.SELECT);
        SceneManager.LoadScene("GameScene");
    }
}

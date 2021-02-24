using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    private GameObject[] objectToEnableAfterStartGame;
    private GameObject[] objectToDisableAfterStartGame;
    private GunFire gunFire;

    public void PlayFireVisualEffect(GameObject objectToShoot)
    {
        gunFire.StartFire(objectToShoot);
    }
    
    public void onClickStartButton()
    {
        foreach(GameObject obj in objectToEnableAfterStartGame)
        {
            obj.SetActive(true);
        }

        foreach(GameObject obj in objectToDisableAfterStartGame)
        {
            obj.SetActive(false);
        }

        AudioManager.Instance.PlaySelect();

        // start creating wall (it's coroutine because it might some time to finish so we allow game to continue updating until done)
        IEnumerator  coroutine = GameManager.Instance.CreateCubesWall();
        StartCoroutine(coroutine);
        
    }

    public void onClickRestartButton()
    {
        AudioManager.Instance.PlaySelect();
        IEnumerator  coroutine = GameManager.Instance.CreateCubesWall();
        StartCoroutine(coroutine);
        
    }

    public void onClickHowToPlay()
    {
        AudioManager.Instance.PlaySelect();
        SceneManager.LoadScene("HowToPlayScene");
    }

    // Start is called before the first frame update
    void Awake()
    {
        // get gunFire to play animation on shooting.
        GameObject gunHead = GameObject.FindGameObjectWithTag("GunHead");
        gunFire = gunHead.GetComponent<GunFire>();

        // disable the UI elements that we need to show after clicking on starting game
        // note: in unity it's hard to get inactive objects in a proper way. So tag UI elements with special tag, and we set in-active.
        //       on OnClick function, we will enable those UI elements.
        objectToEnableAfterStartGame = GameObject.FindGameObjectsWithTag("EnableOnStartGame");
        foreach(GameObject obj in objectToEnableAfterStartGame)
        {
            obj.SetActive(false);
        }

        // prepare them ahead. Will be disabled after user starts game.
        objectToDisableAfterStartGame = GameObject.FindGameObjectsWithTag("DisableOnStartGame");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

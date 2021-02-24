using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    // ################
    // ##  Singleton ##
    // ################
    public static GameManager Instance {get; private set;} = null;

    // #############################
    // ## Unity Script Parameters ##
    // #############################

    [Header("Basic Settings")]
    public uint BoardWidth = 0;
    public uint BoardHeight = 0;
    public float WhiteCubesProbability = 0;
    public float RedCubesProbability = 0;
    public  uint PointsPerDestroyedCube = 0;

    [Header("Extras")]
    public bool FirstPersonCameraEffect = false;
    public uint PointsPerShot = 0;

    // #################
    // ## Member Vars ##
    // #################

    private CubeGenerator cubeGenerator = new CubeGenerator();
    private bool isDoneCreatingCubes = false;
    private CubesWallHandler cubesWallHandler = new CubesWallHandler();
    
    private UIManager UIManager;


    void Awake()
    {
        InitSingleton();
        ValidateConfig();
        cubeGenerator.Initialize();
        cubesWallHandler.Initialize();
        UIManager = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>();
    }
    
    private void InitSingleton()
    {
        // ########################
        // ## Singleton Handling ##
        // ########################
        
        if (Instance != null)
        {
            throw new Exception("Can't have two GameManager objects in the scene!");
        }
        
        Instance = this;
    }

    private void ValidateConfig()
    {
        if (BoardWidth == 0 ||
           BoardHeight == 0 ||
           WhiteCubesProbability == 0 ||
           RedCubesProbability == 0 ||
           PointsPerDestroyedCube == 0)
        {
            throw new Exception("Please check game config. Please check readme file for valid config options.");
        }

        if(FirstPersonCameraEffect == false)
        {
            Camera.main.GetComponent<FirstPersonCameraMode>().enabled = false;
        }

    }
    

    public IEnumerator CreateCubesWall()
    {
        isDoneCreatingCubes = false;
        cubesWallHandler.Reset();

        for(uint row = 0; row < GameManager.Instance.BoardHeight; ++row)
        {
            for(uint col = 0; col < GameManager.Instance.BoardWidth; ++col)
            {
                GameObject newCube = cubeGenerator.GenerateCube(col, row);
                cubesWallHandler.Add(newCube, col, row);
                yield return new WaitForSeconds(0.01f);
            }
        }

        isDoneCreatingCubes = true;
        cubesWallHandler.SetGameStartedFirstTime(true);
    }
    
    public 

    IEnumerator Start()
    {
        yield return null;
        //return RestartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(! isDoneCreatingCubes)
        {
            return;
        }

        bool calculationDone = cubesWallHandler.Update();

        if (Input.GetMouseButtonDown(0) && calculationDone)
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                   if(hit.transform.name == "CubeOnWall")
                   {
                       GameObject cubeGameObj = hit.transform.gameObject;

                       AudioManager.Instance.PlayShooting();
                       cubeGameObj.GetComponent<CubeBehavior>().Hit(cubesWallHandler, cubeGenerator);

                       Camera.main.GetComponent<Animator>().SetTrigger("IsCubeShot");
                       UIManager.PlayFireVisualEffect(cubeGameObj);
                   }
            }
        }
    }

    public CubesWallHandler GetGameDynamics()
    {
        return cubesWallHandler;
    }
}

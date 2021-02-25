using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

/// <summary>
/// [GameManager]
///    This class handles main game play. It mainly does the following:
///      - Reading configs and updating game accordingly
///      - Creating cubes wall
///      - Taking user input and calling update on relevent classes
///      - Invoking gun animation
/// </summary>
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

    private CubesWallHandler cubesWallHandler = new CubesWallHandler();
    private GunHandler gunHandler = new GunHandler();

    // #################
    // ## Constants   ##
    // #################

    private const string INTRO_CUBES_PREFAB_NAME = "Prefabs/IntroCubes";
    private const uint MAX_WIDTH = 20;
    private const uint MAX_HEIGHT = 15;

    // #################
    // ## Methods     ##
    // #################

    void Awake()
    {
        InitSingleton();
        ValidateConfig();
        CreateIntro();
        cubesWallHandler.Initialize();
        gunHandler.Initialize(); 
    }
    
    /// <summary>
    /// This function initializes the singleton instance pointer.
    /// </summary>
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

    /// <summary>
    /// Validates all game configs, and enable the "FPS camera" effect if needed.
    /// </summary>
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

    }
    

    /// <summary>
    /// Calls CubeWallHandler to create cubes wall.
    /// </summary>
    public IEnumerator CreateCubesWall()
    {
        return cubesWallHandler.CreateCubesWall();
    }

    /// <summary>
    /// Creates the game objects for the game-into animation (main screen). 
    /// The intro composed of "red and white dancing cubes" and other falling cubes of basic colors.
    /// </summary>    
    public void CreateIntro()
    {
        // Since these are not going to change, I created a prefab for all of them. All what is left to do is to instantiate it.
        GameObject introCubes = Resources.Load(INTRO_CUBES_PREFAB_NAME) as GameObject;
        GameObject.Instantiate(introCubes, introCubes.transform.position, Quaternion.identity);
    }

    /// <summary>
    /// Handles user input (mouses clicks) and calls the relevant methods to update cubes wall.
    /// </summary> 
    void Update()
    {
        if(! cubesWallHandler.IsDoneCreatingCubes)
        {
            return;
        }

        bool calculationDone = cubesWallHandler.Update();
        if (Input.GetMouseButtonDown(0) && calculationDone)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit objectHit;

            if (Physics.Raycast(ray, out objectHit))
            {
                GameObject hitGameObject = objectHit.transform.gameObject;

                   if(cubesWallHandler.IsCubeOnWallHit(hitGameObject))
                   {
                       cubesWallHandler.HandleCubeHit(hitGameObject);
                       
                       Camera.main.GetComponent<Animator>().SetTrigger("IsCubeShot");

                       AudioManager.Instance.PlayEffect(AudioManager.SoundEffect.SHOOT);
                       gunHandler.StartFire(hitGameObject);
                   }
            }
        }
    }

    /// <summary>
    /// Returns the CubesWallHadler. Used by several UI elements to read flags to know how they update themselves.
    /// I chose to let them pass through the GameManager and not directly to the CubesWallHandler in order not to create extra dependencies.
    /// </summary> 
    public CubesWallHandler GetCubesWallHandler()
    {
        return cubesWallHandler;
    }
}

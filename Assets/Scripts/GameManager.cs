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

    // ################
    // ##  Configs ##
    // ################

    public Configs GameConfigs;

    // #################
    // ## Member Vars ##
    // #################

    private CubesWallHandler cubesWallHandler = new CubesWallHandler();
    private GunHandler gunHandler = new GunHandler();

    // #################
    // ## Constants   ##
    // #################

    private const string INTRO_CUBES_PREFAB_NAME = "Prefabs/IntroCubes";
    private const string FLOOR_PREFAB_NAME = "Prefabs/Floor";
    private const string FLOOR_OPENGL_PREFAB_NAME = "Prefabs/Floor_OpenGL";
    private const uint MAX_WIDTH = 17;
    private const uint MAX_HEIGHT = 10;

    // #################
    // ## Methods     ##
    // #################

    void Awake()
    {
        InitSingleton();
        ValidateConfig();
        LoadPrefabs();
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
        if (GameConfigs.BoardWidth == 0 || GameConfigs.BoardWidth > MAX_WIDTH   ||
           GameConfigs.BoardHeight == 0 || GameConfigs.BoardHeight > MAX_HEIGHT ||
           GameConfigs.WhiteCubesProbability == 0 ||
           GameConfigs.RedCubesProbability == 0 ||
           GameConfigs.PointsPerDestroyedCube == 0)
        {
            #if UNITY_EDITOR
                        // NOTE: Application.Quit(); does not work in the editor
                        Debug.LogError("Invalid configs used. Please see README file. Stopping game.");
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                        Application.Quit();
            #endif
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
    /// Creates the game objects for the game-into animation (main screen) + floor. 
    /// The intro composed of "red and white dancing cubes" and other falling cubes of basic colors.
    /// </summary>    
    public void LoadPrefabs()
    {
        // Since these are not going to change, I created a prefab for all of them. All what is left to do is to instantiate it.
        GameObject introCubesPrefab = Resources.Load(INTRO_CUBES_PREFAB_NAME) as GameObject;
        GameObject.Instantiate(introCubesPrefab, introCubesPrefab.transform.position, Quaternion.identity);

        // Also floor is not going to change, create it and don't store. make it's parent the 
        string floorPrefabName = FLOOR_PREFAB_NAME;
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            floorPrefabName = FLOOR_OPENGL_PREFAB_NAME;
        }
        GameObject floorPrefab = Resources.Load(floorPrefabName) as GameObject;
        GameObject.Instantiate(floorPrefab, floorPrefab.transform.position, Quaternion.identity);
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
                       AudioManager.Instance.PlayEffect(AudioManager.SoundEffect.SHOOT);
                       Camera.main.GetComponent<Animator>().SetTrigger("IsCubeShot");
                       gunHandler.StartFire(hitGameObject);
                       cubesWallHandler.HandleCubeHit(hitGameObject);
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

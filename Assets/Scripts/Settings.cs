using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using Random = System.Random;

public class Settings : MonoBehaviour
{

    [Header("Game Settings")]
    public uint BoardWidth = 0;
    public uint BoardHeight = 0;
    public float WhiteCubesProbability = 0;
    public float RedCubesProbability = 0;
    public  uint PointsPerDestroyedCube = 0;

    // Start is called before the first frame update
    public enum Colors{
        Cyan,
        Yellow,
        Magenta,
        Black,
        Red, 
        White,
    }

    private Colors[] basicColors = new Colors[] { Colors.Cyan, Colors.Yellow, Colors.Magenta, Colors.Black};

    private Dictionary<Colors, Material> materials = new Dictionary<Colors, Material>();
    private Dictionary<Colors, AnimationClip> animations = new Dictionary<Colors, AnimationClip>();
    private Dictionary<Colors, float> probabilites = new Dictionary<Colors, float>();
    private Dictionary<Colors, string> specialPowers = new Dictionary<Colors, string>() {{Colors.Red, "RedBehavior"}, {Colors.White, "WhiteBehavior"}};

    private GameDynamics gameDynamics = new GameDynamics();

    private GunFire gunFire;
    private bool isDoneCreatingCubes = false;
    
    private SoundEffectsManager soundEffectsManager;

    private GameObject[] objectToEnableAfterStartGame;

    private GameObject[] objectToDisableAfterStartGame;

    void Awake()
    {
        if (BoardWidth == 0 ||
           BoardHeight == 0 ||
           WhiteCubesProbability == 0 ||
           RedCubesProbability == 0 ||
           PointsPerDestroyedCube == 0)
        {
            #if UNITY_EDITOR
                        // does not work in the editor
                        // Application.Quit(); 
                        //UnityEditor.EditorApplication.isPlaying = false;
            #else
                            //Application.Quit();
            #endif
        }

        // initialize the materials
        foreach (Colors color in Enum.GetValues(typeof(Colors)))
        {
            materials[color] = Resources.Load($"Materials/{color.ToString()}", typeof(Material)) as Material;
        }

        // initiaialize animations
        foreach (Colors color in Enum.GetValues(typeof(Colors)))
        {
            animations[color] = Resources.Load($"Animations/{color.ToString()}Emission", typeof(AnimationClip)) as AnimationClip;
        }

        // set probabilites
        probabilites[Colors.Red] = RedCubesProbability;
        probabilites[Colors.White] = WhiteCubesProbability;
        float othersProbability = (1.0f - RedCubesProbability - WhiteCubesProbability) / 4;
        probabilites[Colors.Cyan] = probabilites[Colors.Yellow] = probabilites[Colors.Magenta] = probabilites[Colors.Black] = othersProbability;

        soundEffectsManager = GameObject.FindGameObjectWithTag("SoundEffects").GetComponent<SoundEffectsManager>();

        gunFire = GameObject.FindGameObjectWithTag("GunHead").GetComponent<GunFire>();

        objectToEnableAfterStartGame = GameObject.FindGameObjectsWithTag("EnableOnStartGame");
        foreach(GameObject obj in objectToEnableAfterStartGame)
        {
            obj.SetActive(false);
        }

        objectToDisableAfterStartGame = GameObject.FindGameObjectsWithTag("DisableOnStartGame");

        gameDynamics.Init(BoardWidth, BoardHeight, PointsPerDestroyedCube, soundEffectsManager);
    }
    
    public void onClickRestartButton()
    {
        //gameDynamics.SetGameStartedFirstTime(false);

        if(isDoneCreatingCubes)
        {
            soundEffectsManager.PlaySelect();
            IEnumerator  coroutine = RestartGame();
            StartCoroutine(coroutine);
        }
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

        soundEffectsManager.PlaySelect();
        IEnumerator  coroutine = RestartGame();
        StartCoroutine(coroutine);
        
    }
    
    IEnumerator Start()
    {
        yield return null;
        //return RestartGame();
    }

    public IEnumerator RestartGame()
    {
        isDoneCreatingCubes = false;

        gameDynamics.Reset();

        var height = 2*Camera.main.orthographicSize;
        var width = height*Camera.main.aspect;

        var startingX = (-1.0f)*(BoardWidth)/2;

        for(uint row = 0; row < BoardHeight; ++row)
        {
            for(uint col = 0; col < BoardWidth; ++col)
            {
                GameObject cubePrefab = Resources.Load("Prefabs/Cube") as GameObject;
                Vector3 cubePos = new Vector3(startingX + col,height + row,10);
                var newObj = GameObject.Instantiate(cubePrefab, cubePos, transform.rotation) as GameObject;
                newObj.name = "Cube";
                newObj.GetComponent<Rigidbody>().freezeRotation = true;
                gameDynamics.Add(newObj, col, row);
                Random random = new Random();
                float randomNum = (float)random.NextDouble();

                float accumulatedChance = 0.0f;
                foreach(var prob in probabilites)
                {
                    if(randomNum < accumulatedChance + prob.Value)
                    {
                        CubeBehavior cubeBehavior;

                        newObj.GetComponent<MeshRenderer>().material = materials[prob.Key];

                        //AnimatorController controller = Resources.Load("Animations/Emission", typeof(AnimatorController)) as AnimatorController;
                        //var state = controller.layers[0].stateMachine.states.FirstOrDefault(s => s.state.name.Equals("Emission")).state;
                        //controller.SetStateEffectiveMotion(state, animations[prob.Key]);
                        //Animator newAnim = newObj.AddComponent<Animator>();
                        //newAnim.runtimeAnimatorController = new AnimatorOverrideController(controller);

                        Animator newAnim = newObj.AddComponent<Animator>();
                        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(Resources.Load("Animations/Emission", typeof(AnimatorController)) as AnimatorController);
                        newAnim.runtimeAnimatorController = animatorOverrideController;
                        animatorOverrideController["EmissionPlaceholder"] = animations[prob.Key];
                        
                        //clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
                        
                        if(specialPowers.ContainsKey(prob.Key))
                        {
                            cubeBehavior = newObj.AddComponent(Type.GetType(specialPowers[prob.Key])) as CubeBehavior;
                        }
                        else
                        {
                            cubeBehavior = newObj.AddComponent<CubeBehavior>();
                        }

                        cubeBehavior.UpdateCoords(col, row);

                        break;
                    }

                    accumulatedChance += prob.Value;
                }
                
                yield return new WaitForSeconds(0.01f);
            }
        }

        isDoneCreatingCubes = true;
        gameDynamics.SetGameStartedFirstTime(true);
    }
    // Update is called once per frame
    void Update()
    {
        if(! isDoneCreatingCubes)
        {
            return;
        }

        bool calculationDone = gameDynamics.Update();

        if (Input.GetMouseButtonDown(0) && calculationDone)
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                   if(hit.transform.name == "Cube")
                   {
                       soundEffectsManager.PlayShooting();
                       hit.transform.gameObject.GetComponent<CubeBehavior>().Hit(gameDynamics, this);
                       Camera.main.GetComponent<Animator>().SetTrigger("IsCubeShot");
                       gunFire.StartFire(hit.transform.gameObject);
                   }
            }
        }
    }

    public Colors[] GetBasicColors()
    {
        return basicColors;
    }

    public Dictionary<Colors, Material> GetMaterials()
    {
        return materials;
    }

    public Dictionary<Colors, AnimationClip> GetAnimations()
    {
        return animations;
    }

    public GameDynamics GetGameDynamics()
    {
        return gameDynamics;
    }
}

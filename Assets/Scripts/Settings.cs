using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Settings : MonoBehaviour
{
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
    private Dictionary<Colors, float> probabilites = new Dictionary<Colors, float>();
    private Dictionary<Colors, string> specialPowers = new Dictionary<Colors, string>() {{Colors.Red, "RedBehavior"}, {Colors.White, "WhiteBehavior"}};

    private GameDynamics gameDynamics = new GameDynamics();

    void Awake()
    {
        if (BoardWidth == 0 ||
           BoardHeight == 0 ||
           WhiteCubesProbability == 0 ||
           RedCubesProbability == 0 ||
           PointsPerDestroyedCube == 0)
        {
            //Debug.LogError("One of game settings variables equals 0. Please check settings.");

            #if UNITY_EDITOR
                        // does not work in the editor
                        // Application.Quit(); 
                        //UnityEditor.EditorApplication.isPlaying = false;
            #else
                            //Application.Quit();
            #endif
        }

        gameDynamics.Init(BoardWidth, BoardHeight);

        // initialize the materials
        foreach (Colors color in Enum.GetValues(typeof(Colors)))
        {
            materials[color] = Resources.Load($"Materials/{color.ToString()}", typeof(Material)) as Material;
        }

        // set probabilites
        probabilites[Colors.Red] = RedCubesProbability;
        probabilites[Colors.White] = WhiteCubesProbability;
        float othersProbability = (1.0f - RedCubesProbability - WhiteCubesProbability) / 4;
        probabilites[Colors.Cyan] = probabilites[Colors.Yellow] = probabilites[Colors.Magenta] = probabilites[Colors.Black] = othersProbability;

        // special powers
    }
    IEnumerator Start()
    {
        var camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
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
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Entering Update");
        bool calculationDone = gameDynamics.Update();

        if (Input.GetMouseButtonDown(0) && calculationDone)
        {
            Debug.Log("Entering mouse if.");
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                   if(hit.transform.name == "Cube")
                   {
                       Debug.Log("Hit a cube!");
                       hit.transform.gameObject.GetComponent<CubeBehavior>().Hit(gameDynamics, this);
                   }
            }
            Debug.Log("Ending mouse if.");
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

    public GameDynamics GetGameDynamics()
    {
        return gameDynamics;
    }
}

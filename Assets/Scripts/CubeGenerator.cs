using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for generating cubes. It contains all needed information and the methods
/// for cubes creation. It creates ready-to-use cubes for that contains everything, such as components based on the color, animation, material and etc.
/// </summary>
public class CubeGenerator
{

    // ##############
    // ## Privates ##
    // ##############

    private Colors[] basicColors = new Colors[] { Colors.Cyan, Colors.Yellow, Colors.Magenta, Colors.Black };

    private Dictionary<Colors, string> specialPowers = new Dictionary<Colors, string>() { { Colors.Red, "RedBehavior" }, { Colors.White, "WhiteBehavior" } };

    private Dictionary<Colors, Material> materials = new Dictionary<Colors, Material>();
    private Dictionary<Colors, AnimationClip> animations = new Dictionary<Colors, AnimationClip>();
    private Dictionary<Colors, float> probabilites = new Dictionary<Colors, float>();

    RuntimeAnimatorController animatorOverrideController = null;

    // #################
    // ## Colors Enum ##
    // #################

    public enum Colors
    {
        Cyan,
        Yellow,
        Magenta,
        Black,
        Red,
        White,
    }

    // ###############
    // ## Constants ##
    // ###############
    private const string CUBE_PREFAB_PATH = "Prefabs/Cube";
    private const string MATERIALS_SUB_PATH = "Materials/";
    private const string ANIMATIONS_SUB_PATH = "Animations/";
    private const string ANIMATIONS_SUFFIX = "Emission";
    private const string CUBE_ON_WALL_ID = "CubeOnWall";

    private const string EMISSION_ANIMATION_CONTROLLER_PATH = "Animations/Emission";

    // ###############
    // ## Methods   ##
    // ###############

    /// <summary>
    /// It performs the following:
    /// (1) loads meterials for all colors.
    /// (2) loads animations for all colors.
    /// (3) calculates probabilities of each color to appear in-game.
    /// </summary>
    public void Initialize()
    {
        //////////////////////////////
        // initialize the materials //
        //////////////////////////////

        foreach (Colors color in Enum.GetValues(typeof(Colors)))
        {
            Material colorMaterial = Resources.Load(MATERIALS_SUB_PATH + color.ToString(), typeof(Material)) as Material;
            if (colorMaterial == null)
            {
                throw new Exception($"Failed to load color material file=[{MATERIALS_SUB_PATH + color.ToString()}].");
            }

            materials[color] = colorMaterial;
        }

        ///////////////////////////////
        // initialize the animations //
        ///////////////////////////////

        foreach (Colors color in Enum.GetValues(typeof(Colors)))
        {
            AnimationClip colorAnimation = Resources.Load(ANIMATIONS_SUB_PATH + color.ToString() + ANIMATIONS_SUFFIX, typeof(AnimationClip)) as AnimationClip;
            if (colorAnimation == null)
            {
                throw new Exception($"Failed to load color animation clip file=[{ANIMATIONS_SUB_PATH + color.ToString() + ANIMATIONS_SUFFIX}].");
            }

            animations[color] = colorAnimation;
        }

        //////////////////////////////
        // initialize probabilities //
        //////////////////////////////

        probabilites[Colors.Red] = GameManager.Instance.GameConfigs.RedCubesProbability;
        probabilites[Colors.White] = GameManager.Instance.GameConfigs.WhiteCubesProbability;

        float basicColorProabilityForEach = (1.0f - GameManager.Instance.GameConfigs.RedCubesProbability - GameManager.Instance.GameConfigs.WhiteCubesProbability) / 4;
        probabilites[Colors.Cyan] = probabilites[Colors.Yellow] = probabilites[Colors.Magenta] = probabilites[Colors.Black] = basicColorProabilityForEach;

        // loads emission animation controller since it's going to be used many  times.
        animatorOverrideController = Resources.Load(EMISSION_ANIMATION_CONTROLLER_PATH) as RuntimeAnimatorController;
    }


    /// <summary>
    /// This function generates a cube gameobject with it's respective material and animation already added to its components.
    /// The color of the cube will be determined based on the probability of it to appear.
    /// Cubes colors with special abilities will replace the base class "CubeBehaviour" with the relevant class that inherits base class.
    /// </summary>
    public GameObject GenerateCube(uint col, uint row)
    {
        // calculating camera view width and height.
        var height = 2*Camera.main.orthographicSize;
        var width = height*Camera.main.aspect;

        // calculating the starting X coord position.
        // Note: (wall_width/2)*(-1) should give us the starting X coord, that will make the wall be on screen center when creating all cubes.
        var startingX = (-1.0f)*(GameManager.Instance.GameConfigs.BoardWidth)/2;

        // load prefab cube.
        GameObject cubePrefab = Resources.Load(CUBE_PREFAB_PATH) as GameObject;

        // set position based on the coord.
        Vector3 cubePos = new Vector3(startingX + col, height + row, 10);

        // Instantiate new cube onn the wall!
        // NOTE: the CUBE_ON_WALL_ID will be used to identify cubes which is part of the cubes wall.
        var newObj = GameObject.Instantiate(cubePrefab, cubePos, Quaternion.identity) as GameObject;
        newObj.name = CUBE_ON_WALL_ID;
        newObj.GetComponent<Rigidbody>().freezeRotation = true; //rotation of cube might break the wall in some collision scenarios.

        // generate random float between 0 and 1. This will be used to find which color to create.
        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();

        // How color is being determined?
        // Example:assuming -> red=5% - white=15% - others=20% each.
        //             putting this on 0-1 scale:
        //             red=0.0->0.05, white=0.05->0.2, others=0.2->0.4, 0.4->0.6, 0.6->0.8, 0.8->1
        //          So when generating random number between 0-1, we need to check into what range it falls into.
        //          PLEASE NOTE: order of the cube colors in the way we iterate over the probabilites doesn't matter as long as it stays the same for all the run.

        float accumulatedChance = 0.0f;
        foreach (var prob in probabilites)
        {
            Colors currentColor = prob.Key;

            if (randomNum < accumulatedChance + prob.Value)
            {
                // we need material to set the base color for the animation.
                newObj.GetComponent<MeshRenderer>().material = materials[currentColor];

                // animation to make cube glow on and off.
                ReplaceAnimationClip(newObj, animations[currentColor]);

                // add and get behaviour based on color, and update cube x and y.
                AddBehaviour(newObj, currentColor).UpdateCoords(col, row);

                break;
            }

            accumulatedChance += prob.Value;
        }
    
        return newObj;
    }


    /// <summary>
    /// This function rolls a random basic color, and returns its material and animationClip through function parameters.
    /// </summary>
    public void GetRandomBasicColor(out Material colorMaterial, out AnimationClip colorAnimation)
    {
        System.Random random = new System.Random();
        Colors choosenColor = basicColors[random.Next(0, basicColors.Length)];
        colorMaterial = materials[choosenColor];
        colorAnimation = animations[choosenColor];
    }

    // This functions replaces cube animation clip.
    private void ReplaceAnimationClip(GameObject gameObject, AnimationClip animClip)
    {
        Animator newAnim = gameObject.AddComponent<Animator>();
        AnimatorOverrideController animOverride = new AnimatorOverrideController(animatorOverrideController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(animatorOverrideController.animationClips[0], animClip));
        animOverride.ApplyOverrides(anims);
        newAnim.runtimeAnimatorController = animOverride;
    }

    // This function adds the behavior based on cube color. Some cube colors has special powers (different behavior class).
    private CubeBehavior AddBehaviour(GameObject gameObject, Colors color)
    {
        CubeBehavior cubeBehavior;

        if (specialPowers.ContainsKey(color))
        {
            cubeBehavior = gameObject.AddComponent(Type.GetType(specialPowers[color])) as CubeBehavior;
        }
        else
        {
            cubeBehavior = gameObject.AddComponent<CubeBehavior>();
        }

        return cubeBehavior;
    }
}

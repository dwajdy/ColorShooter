using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator
{

    // #################
    // ## Member Vars ##
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

    private Colors[] basicColors = new Colors[] { Colors.Cyan, Colors.Yellow, Colors.Magenta, Colors.Black };

    private Dictionary<Colors, string> specialPowers = new Dictionary<Colors, string>() { { Colors.Red, "RedBehavior" }, { Colors.White, "WhiteBehavior" } };

    private Dictionary<Colors, Material> materials = new Dictionary<Colors, Material>();
    private Dictionary<Colors, AnimationClip> animations = new Dictionary<Colors, AnimationClip>();
    private Dictionary<Colors, float> probabilites = new Dictionary<Colors, float>();

    public void Initialize()
    {
        //////////////////////////////
        // initialize the materials //
        //////////////////////////////

        foreach (Colors color in Enum.GetValues(typeof(Colors)))
        {
            Material colorMaterial = Resources.Load($"Materials/{color.ToString()}", typeof(Material)) as Material;
            if (colorMaterial == null)
            {
                throw new Exception($"Failed to load color material file=[Materials/{color.ToString()}].");
            }

            materials[color] = colorMaterial;
        }

        ///////////////////////////////
        // initialize the animations //
        ///////////////////////////////

        foreach (Colors color in Enum.GetValues(typeof(Colors)))
        {
            AnimationClip colorAnimation = Resources.Load($"Animations/{color.ToString()}Emission", typeof(AnimationClip)) as AnimationClip;
            if (colorAnimation == null)
            {
                throw new Exception($"Failed to load color animation clip file=[Animations/{color.ToString()}Emission].");
            }

            animations[color] = colorAnimation;
        }

        //////////////////////////////
        // initialize probabilities //
        //////////////////////////////

        probabilites[Colors.Red] = GameManager.Instance.RedCubesProbability;
        probabilites[Colors.White] = GameManager.Instance.WhiteCubesProbability;

        float basicColorProabilityForEach = (1.0f - GameManager.Instance.RedCubesProbability - GameManager.Instance.WhiteCubesProbability) / 4;
        probabilites[Colors.Cyan] = probabilites[Colors.Yellow] = probabilites[Colors.Magenta] = probabilites[Colors.Black] = basicColorProabilityForEach;
    }

    public GameObject GenerateCube(uint col, uint row)
    {
        var height = 2*Camera.main.orthographicSize;
        var width = height*Camera.main.aspect;
        var startingX = (-1.0f)*(GameManager.Instance.BoardWidth)/2;

        GameObject cubePrefab = Resources.Load("Prefabs/Cube") as GameObject;
        Vector3 cubePos = new Vector3(startingX + col, height + row, 10);

        var newObj = GameObject.Instantiate(cubePrefab, cubePos, Quaternion.identity) as GameObject;
        newObj.name = "CubeOnWall";
        newObj.GetComponent<Rigidbody>().freezeRotation = true;

        System.Random random = new System.Random();
        float randomNum = (float)random.NextDouble();

        float accumulatedChance = 0.0f;
        foreach (var prob in probabilites)
        {
            if (randomNum < accumulatedChance + prob.Value)
            {
                CubeBehavior cubeBehavior;

                newObj.GetComponent<MeshRenderer>().material = materials[prob.Key];

                Animator newAnim = newObj.AddComponent<Animator>();
                RuntimeAnimatorController animatorOverrideController = Resources.Load("Animations/Emission") as RuntimeAnimatorController;
                AnimatorOverrideController animOverride = new AnimatorOverrideController(animatorOverrideController);

                var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(animatorOverrideController.animationClips[0], animations[prob.Key]));
                animOverride.ApplyOverrides(anims);

                newAnim.runtimeAnimatorController = animOverride;

                if (specialPowers.ContainsKey(prob.Key))
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
    
        return newObj;
    }

    public void GetRandomBasicColor(out Material colorMaterial, out AnimationClip colorAnimation)
    {
        System.Random random = new System.Random();
        Colors choosenColor = basicColors[random.Next(0, basicColors.Length)];
        colorMaterial = materials[choosenColor];
        colorAnimation = animations[choosenColor];
    }
}

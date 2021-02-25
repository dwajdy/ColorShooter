using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GalacticColorShooter/Configs")]
public class Configs : ScriptableObject 
{
        // #############################
    // ## Unity Script Parameters ##
    // #############################

    [Header("Basic Settings")]
    public uint BoardWidth;
    public uint BoardHeight;
    public float WhiteCubesProbability;
    public float RedCubesProbability;
    public  uint PointsPerDestroyedCube;

    [Header("Extras")]
    public bool FirstPersonCameraEffect;
    public uint PointsPerShot;


}


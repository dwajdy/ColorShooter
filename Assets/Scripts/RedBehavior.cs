using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents the red cubes behaviour. It contains an override for the Hit() function to implement special powers.
/// </summary>
public class RedBehavior : CubeBehavior
{
    public override void Hit(CubesWallHandler cubesWallHandler, CubeGenerator cubeGenerator) 
    {
        for(uint row = y==0 ? 0 : y-1; row <= y+1; ++row)
        {
            for(uint col = x==0 ? 0 :x-1; col <= x+1; ++col)
            {
                cubesWallHandler.Remove(col, row, col==x && row==y ? false : true);
            }
        }
    }
}

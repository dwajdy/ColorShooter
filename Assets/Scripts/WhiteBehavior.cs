using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBehavior : CubeBehavior
{    public override void Hit(CubesWallHandler cubesWallHandler, CubeGenerator cubeGenerator) 
    {
        
        cubeGenerator.GetRandomBasicColor(out var colorMaterial, out var colorAnimation);
        
        for(uint row = y==0 ? 0 : y-1; row <= y+1; ++row)
        {
            for(uint col = x==0 ? 0 :x-1; col <= x+1; ++col)
            {
                cubesWallHandler.Replace(col, row, colorMaterial, colorAnimation, "CubeBehavior");
            }
        }

        cubesWallHandler.Remove(x, y);
    }
}

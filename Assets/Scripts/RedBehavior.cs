using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBehavior : CubeBehavior
{
    public override void Hit(GameDynamics gameDynamics, Settings gameSettings) 
    {
        for(uint row = y==0 ? 0 : y-1; row <= y+1; ++row)
        {
            for(uint col = x==0 ? 0 :x-1; col <= x+1; ++col)
            {
                gameDynamics.Remove(col, row);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBehavior : CubeBehavior
{    public override void Hit(GameDynamics gameDynamics, Settings gameSettings) 
    {
        Settings.Colors[] basicColors = gameSettings.GetBasicColors();
        Dictionary<Settings.Colors, Material> materials = gameSettings.GetMaterials();
        Dictionary<Settings.Colors, AnimationClip> animations = gameSettings.GetAnimations();

        System.Random random = new System.Random();
        Settings.Colors choosenColor = basicColors[random.Next(0, basicColors.Length)];

        for(uint row = y==0 ? 0 : y-1; row <= y+1; ++row)
        {
            for(uint col = x==0 ? 0 :x-1; col <= x+1; ++col)
            {
                gameDynamics.Replace(col, row, materials[choosenColor], animations[choosenColor], "CubeBehavior");
            }
        }

        gameDynamics.Remove(x, y);
    }
}

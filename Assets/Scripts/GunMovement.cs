using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents the gun movement. It makes gun follow user mouse pointer.
/// </summary>
public class GunMovement : MonoBehaviour
{
    // ###############
    // ## Constants ##
    // ###############
    private const int CUBES_WALL_Z_COORD = 10;

    // ###############
    // ## Methods   ##
    // ###############

    // Make camera look at the mouse (after translating the mouse coords into world points)
    void Update()
    {
        Vector3 mousePosOnCubesWall = new Vector3(Input.mousePosition.x,Input.mousePosition.y, CUBES_WALL_Z_COORD - Camera.main.transform.position.z);
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(mousePosOnCubesWall);
        this.transform.LookAt(aimPos);
    }
}

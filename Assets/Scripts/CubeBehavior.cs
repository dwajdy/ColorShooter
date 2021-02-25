using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for all cubes in game. It mainly exposes two public methods, one to update it's coords
//  and the other implements "hit" operation on that cube. By that, each cube is independent in terms of what operations
//  to do when it is hit. (please see also RedBehaviour and WhiteBehavior classes that inherit it).
/// </summary>
public class CubeBehavior : MonoBehaviour
{
    // ###############
    // ## Protected ##
    // ###############
    // NOTE: in this class we define x and y as protected to allow 
    //       classes that inherit this class, to acces them.

    protected uint x;
    protected uint y;

    // ###############
    // ## Methods   ##
    // ###############

    /// <summary>
    /// Updates the x and y coords of a cube.
    //  Please note that x and y doesn't represent in-game scene coords, they are the indexes on the cubes walls caluclation matrix.
    /// </summary>
    public void UpdateCoords(uint x, uint y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Represents the action to take once a cube is shot.
    /// For basic cubes, the default behaviour is to remove the cube. 
    /// Other classes might override this behaviour such as red and white cubes.
    /// </summary>
    virtual public void Hit(CubesWallHandler cubesWallHandler, CubeGenerator cubeGenerator)
    {
        cubesWallHandler.Remove(x, y);
    }
}

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
    protected uint x;
    protected uint y;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCoords(uint x, uint y)
    {
        this.x = x;
        this.y = y;
    }
    virtual public void Hit(CubesWallHandler cubesWallHandler, CubeGenerator cubeGenerator)
    {
        cubesWallHandler.Remove(x, y);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///   This class is responsible to create the cubes wall, and keep them updated. It 
///   implements the core game play rules (match-3) and exposes public methods to
///   allow removing and replacing cubes, to allow cubes communicate with it when hit.
/// </summary>
public class CubesWallHandler
{
    // #############################
    // ## Privates: classes used. ##
    // #############################

    private CubeGenerator cubeGenerator = new CubeGenerator();

    // ##############################
    // ## Privates: algorithm vars ##
    // ##############################

    private GameObject[,] cubesMatrix = null;
    private const int TIME_TO_WAIT_AFTER_REMOVE= 1000;
    private const float TIME_TO_WAIT_BETWEEN_CUBES_CREATION = 0.01f;

    // #####################
    // ## Privates: flags ##
    // #####################
    private bool removeOperationPerformed = false; // for optimization


    // ####################
    // ## Public: flags  ##
    // ####################
    public bool IsGameOver {get; set;} = false;
    public bool IsGameReady {get; private set;} = false;
    public bool IsDoneCreatingCubes {get; private set;} = false;
    public bool IsGameStartedFirstTime {get; private set;} = false;
    public bool IsNoCubesLeft {get; private set;} = false;
    public uint Score {get; private set;} = 0;
    public DateTime TimeOfPreviousClick {get; private set;}
    
    
    // ###############
    // ## Methods   ##
    // ###############

    /// <summary>
    ///  Initializes the cubes matrix based on the given width and height from config. And initailize the CubeGenerator.
    /// </summary>
    public void Initialize()
    {
        cubesMatrix = new GameObject[GameManager.Instance.BoardWidth, GameManager.Instance.BoardHeight];
        cubeGenerator.Initialize();
    }


    /// <summary>
    ///  Replaces cube's material, animation and behavior script on a given x, y.
    /// </summary>
    public void Replace(uint x, uint y, Material newMaterial, AnimationClip newAnimaion, string newBehaviorTypeName)
    {
        if(x < 0 || x >= GameManager.Instance.BoardWidth || y < 0 || y >= GameManager.Instance.BoardHeight || cubesMatrix[x, y] == null)
        {
            return;
        }
        cubesMatrix[x, y].GetComponent<MeshRenderer>().material = newMaterial;
        UnityEngine.Object.Destroy(cubesMatrix[x, y].GetComponent<CubeBehavior>());
        ((CubeBehavior)cubesMatrix[x, y].AddComponent(Type.GetType(newBehaviorTypeName))).UpdateCoords(x,y);
        //cubesMatrix[x, y].GetComponent<CubeBehavior>().UpdateCoords(x,y); // since we destroyed old one, update the new created one with the coords - update: not working
        // learning!!!!! Destory doesn't remove immediately. so what happended here is that when I called GetComponent<CubeBehavior> I was gettig the old one! hence, updating
        // x and y was performed on the old one, not the new added one!!!!!!!!!! that's why when I called UpdateCoords() on the AddComponent() it worked, because it's guaranteed
        // that it takes the new one. Note that there's another version of Destory called DestroyImmediate (see https://docs.unity3d.com/ScriptReference/Object.DestroyImmediate.html#:~:text=In%20game%20code%20you%20should,executed%20within%20the%20same%20frame).)
    
        var anim = cubesMatrix[x, y].GetComponent<Animator>();
        var animOverride = anim.runtimeAnimatorController as AnimatorOverrideController;
        animOverride["EmissionPlaceholder"] = newAnimaion;

        AudioManager.Instance.PlayEffect(AudioManager.SoundEffect.REPLACE);
    }
    
    /// <summary>
    /// Removes a cube from matrix at given (x,y). The second parameter tells whether to increase score or not.
    /// However, note that if 'increaseScore' parameter is false, code will increase score if game config 'extras' specifies 'PointsPerShot'.
    /// </summary>
    public void Remove(uint x, uint y, bool increaseScore = false)
    {
        if(x < 0 || x >= GameManager.Instance.BoardWidth || y < 0 || y >= GameManager.Instance.BoardHeight || cubesMatrix[x, y] == null)
        {
            return;
        }

        UnityEngine.Object.Destroy(cubesMatrix[x, y]);
        cubesMatrix[x, y] = null;
        removeOperationPerformed = true;
        TimeOfPreviousClick = System.DateTime.Now;
        
        if(IsGameReady)
        {
            if(increaseScore)
            {
                Score += GameManager.Instance.PointsPerDestroyedCube;
                AudioManager.Instance.PlayEffect(AudioManager.SoundEffect.SCORE);
            }
            else
            {
                Score += GameManager.Instance.PointsPerShot;
            }
        }
        else // if(! IsGameReady)
        {
            AudioManager.Instance.PlayEffect(AudioManager.SoundEffect.COLLAPSE);
        }

    }

    /// <summary>
    /// This function will create cubes wall based game configs, and update game state flags./
    /// Note: this function implemented to allow being called from coroutine, because between each cube creation we wait small fraction.
    /// </summary>
    public IEnumerator CreateCubesWall()
    {
        // reset flags and cubes matrix.
        Reset();

        IsDoneCreatingCubes = false; // this is checked by GameManager Update() function. when false, Update() skips.

        // simply iterate over width and height indexes and create cubes using CubeGenerator.
        for(uint row = 0; row < GameManager.Instance.BoardHeight; ++row)
        {
            for(uint col = 0; col < GameManager.Instance.BoardWidth; ++col)
            {
                GameObject newCube = cubeGenerator.GenerateCube(col, row);
                this.cubesMatrix[col,row] = newCube;
                yield return new WaitForSeconds(TIME_TO_WAIT_BETWEEN_CUBES_CREATION);
            }
        }

        IsDoneCreatingCubes = true;

        IsGameStartedFirstTime = true; //only set once. no changing back later.
    }

    /// <summary>
    /// Tells if an objects belongs to the cubes wall.
    /// </summary>
    public bool IsCubeOnWallHit(GameObject objectHit)
    {
        return objectHit.name == "CubeOnWall";
    }

    /// <summary>
    /// Calls Hit(...) method on the cube to make some behavior actions.
    /// Remember different colors can have different behaviors.
    /// </summary>
    public void HandleCubeHit(GameObject objectHit)
    {
        objectHit.GetComponent<CubeBehavior>().Hit(this, cubeGenerator);
    }

    /// <summary>
    ///   This functions updates cubes wall by looking for 3+ matching sequences.
    /// </summary>
    /// <returns> 
    ///   true will tell the GameManager that you can take input, false tells GameManager not to take inputs and wait for cubes wall update to settle.
    /// </returns>
    public bool Update()
    {

        //---------------------------------------------------------------------------------------------------------------------------------
        // NOTE: It would be wise in this function to use try/catch and throw exceptions instead of reading return values.
        //       But since overall in the project I didn't use exceptions to handle failures, I decided not to use here, for consistency.
        //----------------------------------------------------------------------------------------------------------------------------------


        // after removal of cube, wait some time to make a better looking effect, 
        // otherwise it will look like we're removing cubes which doesn't form 3+ match.
        if( (System.DateTime.Now - TimeOfPreviousClick).TotalMilliseconds < TIME_TO_WAIT_AFTER_REMOVE ||
            IsGameOver || !IsGameStartedFirstTime)
        {
            return false;
        }

        if(CheckIfCubesAreMoving()) // it also updates IsNoCubesLeft (optimization. No need to iterate over the matrix twice)
        {
            // if moving cubes found, then return false. 
            // --> we need to tell GameManager that calculation is ongoing, don't take input
            return false;
        }

        if(IsNoCubesLeft)
        {
            IsNoCubesLeft = true;
            IsGameOver = true;
            return true;
        }

        MakeCubesCollapseAfterRemoval();

        if(LookForMatch())
        {
            // if matching found, then return false. 
            // --> because if match found, then we removed cubes, we need to tell GameManager that calculation is ongoing, don't take input
            return false;
        }

        IsGameReady = true;
        return true;
    }

    // this functions simply looks for 3+ matching colors. The colors is based on the color in the Material of the game object.
    private bool LookForMatch()
    {
        // look for 3 in a row or col
        for (int x = 0; x < GameManager.Instance.BoardWidth; ++x)
        {
            for (int y = cubesMatrix.GetLength(1) - 1; y >= 0; --y)
            {
                if(cubesMatrix[x, y] == null)
                {
                    continue;
                }

                // -------------------------
                // VERTICAL 3 MATCH LOOKUP
                //--------------------------

                int runX = x + 1;
                while (runX < GameManager.Instance.BoardWidth &&
                      cubesMatrix[runX, y] != null &&
                      cubesMatrix[x, y].GetComponent<MeshRenderer>().material.name.Equals(cubesMatrix[runX, y].GetComponent<MeshRenderer>().material.name))
                {
                    ++runX;
                }

                if ((runX - x) >= 3)
                {
                    for (int x_ = x; x_ < runX; ++x_)
                    {
                        Remove((uint)x_, (uint)y, true);
                    }

                    return true;
                }

                // -------------------------
                // HORIZONTAL 3 MATCH LOOKUP
                //--------------------------

                int runY = y - 1;
                while (runY >= 0 &&
                      cubesMatrix[x, runY] != null &&
                      cubesMatrix[x, y].GetComponent<MeshRenderer>().material.name.Equals(cubesMatrix[x, runY].GetComponent<MeshRenderer>().material.name))
                {
                    --runY;
                }

                if ((y - runY) >= 3)
                {
                    for (int y_ = y; y_ > runY; --y_)
                    {
                        Remove((uint)x, (uint)y_, true);
                    }

                    return true;
                }
            }
        }

        return false;
    }

    // checks whether there are any moving cubes on the wall. If found, we need to make sure GameManager knows in order not to take input from user.
    private bool CheckIfCubesAreMoving()
    {
        IsNoCubesLeft = true; //until proven otherwise
        foreach(GameObject obj in cubesMatrix) //simply iterate over all object, no importane for specific order.
        {
            if(obj == null)
            {
                continue;
            }

            IsNoCubesLeft = false;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if(rb.velocity.magnitude > 0)
            {
                // movement found!
                return true;
            }
        }

        return false;
    }

    // look for holes in the cubes matrix (as a results of the shooting). If found, we need to shift down cubes if there are any.
    private void MakeCubesCollapseAfterRemoval()
    {
        // after making sure everthing is settled, update matrix by bubbling null cube up
        if (removeOperationPerformed)
        {
            for (uint x = 0; x < cubesMatrix.GetLength(0); ++x)
            {
                for (int y = cubesMatrix.GetLength(1)-1; y >= 0 ; y--)
                {
                    if(cubesMatrix[x, y] == null)
                    {
                        ShiftDownCubes(x, (uint)y + 1);
                    }
                }
            }

            removeOperationPerformed = false;
        }
    }

    // clears cubes matrix, and reset flags.
    private void Reset()
    {
        Score = 0;
        IsGameOver = false;
        IsGameReady = false;
        IsNoCubesLeft = false;

        for (uint x = 0; x < cubesMatrix.GetLength(0); ++x)
        {
            for (int y = cubesMatrix.GetLength(1) - 1; y >= 0; y--)
            {
                UnityEngine.Object.Destroy(cubesMatrix[x, y]);
            }
        }
    }

    // will shift down cubes until bubbling "null" cube up. This simply makes cubes fall to fill the shot cube position.
    private void ShiftDownCubes(uint column, uint startingRow)
    {
        if( startingRow >= GameManager.Instance.BoardHeight)
        {
            return;
        }

        for(uint y = startingRow; y < GameManager.Instance.BoardHeight; ++y)
        {
            cubesMatrix[column, y-1] = cubesMatrix[column, y];

            if(cubesMatrix[column, y-1] != null)
            {
                cubesMatrix[column, y-1].GetComponent<CubeBehavior>().UpdateCoords(column, y-1);
            }

            cubesMatrix[column, y] = null;
        }
    }
}

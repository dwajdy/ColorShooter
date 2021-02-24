using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [CubesWallHandler]
///     This class is responsible to create the cubes wall, and keep them updated. It 
///   implements the core game play rules (match-3) and expose public methods to
///   allow removing and replacing cubes, to allow cubes communicate with it when hit.
/// </summary>
public class CubesWallHandler
{
    private CubeGenerator cubeGenerator = new CubeGenerator();



    private GameObject[,] cubesMatrix = null;
    private bool removeOperationPerformed = false; // for optimization

    private bool gameIsReady = false;

    public bool IsDoneCreatingCubes {get; private set;} = false;

    private DateTime timeClick;
    private const int timeToWaitAfterRemove= 1000;

    public void Initialize()
    {
        cubesMatrix = new GameObject[GameManager.Instance.BoardWidth, GameManager.Instance.BoardHeight];
        cubeGenerator.Initialize();
    }

    public void Add(GameObject cube, uint x, uint y)
    {
        this.cubesMatrix[x,y] = cube;
    }
  
    private uint score = 0;
    private bool isGameOver = false;
    private bool isNoCubesLeft = false;

    private bool isGameStartedFirstTime = false;

    // todo: eliminate it but check what to do with GameReady
    public bool GetIsGameStartedFirstTime()
    {
        return isGameStartedFirstTime;
    }
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

        AudioManager.Instance.PlayReplace();
    }
    
    public void Remove(uint x, uint y, bool increaseScore = false)
    {
        if(x < 0 || x >= GameManager.Instance.BoardWidth || y < 0 || y >= GameManager.Instance.BoardHeight || cubesMatrix[x, y] == null)
        {
            return;
        }

        UnityEngine.Object.Destroy(cubesMatrix[x, y]);
        cubesMatrix[x, y] = null;
        removeOperationPerformed = true;
        timeClick = System.DateTime.Now;
        
        if(gameIsReady)
        {
            if(increaseScore)
            {
                score += GameManager.Instance.PointsPerDestroyedCube;
                AudioManager.Instance.PlayScoreIncrease();
            }
            else
            {
                score += GameManager.Instance.PointsPerShot;
            }
        }
        else // if(! gameIsReady)
        {
            AudioManager.Instance.PlayCollapse();
        }

    }

    internal void Reset()
    {
        score = 0;
        isGameOver = false;
        gameIsReady = false;
        isNoCubesLeft = false;

        for (uint x = 0; x < cubesMatrix.GetLength(0); ++x)
        {
            for (int y = cubesMatrix.GetLength(1) - 1; y >= 0; y--)
            {
                UnityEngine.Object.Destroy(cubesMatrix[x, y]);
            }
        }
    }

    public bool IsNoCubesLeft()
    {
        return isNoCubesLeft;
    }
    public uint GetScore()
    {
        return score;
    }

    public DateTime GetLastCubeHitTime()
    {
        return timeClick;
    }
    public bool GetIsGameOver()
    {
        return isGameOver && isGameStartedFirstTime;
    }

    public bool IsGameReady()
    {
        return gameIsReady;
    }
    public void SetIsGameOver(bool value)
    {
        isGameOver = value;
    }

    public IEnumerator CreateCubesWall()
    {
        IsDoneCreatingCubes = false;
        Reset();

        for(uint row = 0; row < GameManager.Instance.BoardHeight; ++row)
        {
            for(uint col = 0; col < GameManager.Instance.BoardWidth; ++col)
            {
                GameObject newCube = cubeGenerator.GenerateCube(col, row);
                Add(newCube, col, row);
                yield return new WaitForSeconds(0.01f);
            }
        }

        IsDoneCreatingCubes = true;
        isGameStartedFirstTime = true;
    }

    public bool IsCubeOnWallHit(GameObject objectHit)
    {
        return objectHit.name == "CubeOnWall";
    }

    public void HandleCubeHit(GameObject objectHit)
    {
        objectHit.GetComponent<CubeBehavior>().Hit(this, cubeGenerator);
    }

    public bool Update()
    {

        if( (System.DateTime.Now - timeClick).TotalMilliseconds < timeToWaitAfterRemove ||
            isGameOver || !isGameStartedFirstTime)
        {
            return false;
        }

        // todo: update matrix by removing nulls and shifting cubes
        //bool isNoCubesLeft = true;
        isNoCubesLeft = true; //until proven otherwise
        foreach(GameObject obj in cubesMatrix)
        {
            if(obj == null)
            {
                continue;
            }

            isNoCubesLeft = false;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if(rb.velocity.magnitude > 0)
            {
                return false;
            }
        }

        if(isNoCubesLeft)
        {
            isNoCubesLeft = true;
            isGameOver = true;
            return true;
        }

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

        // look for 3 in a row or col
        for (int x = 0; x < GameManager.Instance.BoardWidth; ++x)
        {
            for (int y = cubesMatrix.GetLength(1) - 1; y >= 0; --y)
            {
                if(cubesMatrix[x, y] == null)
                {
                    continue;
                }

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

                    return false;
                }

               
                int runY = y - 1;
                Debug.Log($"(x, y)=({x}, {y}) material={cubesMatrix[x, y].GetComponent<MeshRenderer>().material.name}");
                //Debug.Log($"(x, y)=({x}, {runY}) material={cubesMatrix[x, runY].GetComponent<MeshRenderer>().material.name}");

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

                    return false;
                }
            }
        }

        gameIsReady = true;
        return true;
    }

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

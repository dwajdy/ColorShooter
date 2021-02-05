using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDynamics
{
    private GameObject[,] cubesMatrix = null;
    private uint width;
    private uint height;
    private bool removeOperationPerformed = false; // for optimization

    private DateTime timeClick;
    private const int timeToWaitAfterRemove= 1000;

    public void Init(uint width, uint height)
    {
        this.width = width;
        this.height = height;
        cubesMatrix = new GameObject[width, height];
    }

    public void Add(GameObject cube, uint x, uint y)
    {
        this.cubesMatrix[x,y] = cube;
    }
  
    private uint score = 0;
    public void Replace(uint x, uint y, Material newMaterial, string newBehaviorTypeName)
    {
        if(x < 0 || x >= width || y < 0 || y >= height || cubesMatrix[x, y] == null)
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
    }
    
    public void Remove(uint x, uint y)
    {
        if(x < 0 || x >= width || y < 0 || y >= height || cubesMatrix[x, y] == null)
        {
            return;
        }
        UnityEngine.Object.Destroy(cubesMatrix[x, y]);
        cubesMatrix[x, y] = null;
        removeOperationPerformed = true;
        timeClick = System.DateTime.Now;
    }

    public void AddScore(uint addition)
    {
        score += addition;
    }

    public uint GetScore()
    {
        return score;
    }

    public bool Update()
    {
        Debug.Log("Entering Update");

        if( (System.DateTime.Now - timeClick).TotalMilliseconds < timeToWaitAfterRemove)
        {
            return false;
        }

        // todo: update matrix by removing nulls and shifting cubes
        foreach(GameObject obj in cubesMatrix)
        {
            if(obj == null)
            {
                continue;
            }

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if(rb.velocity.magnitude > 0)
            {
                return false;
            }
        }

        // after making sure everthing is settled, update matrix by bubbling null cube up
        if (removeOperationPerformed)
        {
            for (uint x = 0; x < cubesMatrix.GetLength(0); x++)
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
        for (int x = 0; x < cubesMatrix.GetLength(0); x++)
        {
            for (int y = cubesMatrix.GetLength(1) - 1; y >= 0; y--)
            {
                if (0 <= (x-1) && width > (x+1)) 
                {
                    if(cubesMatrix[x-1, y] != null &&
                       cubesMatrix[x, y] != null &&
                       cubesMatrix[x+1, y] != null &&
                       cubesMatrix[x-1, y].GetComponent<MeshRenderer>().material.name.Equals(cubesMatrix[x, y].GetComponent<MeshRenderer>().material.name)&&
                       cubesMatrix[x-1, y].GetComponent<MeshRenderer>().material.name.Equals(cubesMatrix[x+1, y].GetComponent<MeshRenderer>().material.name))
                       {
                           Remove((uint)(x-1), (uint)y);
                           Remove((uint)x,(uint)y);
                           Remove((uint)(x+1),(uint)y);
                           AddScore(3);
                           return false;
                       }
                }

                if (0 <= (y-1) && height > (y+1)) 
                {
                    if(cubesMatrix[x, y-1] != null &&
                       cubesMatrix[x, y] != null &&
                       cubesMatrix[x, y+1] != null &&
                       cubesMatrix[x, y-1].GetComponent<MeshRenderer>().material.name.Equals(cubesMatrix[x, y].GetComponent<MeshRenderer>().material.name)&&
                       cubesMatrix[x, y-1].GetComponent<MeshRenderer>().material.name.Equals(cubesMatrix[x, y+1].GetComponent<MeshRenderer>().material.name))
                       {
                           Remove((uint)x, (uint)(y-1));
                           Remove((uint)x,(uint)y);
                           Remove((uint)x,(uint)(y+1));
                           AddScore(3);
                           return false;
                       }
                }
            }
        }

        Debug.Log("Ending Update");
        return true;
    }

    private void ShiftDownCubes(uint column, uint startingRow)
    {
        if( startingRow >= height)
        {
            return;
        }

        for(uint y = startingRow; y < height; ++y)
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

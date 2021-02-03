using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDynamics
{
    private GameObject[,] cubesMatrix = null;
    private uint width;
    private uint height;

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
  
    public void Replace(uint x, uint y, Material newMaterial, string newBehaviorTypeName)
    {
        if(x < 0 || x >= width || y < 0 || y >= height || cubesMatrix[x, y] == null)
        {
            return;
        }
        cubesMatrix[x, y].GetComponent<MeshRenderer>().material = newMaterial;
        UnityEngine.Object.Destroy(cubesMatrix[x, y].GetComponent<CubeBehavior>());
        cubesMatrix[x, y].AddComponent(Type.GetType(newBehaviorTypeName));
    }
    
    public void Remove(uint x, uint y)
    {
        if(x < 0 || x >= width || y < 0 || y >= height || cubesMatrix[x, y] == null)
        {
            return;
        }
        UnityEngine.Object.Destroy(cubesMatrix[x, y]);
        cubesMatrix[x, y] = null;
    }

    public bool Update()
    {
        Debug.Log("Entering Update");
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

        Debug.Log("Ending Update");
        return true;
    }
}

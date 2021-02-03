using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    virtual public void Hit(GameDynamics gameDynamics, Settings gameSettings)
    {
        gameDynamics.Remove(x, y);
    }
}

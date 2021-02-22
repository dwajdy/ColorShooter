using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosOnCubesWall = new Vector3(Input.mousePosition.x,Input.mousePosition.y, 10 - Camera.main.transform.position.z);
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(mousePosOnCubesWall);
        this.transform.LookAt(aimPos);
    }
}

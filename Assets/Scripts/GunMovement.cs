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
        Vector3 vetor3 = new Vector3(Input.mousePosition.x,Input.mousePosition.y,10);
        Vector3 aimPos = Camera.main.ScreenToWorldPoint(vetor3);
        aimPos.z = this.transform.position.z + 10;
        this.transform.LookAt(aimPos);
        Debug.Log($"Aim position is: ({aimPos.x}, {aimPos.y}, {aimPos.z}).");
    }
}

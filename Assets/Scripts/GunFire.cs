﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : MonoBehaviour
{

    private LineRenderer fireLine;
    private Light fireLight;

    private const float shootingTime =0.2f;
    private float timeFromStartShooting = 0.0f;

    private float lightZAxisOffset = 2.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        fireLight = this.GetComponent<Light>();
        fireLine = this.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timeFromStartShooting += Time.deltaTime;
        if(timeFromStartShooting >= shootingTime)
        {
            StopFire();
        }
    }

    public void StopFire()
    {
        fireLine.enabled = false;
        fireLight.enabled = false;
    }

    public void StartFire(GameObject objectToShoot)
    {
        timeFromStartShooting = 0.0f;
        Vector3 lightStartingPos = transform.position;
        lightStartingPos.z += lightZAxisOffset;
        fireLine.SetPosition(0, lightStartingPos);
        fireLine.SetPosition(1, objectToShoot.transform.position);

        fireLight.color = objectToShoot.GetComponent<MeshRenderer>().material.color;

        fireLine.enabled = true;
        fireLight.enabled = true;
    }
}
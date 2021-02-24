using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class is added as component to gun game object. It is responsible for 
///   creating the shooting effect: lighting up gun head, and creating laser effect.
/// </summary>
public class GunFire : MonoBehaviour
{

    private LineRenderer fireLine;
    private Light fireLight;

    private GameObject LightEffectOnGun;
    private const float shootingTime =0.3f;
    private float timeFromStartShooting = 0.0f;

    private float lightZAxisOffset = 1.0f;
    
    void Start()
    {
        fireLight = this.GetComponent<Light>();
        fireLine = this.GetComponent<LineRenderer>();
        LightEffectOnGun = this.transform.GetChild(0).gameObject;
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
        LightEffectOnGun.SetActive(false);
    }

    public void StartFire(GameObject objectToShoot)
    {
        timeFromStartShooting = 0.0f;
        Vector3 lightStartingPos = transform.position;
        
        lightStartingPos.z += lightZAxisOffset;

        fireLine.SetPosition(0, lightStartingPos);
        fireLine.SetPosition(1, objectToShoot.transform.position);

        fireLight.color = objectToShoot.GetComponent<MeshRenderer>().material.color;

        LightEffectOnGun.SetActive(true);
        fireLine.enabled = true;
        fireLight.enabled = true;
    }
}

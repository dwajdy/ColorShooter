using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class is added as component to gun game object. It is responsible for 
///   creating the shooting effect: lighting up gun head, and creating laser effect.
/// </summary>
public class GunFire : MonoBehaviour
{
    // ##############
    // ## Privates ##
    // ##############

    // represents the laser line that we draw on gun shot
    private LineRenderer fireLine;

    // represents the light that is created as a result of the shot.
    // It changes it's color based on the cube color that user shot.
    private Light fireLight;

    // this represents light effect on the gun it self (light on the gun head).
    private GameObject LightEffectOnGun;
    

    // counting the time to know when to disable laster light.
    private float timeFromStartShooting = 0.0f;

    
    // ###############
    // ## Constants ##
    // ###############
    
    // the time to keep laster shown.
    private const float FIRING_EFFECT_TIME = 0.3f;

    // since we have special camera movement, this is an offset fix to make laser look better.
    private const float LIGHT_Z_AXIS_OFFSET_FIX = 1.0f;

    // ###############
    // ## Methods   ##
    // ###############

    // getting needed components and objects.
    void Start()
    {
        fireLight = this.GetComponent<Light>();
        fireLine = this.GetComponent<LineRenderer>();
        LightEffectOnGun = this.transform.GetChild(0).gameObject;
    }

    // Only thing it does is to check if fire need to be stopped, after someone has triggered it...
    void Update()
    {
        timeFromStartShooting += Time.deltaTime;
        if(timeFromStartShooting >= FIRING_EFFECT_TIME)
        {
            StopFire();
        }
    }

    // stopping fire effect means disabling laser, light effect on cubes wall and light effect on gun itself.
    public void StopFire()
    {
        fireLine.enabled = false;
        fireLight.enabled = false;
        LightEffectOnGun.SetActive(false);
    }

    // When GunHandler receives start fire request, it calls this function. 
    // This function will enable the 'fireing' effects and set the flags and timers.
    public void StartFire(GameObject objectToShoot)
    {
        timeFromStartShooting = 0.0f;
        Vector3 lightStartingPos = transform.position;
        
        // applying offset fix over z axis
        lightStartingPos.z += LIGHT_Z_AXIS_OFFSET_FIX;

        // setting start and end of laser line
        fireLine.SetPosition(0, lightStartingPos);
        fireLine.SetPosition(1, objectToShoot.transform.position);

        // setting the color light to the cube color that has been shot.
        fireLight.color = objectToShoot.GetComponent<MeshRenderer>().material.color;

        // enabling effects.
        LightEffectOnGun.SetActive(true);
        fireLine.enabled = true;
        fireLight.enabled = true;
    }
}

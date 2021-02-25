using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  This class adds the GunFire component, and responsible to start animation on the gun.
///  It's the class that the GameManager holds to communicate with the gun effect compoenent.
/// </summary>
public class GunHandler 
{

    // ###############
    // ## Privates ##
    // ###############
    GunFire gunFire = null;


    // ###############
    // ## Constants ##
    // ###############
    private const string GUN_PREFAB = "Prefabs/Gun";
    

    // ###############
    // ## Methods   ##
    // ###############

    /// <summary>
    ///  Instantiate gun gameobject, and add it under the main camera. (in order not to make it move with the movement effect that we use for the main camera)
    /// </summary>
    public void Initialize()
    {
        GameObject gunPrefab = Resources.Load(GUN_PREFAB) as GameObject;
        var gunObject = GameObject.Instantiate(gunPrefab, Camera.main.transform.position + gunPrefab.transform.position, Quaternion.identity) as GameObject;
        gunObject.transform.parent = Camera.main.transform;

        // adding compoenet for the firing effect.
        gunFire = GameObject.FindGameObjectWithTag("GunHead").AddComponent<GunFire>();
    }

    // start the firing effect.
    public void StartFire(GameObject objectToShoot)
    {
        gunFire.StartFire(objectToShoot);
    }
}

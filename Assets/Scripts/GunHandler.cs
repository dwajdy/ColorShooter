using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     This class adds the GunFire component, and responsible to start animation on the gun.
///  It's the class that the GameManager holds to communicate with the gun effect compoenent.
/// </summary>
public class GunHandler 
{

    GunFire gunFire = null;

    public void Initialize()
    {
        GameObject gunPrefab = Resources.Load("Prefabs/Gun") as GameObject;
        var gunObject = GameObject.Instantiate(gunPrefab, Camera.main.transform.position + gunPrefab.transform.position, Quaternion.identity) as GameObject;
        gunObject.transform.parent = Camera.main.transform;
        gunFire = GameObject.FindGameObjectWithTag("GunHead").AddComponent<GunFire>();
    }

    public void StartFire(GameObject objectToShoot)
    {
        gunFire.StartFire(objectToShoot);
    }
}

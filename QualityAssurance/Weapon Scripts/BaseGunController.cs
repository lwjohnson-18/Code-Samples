/*****************************************************************************
// File Name :         BaseGunController.cs
// Author :            Lucas Johnson
// Creation Date :     September 5, 2022
//
// Brief Description : A C# script that controls the base mechanics of the 
                       gun. Including shooting base projectiles.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGunController : MonoBehaviour
{
    [Header("Base Variables:")]
    public Transform firePoint;
    public float projectileForce = 20f;
    public float timeBetweenShots = 0.5f;
    public float destroyDelay = 5f;
    
    /// <summary>
    /// Time till next shot can be fired based on total game time
    /// </summary>
    private float nextShotTime;

    public bool DetectBeingHeld()
    {
        if(transform.parent != null && transform.parent.tag == "HoldingPoint")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Handles the firing of gun type weapons
    /// </summary>
    /// <param name="projectileType">What type of projectile should be fired</param>
    /// <param name="destroyAfterDelay">Should the projectile be destroyed after a delay</param>
    public void Shoot(GameObject projectileType, bool destroyAfterDelay = true)
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Limits the rate at which projectiles can be fired
            bool canShoot = Time.time > nextShotTime;

            if (canShoot)
            {
                // Spawn a projectile and add a forward force to it
                GameObject projectile = Instantiate(projectileType, firePoint.position, firePoint.rotation);

                if (!TestScan.complete && projectileType.GetComponent<ObjectTypeStats>())
                {
                    TestScan.complete = true;
                }

                if (!projectile.activeSelf)
                {
                    projectile.SetActive(true);
                }

                if(projectile.tag == "Paintball")
                {
                    PaintballModifier pbm = GetComponent<PaintballModifier>();
                    projectile.GetComponent<MeshRenderer>().material = pbm.paintMaterial;
                    pbm.PickRandomPaintColor();
                }

                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddForce(firePoint.forward * projectileForce, ForceMode.Impulse);

                // If the projectile should destroy after a delay...
                if (destroyAfterDelay)
                {
                    // Destroy the projectile after some time
                    Destroy(projectile, destroyDelay);
                }

                // Resets projectile delay timer
                nextShotTime = Time.time + timeBetweenShots;
            }
        }
    }
}

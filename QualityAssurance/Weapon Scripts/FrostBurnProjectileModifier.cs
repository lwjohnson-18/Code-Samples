/*****************************************************************************
// File Name :         FrostBurnProjectileModifier.cs
// Author :            Lucas johnson
// Creation Date :     September 11, 2022
//
// Brief Description : A C# script that modifies fired projectiles to have the
                       attributes of the FrostBurn weapon when colliding with
                       an object.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostBurnProjectileModifier : BaseProjectileController
{
    [Header("Modifier Variables:")]
    public bool fireProjectile; // True = Fire, False = Ice

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<ObjectTypeStats>())
        {
            ObjectTypeStats ots = collision.gameObject.GetComponent<ObjectTypeStats>();

            if(fireProjectile)
            {
                if (ots.isFrozen)
                {
                    ots.ResetObject();
                }
                else
                {
                    TestFire.complete = true;
                    ots.BurnObject();
                }
            }
            else
            {
                if (ots.isBurned)
                {
                    ots.ResetObject();
                }
                else
                {
                    TestIce.complete = true;
                    ots.FreezeObject();
                }
            }
        }

        if(destroyOnCollision)
        {
            Destroy(gameObject);
        }
    }

}

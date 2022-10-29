/*****************************************************************************
// File Name :         PaintProjectileModifier.cs
// Author :            Lucas johnson
// Creation Date :     October 16, 2022
//
// Brief Description : A C# script that modifies fired projectiles to have the
                       attributes of the Paintball Gun weapon when colliding 
                       with an object.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintProjectileModifier : BaseProjectileController
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<ObjectTypeStats>())
        {
            ObjectTypeStats ots = collision.gameObject.GetComponent<ObjectTypeStats>();

            ots.PaintObject();
        }

        if (destroyOnCollision)
        {
            Destroy(gameObject);
        }
    }
}

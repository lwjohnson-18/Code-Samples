/*****************************************************************************
// File Name :         BaseProjectileController.cs
// Author :            Lucas johnson
// Creation Date :     September 6, 2022
//
// Brief Description : A C# script that handles fired projectiles and how 
                       they should function.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectileController : MonoBehaviour
{
    [Header("Base Variables:")]
    public bool destroyOnCollision = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (destroyOnCollision)
        {
            Destroy(gameObject);
        }
    }
}

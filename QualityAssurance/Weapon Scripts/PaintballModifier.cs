/*****************************************************************************
// File Name :         PaintballModifier.cs
// Author :            Lucas johnson
// Creation Date :     October 16, 2022
//
// Brief Description : A C# script that modifies fired projectiles to have the
                       attributes of the Paintball weapon in air.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintballModifier : BaseGunController
{
    [Header("Modifier Variables:")]
    public GameObject paintballProjectile;
    public Material paintMaterial;

    [HideInInspector]
    public bool pickedUp = false;

    // Start is called before the first frame update
    void Start()
    {
        PickRandomPaintColor();
    }

    // Update is called once per frame
    void Update()
    {
        // Continue only if the gun is being held
        if (DetectBeingHeld())
        {
            Shoot(paintballProjectile);
            
            if(!pickedUp)
            {
                EndOfDayController.instance.productsTested[4] = true;
                pickedUp = true;
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                PickRandomPaintColor();
            }
        }
    }

    public void PickRandomPaintColor()
    {
        paintMaterial.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
    }
}

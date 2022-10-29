/*****************************************************************************
// File Name :         FrostBurnModifier.cs
// Author :            Lucas johnson
// Creation Date :     September 7, 2022
//
// Brief Description : A C# script that modifies fired projectiles to have the
                       attributes of the FrostBurn weapon in air.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostBurnModifier : BaseGunController
{
    [Header("Modifier Variables:")]
    public GameObject fireProjectile;
    public GameObject iceProjectile;

    public Material fireDisplayMaterial;
    public Material iceDisplayMaterial;

    [Space]

    public Material burnedMaterial;
    public Material frozenMaterial;

    public GameObject burnedParticles;
    public GameObject frozenParticles;

    public bool elementType = true; // True = Fire, False = Ice

    private MeshRenderer gunMesh;
    private ObjectiveController oc;
    private bool objectivesCollected = false;

    private void Start()
    {
        gunMesh = transform.GetChild(0).GetComponent<MeshRenderer>();
        oc = GameObject.FindGameObjectWithTag("ObjectiveController").GetComponent<ObjectiveController>();
    }

    private void Update()
    {
        // Continue only if the gun is being held
        if (DetectBeingHeld()) 
        {
            if(!objectivesCollected)
            {
                oc.CollectObjectives(transform.GetChild(1).gameObject, "FrostBurn");
                objectivesCollected = true;
            }

            HandleTypeSelection();

            if (elementType)
            {
                Shoot(fireProjectile);
            }
            else
            {
                Shoot(iceProjectile);
            }
        }
    }

    /// <summary>
    /// Handles input to select what element type is selected
    /// </summary>
    private void HandleTypeSelection()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            elementType = !elementType;

            if (elementType)
            {
                gunMesh.material = fireDisplayMaterial;
            }
            else
            {
                gunMesh.material = iceDisplayMaterial;
            }
        }
    }
}

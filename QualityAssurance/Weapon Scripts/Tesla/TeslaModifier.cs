/*****************************************************************************
// File Name :         TeslaModifier.cs
// Author :            Lucas johnson
// Creation Date :     October 5, 2022
//
// Brief Description : A C# script that modifies the BaseGunController to
                       apply to the Tesla gun's specific mechanics.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaModifier : BaseGunController
{
    [Header("Modifier Variables:")]
    [Tooltip("Distance to raycast from camera")]
    [SerializeField] [Range(1f, 5f)] public float hitRange = 5f;
    public float secBetweenChargeLevel = 1f;
    [Range(0, 4)]public int chargeLevel = 0;

    public bool chargingMode = true;
    public bool charging = false;

    public Material lightMaterial;
    public Material offMaterial;

    private int[] materialIndexes = { 2, 0, 6 };
    private MeshRenderer baseRenderer;
    private Material[] initBaseMaterials;
    private MeshRenderer rotorRenderer;

    private Transform playerCamera = null;
    private int playerMask = ~(1 << 6);

    private ObjectiveController oc;
    private bool objectivesCollected = false;

    private Transform displayParent;
    
    private GameObject electricityVFX;
    private Transform electricityEndPoint;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        oc = GameObject.FindGameObjectWithTag("ObjectiveController").GetComponent<ObjectiveController>();

        baseRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        initBaseMaterials = baseRenderer.materials;

        rotorRenderer = transform.GetChild(1).GetComponent<MeshRenderer>();

        displayParent = transform.GetChild(2);

        electricityVFX = transform.GetChild(0).GetChild(0).gameObject;
        electricityEndPoint = electricityVFX.transform.GetChild(6).transform;
        electricityVFX.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Continue only if the gun is being held
        if (DetectBeingHeld())
        {
            if (!objectivesCollected)
            {
                oc.CollectObjectives(transform.GetChild(3).gameObject, "Tesla Gun");
                objectivesCollected = true;
            }

            HandleModeSelection();

            // Draw a line in the inspector to show scan range
            Debug.DrawRay(playerCamera.position, playerCamera.forward * hitRange, Color.yellow);
            
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                // Shoot a raycast and check if it has an ObjectTypeStats
                if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, hitRange, playerMask))
                {
                    if (hitInfo.transform.GetComponent<ObjectTypeStats>())
                    {
                        ObjectTypeStats chargedOTS = hitInfo.transform.GetComponent<ObjectTypeStats>();

                        // If the object can be charged...
                        if (chargedOTS.charged && !chargedOTS.isFrozen && chargedOTS.currentScaleIndex == 3)
                        {
                            charging = true;

                            if (chargingMode)
                            {
                                StartCoroutine(TransferCharge(true, chargedOTS, hitInfo.point));
                            }
                            else
                            {
                                StartCoroutine(TransferCharge(false, chargedOTS, hitInfo.point));
                            }
                        }
                    }
                }
            }

            // If the player presses and holds the LMB...
            if (!Input.GetKey(KeyCode.Mouse0) && charging)
            {
                HideElectricityVFX();
                charging = false;
            }
        }
    }

    IEnumerator TransferCharge(bool take, ObjectTypeStats otherOTS, Vector3 impactPoint)
    {
        BatteryController bc = null;
        if ((chargeLevel < 4 && otherOTS.chargeLevel > 0) || (chargeLevel > 0 && otherOTS.chargeLevel < 4))
        {
            DisplayElectricityVFX(impactPoint);

            if(otherOTS.gameObject.GetComponent<BatteryController>())
            {
                bc = otherOTS.gameObject.GetComponent<BatteryController>();
            }
        }

        if(take)
        {
            while(charging && ((chargeLevel < 4 && otherOTS.chargeLevel > 0) && (Vector3.Distance(transform.position, otherOTS.transform.position) < hitRange)))
            {
                yield return new WaitForSeconds(secBetweenChargeLevel);
                if (charging)
                {
                    chargeLevel++;
                    otherOTS.chargeLevel--;
                    UpdateChargeVisuals();
                    if(bc)
                    {
                        bc.UpdateVisuals(otherOTS.chargeLevel);
                    }
                }
            }
        }
        else
        {
            while(charging && ((chargeLevel > 0 && otherOTS.chargeLevel < 4) && (Vector3.Distance(transform.position, otherOTS.transform.position) < hitRange)))
            {
                yield return new WaitForSeconds(secBetweenChargeLevel);
                if (charging)
                {
                    chargeLevel--;
                    otherOTS.chargeLevel++;
                    otherOTS.EnableElectronics();
                    UpdateChargeVisuals();
                    if (bc)
                    {
                        bc.UpdateVisuals(otherOTS.chargeLevel);
                    }
                }
            }
        }

        if(otherOTS.chargeLevel <= 0 && otherOTS.disableOnZeroCharge)
        {
            otherOTS.DisableElectronics(true);
        }
        else if(otherOTS.chargeLevel >= 4 && otherOTS.burnOnFullCharge)
        {
            if(otherOTS.tag.Equals("MurphyFace"))
            {
                BossFightController.instance.Attacked();
                otherOTS.chargeLevel = 0;
            }
            else
            {
                otherOTS.BurnObject();
            }
        }

        charging = false;
        HideElectricityVFX();

        yield return 0;
    }

    void DisplayElectricityVFX(Vector3 impactPoint)
    {
        electricityVFX.SetActive(true);
        electricityEndPoint.parent = null;
        electricityEndPoint.position = impactPoint;
    }

    void HideElectricityVFX()
    {
        electricityEndPoint.parent = electricityVFX.transform;
        electricityVFX.SetActive(false);
    }

    private void UpdateChargeVisuals()
    {
        baseRenderer.materials = initBaseMaterials;
        rotorRenderer.material = offMaterial;

        Material[] newMaterials = new Material[baseRenderer.materials.Length];
        for (int i = 0; i < baseRenderer.materials.Length; i++)
        {
            if(i == materialIndexes[0] && chargeLevel >= 1)
            {
                newMaterials[i] = lightMaterial;
            }
            else if (i == materialIndexes[1] && chargeLevel >=2)
            {
                newMaterials[i] = lightMaterial;
            }
            else if (i == materialIndexes[2] && chargeLevel >=3)
            {
                newMaterials[i] = lightMaterial;
            }
            else
            {
                newMaterials[i] = initBaseMaterials[i];
            }
        }
        baseRenderer.materials = newMaterials;

        if(chargeLevel >= 4)
        {
            rotorRenderer.material = lightMaterial;
        }
    }

    private void HandleModeSelection()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            chargingMode = !chargingMode;

            if(chargingMode)
            {
                displayParent.GetChild(0).gameObject.SetActive(true);
                displayParent.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                displayParent.GetChild(0).gameObject.SetActive(false);
                displayParent.GetChild(1).gameObject.SetActive(true);
            }
        }
    }
}

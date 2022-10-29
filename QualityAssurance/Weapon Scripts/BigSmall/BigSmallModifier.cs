/*****************************************************************************
// File Name :         BigSmallModifier.cs
// Author :            Lucas johnson
// Creation Date :     October 12, 2022
//
// Brief Description : A C# script that modifies the BaseGunController to
                       apply to the Big Small gun's specific mechanics.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSmallModifier : BaseGunController
{
    [Header("Modifier Variables:")]
    [Tooltip("Distance to raycast from camera")]
    [SerializeField] [Range(1f, 3f)] public float hitRange = 3f;

    public bool scalingMode = true;

    private Transform playerCamera = null;
    private int playerMask = ~(1 << 6);

    private ObjectiveController oc;
    private bool objectivesCollected = false;

    private Animator anim;

    private GameObject blueLazer;
    private GameObject redLazer;
    private Transform laserEndpoint;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        oc = GameObject.FindGameObjectWithTag("ObjectiveController").GetComponent<ObjectiveController>();
        anim = GetComponent<Animator>();
        
        blueLazer = transform.GetChild(2).gameObject;
        redLazer = transform.GetChild(3).gameObject;
        blueLazer.SetActive(false);
        redLazer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Continue only if the gun is being held
        if (DetectBeingHeld())
        {
            if (!objectivesCollected)
            {
                oc.CollectObjectives(transform.GetChild(1).gameObject, "Big Small Gun");
                objectivesCollected = true;
            }

            HandleModeSelection();

            // Draw a line in the inspector to show scan range
            Debug.DrawRay(playerCamera.position, playerCamera.forward * hitRange, Color.yellow);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                // Shoot a raycast and check if it has an ObjectTypeStats
                if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, hitRange, playerMask))
                {
                    if (hitInfo.transform.GetComponent<ObjectTypeStats>())
                    {
                        ObjectTypeStats scaledOTS = hitInfo.transform.GetComponent<ObjectTypeStats>();

                        // If the object can be charged...
                        if (scaledOTS.scaled)
                        {
                            ChangeSize(scaledOTS, hitInfo.point);

                            if(scalingMode)
                            {
                                TestGrow.complete = true;
                            }
                            else
                            {
                                TestShrink.complete = true;
                            }
                        }
                    }
                }
            }
        }
    }

    public void ChangeSize(ObjectTypeStats otherOTS, Vector3 impactPoint, bool grow = true)
    {
        if (scalingMode && grow)
        {
            if (otherOTS.currentScaleIndex < 6)
            {
                otherOTS.currentScaleIndex++;
                otherOTS.transform.localScale = otherOTS.initScaleLevel * otherOTS.scaleLevels[otherOTS.currentScaleIndex];
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    ActivateLazer(blueLazer, impactPoint);
                }
            }
        }
        else
        {
            if (otherOTS.currentScaleIndex > 0)
            {
                otherOTS.currentScaleIndex--;
                otherOTS.transform.localScale = otherOTS.initScaleLevel * otherOTS.scaleLevels[otherOTS.currentScaleIndex];
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    ActivateLazer(redLazer, impactPoint);
                }
            }
        }
        Invoke("DeactivateLazers", 0.1f);
    }

    private void HandleModeSelection()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            scalingMode = !scalingMode;

            anim.SetBool("bigSmallMode", scalingMode);
        }
    }

    public void ActivateLazer(GameObject laser, Vector3 impactPoint)
    {
        laser.SetActive(true);
        laserEndpoint = laser.transform.GetChild(6);
        laserEndpoint.parent = null;
        laserEndpoint.position = impactPoint;
    }

    public void DeactivateLazers()
    {
        if(laserEndpoint == null)
        {
            return;
        }

        if(scalingMode)
        {
            laserEndpoint.parent = blueLazer.transform;
        }
        else
        {
            laserEndpoint.parent = redLazer.transform;
        }
        blueLazer.SetActive(false);
        redLazer.SetActive(false);
    }
}

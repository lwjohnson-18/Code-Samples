/*****************************************************************************
// File Name :         ScanShootModifier.cs
// Author :            Lucas Johnson
// Creation Date :     September 28, 2022
//
// Brief Description : A C# script that modifies fired projectiles to have the
                       attributes of the ScanShoot weapon in air.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanShootModifier : BaseGunController
{
    [Header("Modifier Variables:")]
    public GameObject selectedProjectile;
    [Tooltip("Distance to raycast from camera")]
    [SerializeField][Range(1f, 3f)] public float hitRange = 3f;

    public bool scanMode = true;

    private Transform playerCamera = null;
    private int playerMask = ~(1 << 6);

    private Transform hologramParent;
    public GameObject scan;
    private ObjectiveController oc;
    private bool objectivesCollected = false;

    private ObjectTypeStats scannedOTS;

    public Material hologramMaterial;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        hologramParent = transform.GetChild(1);
        oc = GameObject.FindGameObjectWithTag("ObjectiveController").GetComponent<ObjectiveController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Continue only if the gun is being held
        if (DetectBeingHeld())
        {
            if (!objectivesCollected)
            {
                oc.CollectObjectives(transform.GetChild(2).gameObject, "Scan-N-Shoot");
                transform.GetChild(2).GetComponent<DecorateOffice>().ShowMarkers();
                objectivesCollected = true;
            }

            HandleModeSelection();

            if (scanMode)
            {
                HandleScanning();
            }
            else
            {
                if (selectedProjectile != null)
                {
                    Shoot(selectedProjectile, false);
                    if (Input.GetKeyDown(KeyCode.Mouse0) && scannedOTS.causesComplaints)
                    {
                        scannedOTS.AddComplaint();
                    }
                }
            }
        }
    }

    private void HandleModeSelection()
    {
        if (Input.GetKeyDown(KeyCode.R) && !scanMode)
        {
            scanMode = !scanMode;

            if (scanMode)
            {
                scannedOTS = null;
                Destroy(hologramParent.GetChild(0).gameObject);
                if (selectedProjectile)
                {
                    Destroy(selectedProjectile);
                    selectedProjectile = null;
                }
            }
        }
    }

    void HandleScanning()
    {
        // Draw a line in the inspector to show scan range
        Debug.DrawRay(playerCamera.position, playerCamera.forward * hitRange, Color.yellow);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, hitRange, playerMask))
            {
                scan.SetActive(true);
                Invoke("ScanOff", 0.5f);
                if (hitInfo.transform.GetComponent<ObjectTypeStats>())
                {
                    scannedOTS = hitInfo.transform.GetComponent<ObjectTypeStats>();

                    if (scannedOTS.scanned)
                    {
                        selectedProjectile = Instantiate(scannedOTS.transform.gameObject, transform);
                        GameObject hologram = Instantiate(scannedOTS.transform.gameObject, hologramParent);
                        FormatHologram(hologram);
                        selectedProjectile.SetActive(false);
                        scanMode = false;
                    }
                }
            }
        }
    }

    void FormatHologram(GameObject hologram)
    {
        if (hologram.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
        if (hologram.TryGetComponent(out Collider collider))
        {
            collider.enabled = false;
        }

        hologram.transform.localPosition = Vector3.zero;
        hologram.transform.rotation = Quaternion.identity;
        hologram.transform.localScale = (Vector3.one * 0.25f) * scannedOTS.scaleSize;
        hologram.layer = 8;
        hologram.GetComponent<ObjectTypeStats>().DisableElectronics(false);


        MeshRenderer[] meshRenderers = new MeshRenderer[hologram.transform.childCount];

        for (int i = 0; i < hologram.transform.childCount; i++)
        {
            Transform currentChild = hologram.transform.GetChild(i);
            currentChild.gameObject.layer = 8;

            if (currentChild.name.Contains("Particle"))
            {
                Destroy(hologram.transform.GetChild(i).gameObject);
                continue;
            }

            meshRenderers[i] = currentChild.GetComponent<MeshRenderer>();
        }

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i] == null)
            {
                continue;
            }
            else
            {
                meshRenderers[i].material = new Material(hologramMaterial);
            }
        }
    }

    void ScanOff()
    {
        scan.SetActive(false);
    }
}

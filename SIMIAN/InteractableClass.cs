/*****************************************************************************
// File Name :         InteractableClass.cs
// Author :            Lucas Johnson
// Creation Date :     March 7, 2022
//
// Brief Description : A C# script that allows objects to be interacted with
                       and handles the interaction process.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableClass : MonoBehaviour
{
    [Tooltip("The max distance at which the player can interact with the object " +
        "(Shown by the white wireframe sphere around the object)")]
    public float interactDistance = 2f;

    [Tooltip("Check this box to show interaction text when the object is interacted with")]
    public bool showInteractionText = false;

    [Tooltip("Check this box to have the object be deleted when it is interacted with")]
    public bool deleteObjectAfterInteraction = false;

    public bool objectInDesk = false;
    public bool objectInSafe = false;

    private bool canPickUp = true;

    [Tooltip("Method to be called when the object is interacted with")]
    public UnityEvent OnInteract;

    private Transform player;
    private TextMeshProUGUI tmp;

    private GameObject fogBorder;

    // Variable only used on doors to see if its open
    private bool doorIsOpen;

    private void Awake()
    {
        fogBorder = GameObject.Find("TextFogBorder");
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (GameObject.Find("Interaction Text"))
        {
            tmp = GameObject.Find("Interaction Text").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            throw new System.Exception("Please add the 'Interaction Text' object to the scene's canvas");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(objectInDesk || objectInSafe)
        {
            canPickUp = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(objectInDesk && gameObject.transform.parent.gameObject.transform.parent.GetComponent<DeskClass>().deskIsOpen)
        {
            canPickUp = true;
        }

        if (objectInSafe && gameObject.transform.parent.GetComponent<SafeClass>().safeIsOpen)
        {
            canPickUp = true;
        }

        if (Vector3.Distance(player.position, transform.position) > interactDistance) return;

        if (canPickUp)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnInteract.Invoke();

                if (showInteractionText)
                {
                    if (tmp == null)
                    {
                        throw new System.Exception("No Pickup Text Object in Scene");
                    }
                    else
                    {
                        tmp.enabled = true;
                        fogBorder.GetComponent<Image>().enabled = true;
                    }
                }

                if (deleteObjectAfterInteraction)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
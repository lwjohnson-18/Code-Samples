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

    [Tooltip("Center of the the interact range sphere")]
    public Vector3 ineractAreaOffset = new Vector3();

    [Tooltip("What text pops up when the player is in range to interact")]
    public string interactPrompt = "Press E to ";

    [Tooltip("Check this box to have the object be deleted when it is interacted with")]
    public bool deleteObjectAfterInteraction = false;

    [Tooltip("Method to be called when the object is interacted with")]
    public UnityEvent OnInteract;

    private GameController gc;
    private Transform player;
    private TextMeshProUGUI tmp;
    private InteractionTextBehaviour itb;

    [HideInInspector]
    public bool active = false;

    private bool interacted = false;

    private void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (GameObject.Find("Interaction Text"))
        {
            itb = GameObject.Find("Interaction Text").GetComponent<InteractionTextBehaviour>();
            tmp = GameObject.Find("Interaction Text").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            throw new System.Exception("Please add the 'Interaction Text' object to the scene's canvas");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;

        if (Vector3.Distance(player.position, transform.position + ineractAreaOffset) > interactDistance)
        {
            tmp.enabled = false;
            return;
        }
        else
        {
            if (!itb.interactedTextVisable && tmp.text != interactPrompt)
            {
                tmp.text = interactPrompt;
            }
            tmp.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (transform.name != "Bed" && !GameController.isPaused)
            {
                if (gc.stamina > 0 || BookshelfClass.isReading || TVClass.TVIsOn || transform.name == "Diary")
                {
                    OnInteract.Invoke();

                    if (!interacted)
                    {
                        if(transform.name != "Diary")
                        {
                            gc.stamina--;
                            interacted = true;
                        }
                    }

                    if (deleteObjectAfterInteraction)
                    {
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    itb.GenerateInteractedText("Maybe I'll do this tomorrow");
                }
            }
            else if (!GameController.isPaused)
            {
                if(gc.stamina <= 0)
                {
                    OnInteract.Invoke();
                }
                else
                {
                    itb.GenerateInteractedText("I'm not very tired right now");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + ineractAreaOffset, interactDistance);
    }
}
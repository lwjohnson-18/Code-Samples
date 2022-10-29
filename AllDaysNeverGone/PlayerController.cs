/*****************************************************************************
// File Name :         PlayerController.cs
// Author :            Lucas Johnson
// Creation Date :     March 2, 2022
//
// Brief Description : A C# script that handles the overal control of the
                       player including movement and looking around.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Reference to the transform of the first person camera")]
    [SerializeField] Transform playerCamera = null;

    [Tooltip("Controls the speed at which the player looks around with the mouse")]
    [SerializeField] public float mouseSensitivity = 3.5f;
    [Tooltip("The speed at which the player is currently moving")]
    [SerializeField] public float currentWalkSpeed = 6f;
    [Tooltip("The fastest speed at which the player can move")]
    [SerializeField] public float maxWalkSpeed = 6f;
    [Tooltip("The speed at which the player moves towards the ground")]
    [SerializeField] float gravity = -13f;
    [Tooltip("The amount of smoothing when the player starts and stops moving")]
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [Tooltip("The amount of smoothing when the player starts and stops looking around with the mouse")]
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;

    [Tooltip("Whether or not the mouse is hidden and locked to the center of the screen")]
    [SerializeField] public bool lockCursor = true;

    [Tooltip("Whether or not the player can move or not")]
    [SerializeField] public bool canMove = true;




    CharacterController controller = null;
    float cameraPitch = 0f;
    float velocityY = 0f;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;


    private List<Transform> interactables = new List<Transform>();

    private InteractableClass closestObject;
    private float closestObjectDis;

    private GameObject hitX = null;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();

        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        InteractableClass[] interactablesArray = FindObjectsOfType<InteractableClass>();

        foreach (var item in interactablesArray)
        {
            interactables.Add(item.transform);
        }

        currentWalkSpeed = maxWalkSpeed * GameController.playerSpeedPercentage;
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove && !GameController.isPaused)
        {
            UpdateMouseLook();
            UpdateMovement();
        }
        
        FindClosestInteractable();

        HandleThoughtBubbles();
    }

    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement()
    {
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if(controller.isGrounded)
        {
            velocityY = 0f;
        }
        velocityY += gravity * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * currentWalkSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
    }

    void FindClosestInteractable()
    {
        closestObjectDis = Mathf.Infinity;

        foreach (var item in interactables)
        {
            float objectDis = Vector3.Distance(item.transform.position, transform.position);

            if (objectDis < closestObjectDis)
            {
                if (closestObject != null)
                {
                    closestObject.active = false;
                }
                closestObject = item.GetComponent<InteractableClass>();
                closestObjectDis = objectDis;
            }
        }

        closestObject.active = true;
    }

    void HandleThoughtBubbles()
    {
        if (GameObject.FindGameObjectWithTag("ThoughtBubble"))
        {
            GameObject[] bubbles = GameObject.FindGameObjectsWithTag("ThoughtBubble");
            int closestBubbleIndex = 0;

            if (bubbles.Length > 1)
            {
                float smallestDis = Mathf.Infinity;

                for(int i = 0; i < bubbles.Length; i++)
                {
                    float _bubbleDis = Vector3.Distance(transform.position, bubbles[i].transform.position + Vector3.down * 2f);
                    float _interactRange = bubbles[i].GetComponent<ThoughtBubbleBehaviour>().interactRange;
                    if (_bubbleDis <= _interactRange && _bubbleDis < smallestDis)
                    {
                        smallestDis = _bubbleDis;
                        closestBubbleIndex = i;
                    }
                }
            }

            float bubbleDis = Vector3.Distance(transform.position, bubbles[closestBubbleIndex].transform.position + Vector3.down * 2f);
            ThoughtBubbleBehaviour tbb = bubbles[closestBubbleIndex].GetComponent<ThoughtBubbleBehaviour>();
            float interactRange = tbb.interactRange;

            if (bubbleDis <= interactRange && Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, interactRange))
            {
                Debug.DrawRay(playerCamera.position, playerCamera.forward * interactRange, Color.blue);

                if (hitInfo.transform.gameObject.name.Contains("Red X"))
                {
                    hitX = hitInfo.transform.gameObject;
                }
                
                if (hitX && hitX.name.Contains("Red X"))
                {
                    hitX.GetComponent<Image>().color = Color.gray;

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        Destroy(hitX);
                        hitX = null;
                        tbb.numXSpawned--;
                    }
                }
                
            }
            else
            {
                if (hitX)
                {
                    hitX.GetComponent<Image>().color = Color.white;
                }
            }
        }
    }
}

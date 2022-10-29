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
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
    [Tooltip("Reference to the transform of the first person camera")]
    [SerializeField] Transform playerCamera = null;

    [Tooltip("Controls the speed at which the player looks around with the mouse")]
    [SerializeField] public float mouseSensitivity = 3.5f;
    [Tooltip("The speed at which the player moves")]
    [SerializeField] float walkSpeed = 6f;
    [Tooltip("The speed at which the player moves towards the ground")]
    [SerializeField] float gravity = -13f;
    [Tooltip("The amount of smoothing when the player starts and stops moving")]
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [Tooltip("The amount of smoothing when the player starts and stops looking around with the mouse")]
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;

    [Tooltip("The range at which the vision effect starts when approaching an enemy")]
    [SerializeField] float visionEffectRange = 10f;

    [Tooltip("Distance to raycast from camera")]
    [SerializeField] public float hitRange = 2f;

    [Tooltip("Whether or not the mouse is hidden and locked to the center of the screen")]
    [SerializeField] public bool lockCursor = true;

    

    CharacterController controller = null;
    float cameraPitch = 0f;
    float velocityY = 0f;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    GameController gc;
    PauseMenuBehaviour pmb;
    public Volume visEffect;
    RedBookClass rbc;

    bool bookPresent = false;
    public bool lookingAtBook = false;
    public bool frozen = false;

    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        controller = GetComponent<CharacterController>();

        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        mouseSensitivity = PlayerPrefs.GetFloat("sensitivityPref");

        pmb = GameObject.Find("Pause Menu").GetComponent<PauseMenuBehaviour>();

        visEffect = GameObject.Find("Mannequin Effects").GetComponent<Volume>();

        if(GameObject.FindGameObjectWithTag("RedBook"))
        {
            bookPresent = true;
            rbc = GameObject.FindGameObjectWithTag("RedBook").GetComponent<RedBookClass>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(pmb.isPaused == false)
        {
            UpdateMouseLook();
            UpdateMovement();
            
            if(gc.end == false)
            {
                FootSteps();
                VisionEffect();
            }

            if (bookPresent && Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, hitRange))
            {
                Debug.DrawRay(playerCamera.position, playerCamera.forward * hitRange, Color.yellow);
                if(hitInfo.transform.gameObject.tag.Equals("RedBook"))
                {
                    lookingAtBook = true;
                }
                else
                {
                    lookingAtBook = false;
                }
            }
        }
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
        if (frozen) return;

        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if(controller.isGrounded)
        {
            velocityY = 0f;
        }
        velocityY += gravity * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
    }

    void FootSteps()
    {
        if (controller.isGrounded && controller.velocity.magnitude > 2.5f && GetComponent<AudioSource>().isPlaying == false)
        {
            GetComponent<AudioSource>().volume = Random.Range(0.8f, 1);
            GetComponent<AudioSource>().pitch = Random.Range(0.8f, 1.1f);
            GetComponent<AudioSource>().Play();
        }
    }

    Transform GetClosestMannequin(List<GameObject> mannequins)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = gameObject.transform.position;
        foreach(GameObject potentialTarget in mannequins)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }
        return bestTarget;
    }

    void VisionEffect()
    {
        if(gc.mannequins != null)
        {
            var closestEnemy = GetClosestMannequin(gc.mannequins);
            float targetDistance = Vector3.Distance(closestEnemy.transform.position, transform.position);
            if (targetDistance < visionEffectRange)
            {
                float effectIntensity = (visionEffectRange - targetDistance) / visionEffectRange;

                visEffect.weight = effectIntensity;
            }
        }
    }
}

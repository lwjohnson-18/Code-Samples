/*****************************************************************************
// File Name :         PlayerController.cs
// Author :            Lucas Johnson
// Creation Date :     August 31, 2022
//
// Brief Description : A C# script that handles the overal control of the
                       player including movement and looking around.
*****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region///////////// Public Variables: ///////////////

    // References
    [Header("Refrences:")]
    [Tooltip("Reference to the transform of the first person camera")]
    [SerializeField] Transform playerCamera = null;


    // Movement
    [Header("Movement:")]
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

    // Cursor
    [Header("Cursor:")]
    [Tooltip("Whether or not the mouse is hidden and locked to the center of the screen")]
    [SerializeField] public bool lockCursor = true;

    // Raycast Detection
    [Header("Raycast Detection:")]
    [Tooltip("Distance to raycast from camera")]
    [SerializeField][Range(1f, 3f)] public float hitRange = 2f;

    // Throw Ability
    [Header("Throw Ability:")]
    [Tooltip("Force at which an object is thrown")]
    [SerializeField][Range(500f, 1500f)] public float throwForce = 1000f;
    
    //key stuff
    public bool frostburnerCard = false;
    public bool ScanShootCard = false;
    public bool TeslaCard = false;

    // Inventory stuff(im sorry)
    public GameObject inventroy;
    #endregion

    #region///////////// Private Variables: ///////////////

    // Refrences
    CharacterController controller = null;
    ObjectiveController oc;
    ComplaintController cc;
    KeyCardDispenseScript DispenserScript;
    KeyCardScript KeycardCheck;
    Transform holdPoint;
    Transform firePoint;

    // Movement
    float cameraPitch = 0f;
    float velocityY = 0f;
    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;
    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;


    // Raycast
    int playerMask = ~(1 << 6); // Hides the player object from the raycast
    bool pickupFrame = false;
    #endregion

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        KeycardCheck = GameObject.FindGameObjectWithTag("Keycard").GetComponent<KeyCardScript>();
        DispenserScript = GameObject.FindGameObjectWithTag("Dispenser").GetComponent<KeyCardDispenseScript>();
        oc = GameObject.FindGameObjectWithTag("ObjectiveController").GetComponent<ObjectiveController>();
        cc = GameObject.FindGameObjectWithTag("ComplaintController").GetComponent<ComplaintController>();
        // Get Refrences
        controller = GetComponent<CharacterController>();
        holdPoint = GameObject.Find("Holding Point").transform;
        firePoint = GameObject.Find("Fire Point").transform;

        // Lock cursor and hide it during gameplay
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //mouseSensitivity = PlayerPrefs.GetFloat("SensitivityPref");

    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        pickupFrame = false;
        // Call Movement updates
        UpdateMouseLook();
        UpdateMovement();

        // Call Ability Updates
        HandlePickup();
        HandleThrowing();
    }

    /// <summary>
    /// Handles looking around using the mouse position
    /// </summary>
    void UpdateMouseLook()
    {
        // Get Mouse Input, smooth, and adjust it to match chosen sensitivity
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        
        // Restrict camera from moving past straight up & down
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        // Update camera and player rotation based on input
        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    /// <summary>
    /// Handles moving the player around using the WASD keys
    /// </summary>
    void UpdateMovement()
    {
        // Get Movement input and smooth it
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        // Handle if the player is falling
        if(controller.isGrounded)
        {
            velocityY = 0f;
        }
        velocityY += gravity * Time.deltaTime;

        // Calculate velocity based on set movement speed and apply it to the player
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Handles the ability to pick up objects (Also interactivity)
    /// </summary>
    void HandlePickup()
    {
        // Draw a line in the inspector to show pickup range
        Debug.DrawRay(playerCamera.position, playerCamera.forward * hitRange, Color.yellow);

        InventoryController ic = inventroy.GetComponent<InventoryController>();

        // If input, check if the player is looking at an object that can be picked up
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hitInfo, hitRange, playerMask))
            {
                if (hitInfo.transform.tag == "CanPickup")
                {
                    if (ic.held != null)
                    {
                        //If true, freeze objects physics & move it to the player's "hand" (Hold Point)
                        hitInfo.transform.GetComponent<Rigidbody>().isKinematic = true;
                        hitInfo.transform.SetParent(holdPoint);
                        hitInfo.transform.localPosition = Vector3.zero;
                        hitInfo.transform.localRotation = Quaternion.identity;
                        ChangeLayer(hitInfo.transform.gameObject, 8);
                        hitInfo.transform.gameObject.SetActive(false);

                        

                        for (int i = 0; i < 5; i++)
                        {
                            if (ic.inventory[i] == null)
                            {
                                ic.inventory.RemoveAt(i);
                                ic.inventory.Insert(i, hitInfo.transform.gameObject);
                                ic.slotImages[i].GetComponent<Image>().sprite = hitInfo.transform.gameObject.GetComponent<Image>().sprite;
                                ic.slotImages[i].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                                break;
                            }
                        }
                    }
                    else
                    {
                        //If true, freeze objects physics & move it to the player's "hand" (Hold Point)
                        hitInfo.transform.GetComponent<Rigidbody>().isKinematic = true;
                        hitInfo.transform.SetParent(holdPoint);
                        hitInfo.transform.localPosition = Vector3.zero;
                        hitInfo.transform.localRotation = Quaternion.identity;
                        ChangeLayer(hitInfo.transform.gameObject, 8);

                        ic.inventory.RemoveAt(ic.num);
                        ic.inventory.Insert(ic.num, hitInfo.transform.gameObject);
                        ic.slotImages[ic.num].GetComponent<Image>().sprite = hitInfo.transform.gameObject.GetComponent<Image>().sprite;
                        ic.slotImages[ic.num].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                        ic.held = hitInfo.transform.gameObject;
                    }

                    pickupFrame = true;
                }
                else if(hitInfo.transform.tag == "Door")
                {
                    hitInfo.transform.parent.GetComponent<DoorScript>().OpenCloseDoor();
                }
                else if(hitInfo.transform.tag == "Keycard")
                {
                    hitInfo.transform.GetComponent<KeyCardScript>().cardCheck();
                }
                else if(hitInfo.transform.tag == "Dispenser")
                {
                    bool[] products = oc.CalculateWhichProductsTested();

                    if(products[1])
                    {
                        DispenserScript.Dispense(false);
                    }
                    else if(products[0])
                    {
                        DispenserScript.Dispense(true);
                    }
                }
                else if(hitInfo.transform.tag == "CheckOut")
                {
                    EndOfDayController.instance.numObjectivesCompleted = oc.CalculateTotalObjectivesAchieved();
                    EndOfDayController.instance.numProductsTested = oc.CalculateTotalProductsTested();
                    EndOfDayController.instance.productsTested = oc.CalculateWhichProductsTested();
                    EndOfDayController.instance.hrComplaints = cc.numComplaints;
                    SceneManager.LoadScene("EndOfDayReportScene");
                }
                else if(hitInfo.transform.TryGetComponent(out ObjectTypeStats interactedOTS) && interactedOTS.disableOnZeroCharge)
                {
                    if(interactedOTS.transform.name.Contains("Lights"))
                    {
                        return;
                    }

                    if (interactedOTS.electronicsEnabled)
                    {
                        interactedOTS.DisableElectronics(false);
                    }
                    else
                    {
                        if (interactedOTS.chargeLevel > 0)
                        {
                            interactedOTS.EnableElectronics();
                        }
                    }
                }
            }
            if (ic.held && !pickupFrame)
            {
                if (ic.held.gameObject.TryGetComponent(out ObjectTypeStats heldOTS) && heldOTS.disableOnZeroCharge)
                {
                    if (heldOTS.electronicsEnabled)
                    {
                        heldOTS.DisableElectronics(false);
                    }
                    else
                    {
                        if (heldOTS.chargeLevel > 0)
                        {
                            heldOTS.EnableElectronics();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handles the ability to throw held objects
    /// </summary>
    void HandleThrowing()
    {

        InventoryController ic = inventroy.GetComponent<InventoryController>();

        // Only continue if the player holding something
        if (holdPoint.childCount == 0 || ic.held == null)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.Q) && !pickupFrame)
        {
            ReleaseObject(ic, false);
        }
        // If input, release held object and add a set force to it
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ReleaseObject(ic, true);
        }
    }

    private void ReleaseObject(InventoryController ic, bool throwObject)
    {
        GameObject thrownItem = ic.held.gameObject;
        thrownItem.transform.SetParent(null);
        thrownItem.transform.position = firePoint.position;

        Rigidbody rb = thrownItem.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        ChangeLayer(thrownItem, 0);

        if(throwObject)
        {
            rb.AddForce(playerCamera.forward * throwForce, ForceMode.Force);
        }
        else
        {
            thrownItem.transform.rotation = Quaternion.identity;
        }

        ic.inventory.RemoveAt(ic.num);
        ic.inventory.Insert(ic.num, null);
        ic.slotImages[ic.num].GetComponent<Image>().color = new Color(255, 255, 255, 0);
        ic.slotImages[ic.num].GetComponent<Image>().sprite = null;
        ic.held = null;
    }

    void ChangeLayer(GameObject objectToChange, int newLayer)
    {
        objectToChange.layer = newLayer;

        for (int i = 0; i < objectToChange.transform.childCount; i++)
        {
            Transform currentChild = objectToChange.transform.GetChild(i);
            currentChild.gameObject.layer = newLayer;

            if(currentChild.tag.Equals("TeslaDisplay"))
            {
                currentChild.GetChild(0).gameObject.layer = newLayer;
                currentChild.GetChild(1).gameObject.layer = newLayer;
            }
        }
    }
}

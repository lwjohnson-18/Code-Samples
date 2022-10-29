/*****************************************************************************
// File Name :         MannequinBehaviour.cs
// Author :            Lucas Johnson
// Creation Date :     March 2, 2022
//
// Brief Description : A C# script that handles the overal control of the
                       mannequin enemies. Including movement, agro detection,
                       and killing the player if they get too close.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MannequinBehaviour : MonoBehaviour
{
    [Tooltip("Refrence to the player's transform")]
    [SerializeField] Transform player;

    [Tooltip("Speed at which the mannequin moves")]
    [SerializeField] float moveSpeed = 1;

    [Tooltip("Radius at which the player dies")]
    [SerializeField] float killRange = 1;

    [SerializeField] bool agroActive = false;

    private float tempMoveSpeed;

    private PauseMenuBehaviour pmb;
    private MapController mc;

    Camera cam;

    Rigidbody rb;

    private void Awake()
    {
        cam = Camera.main;
        pmb = GameObject.Find("Pause Menu").GetComponent<PauseMenuBehaviour>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tempMoveSpeed = moveSpeed;
        mc = GameObject.Find("MapController").GetComponent<MapController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pmb.isPaused || mc.mapIsOpened)
        {
            moveSpeed = 0;
        }
        else
        {
            moveSpeed = tempMoveSpeed;
            FootSteps();
        }

        if (!agroActive) return;

        if(TestVisability())
        {
            Rotation();
            Movement();
            ChangePose();
        }

        if (Vector3.Distance(transform.position, player.position) < killRange)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameObject.Find("Death Screen").GetComponent<DeathScreenBehaviour>().dead = true;
            Time.timeScale = 0;
            GameObject.Find("Death Screen").transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    bool TestVisability()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return !GeometryUtility.TestPlanesAABB(planes, transform.GetComponent<Collider>().bounds);
    }

    void Rotation()
    {
        transform.LookAt(player);

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    void Movement()
    {
        if(Vector3.Distance(transform.position, player.position) > killRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    public void SetAgroActive(bool input)
    {
        agroActive = input;
    }

    void FootSteps()
    {
        if (rb.velocity.magnitude > 0f && GetComponent<AudioSource>().isPlaying == false)
        {
            GetComponent<AudioSource>().volume = Random.Range(0.8f, 1);
            GetComponent<AudioSource>().pitch = Random.Range(0.7f, 1f);
            GetComponent<AudioSource>().Play();
        }
    }

    void ChangePose()
    {
        var pose1 = gameObject.transform.GetChild(0);
        var pose2 = gameObject.transform.GetChild(1);
        var pose3 = gameObject.transform.GetChild(2);

        if(pose1.gameObject.activeInHierarchy)
        {
            pose1.gameObject.SetActive(false);
            pose2.gameObject.SetActive(true);
        }
        else if (pose2.gameObject.activeInHierarchy)
        {
            pose2.gameObject.SetActive(false);
            pose3.gameObject.SetActive(true);
        }
        else if (pose3.gameObject.activeInHierarchy)
        {
            pose3.gameObject.SetActive(false);
            pose1.gameObject.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRange);
    }
}

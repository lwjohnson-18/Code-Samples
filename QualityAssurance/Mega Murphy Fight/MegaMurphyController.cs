/*****************************************************************************
// File Name :         MegaMurphyController.cs
// Author :            Lucas Johnson
// Creation Date :     October 20, 2022
//
// Brief Description : A C# script that controls Mega Murphy.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaMurphyController : MonoBehaviour
{
    public float turnSpeed = 1;
    
    private Transform player;
    private Transform pivotJoint;
    private Transform batteryParent;
    private Transform facesParent;
    private int currentFace = 1;
    private SpriteRenderer[] faces = new SpriteRenderer[4];

    private Animator anim;

    private Vector3 initialRotation;
    private Quaternion lastRotation;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pivotJoint = transform.GetChild(0).GetChild(5);
        initialRotation = pivotJoint.rotation.eulerAngles;

        batteryParent = transform.GetChild(2);
        facesParent = GameObject.FindGameObjectWithTag("MurphyFace").transform;

        anim = transform.GetChild(0).GetComponent<Animator>();

        Invoke("ModifyBatteries", 0.1f);

        for (int i = 0; i < 4; i++)
        {
            faces[i] = facesParent.GetChild(i).GetComponent<SpriteRenderer>();
            if (i != 0)
            {
                faces[i].enabled = false;
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Debug.DrawRay(pivotJoint.position, player.position - pivotJoint.position);
        Debug.DrawRay(pivotJoint.position, pivotJoint.forward);

        if (BossFightController.instance.fightOver)
        {
            return;
        }

        if(!BossFightController.instance.fightActive)
        {
            return;
        }

        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);

        if (clipInfo[0].clip.name == "MurphIdle0")
        {
            FollowPlayer();
        }
        else if(clipInfo[0].clip.name == "MegaMurphAttack" || clipInfo[0].clip.name == "LaserStart" || clipInfo[0].clip.name == "LaserEnd")
        {
            pivotJoint.rotation = lastRotation;
        }
        else if (clipInfo[0].clip.name == "LaserBeam")
        {
            Vector3 newRotation = new Vector3(lastRotation.eulerAngles.x, lastRotation.eulerAngles.y + turnSpeed, lastRotation.eulerAngles.z);

            pivotJoint.rotation = Quaternion.Euler(newRotation);

            lastRotation = Quaternion.Euler(newRotation);
        }
    }

    void ModifyBatteries()
    {
        batteryParent.GetChild(1).GetComponent<ObjectTypeStats>().FreezeObject();

        BigSmallModifier bs = GameObject.Find("BigSmallGun").GetComponent<BigSmallModifier>();
        ObjectTypeStats shrunkBateryOTS = batteryParent.GetChild(3).GetComponent<ObjectTypeStats>();

        bs.ChangeSize(shrunkBateryOTS, transform.position, false);
        bs.ChangeSize(shrunkBateryOTS, transform.position, false);
        bs.ChangeSize(shrunkBateryOTS, transform.position, false);
    }

    void FollowPlayer()
    {
        pivotJoint.LookAt(player);
        pivotJoint.rotation = Quaternion.Euler(initialRotation.x, pivotJoint.transform.rotation.eulerAngles.y + 90f, initialRotation.z);
        lastRotation = pivotJoint.rotation;
    }

    public void IncrementFace()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].enabled = false;
        }

        faces[currentFace].enabled = true;
        currentFace++;

        anim.SetTrigger("triggerAttack");
    }

    public void DeathAnim()
    {
        anim.SetBool("isDead", true);
    }
}

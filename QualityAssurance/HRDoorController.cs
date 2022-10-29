/*****************************************************************************
// File Name :         HRDoorController.cs
// Author :            Lucas Johnson
// Creation Date :     October 26, 2022
//
// Brief Description : A C# script that controls the HR door
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRDoorController : MonoBehaviour
{
    public GameObject fakeWall;

    private ObjectTypeStats ots;
    private bool called = false;

    // Start is called before the first frame update
    void Start()
    {
        ots = GetComponent<ObjectTypeStats>();
        Invoke("ShrinkDoor", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(called && ots.currentScaleIndex == 3)
        {
            Destroy(fakeWall);
            Destroy(ots);
            Destroy(GetComponent<BoxCollider>());
            Destroy(this);
        }
    }

    void ShrinkDoor()
    {
        BigSmallModifier bs = GameObject.Find("BigSmallGun").GetComponent<BigSmallModifier>();

        bs.ChangeSize(ots, transform.position, false);
        bs.ChangeSize(ots, transform.position, false);
        bs.ChangeSize(ots, transform.position, false);

        called = true;
    }
}

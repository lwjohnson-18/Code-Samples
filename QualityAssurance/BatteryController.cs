/*****************************************************************************
// File Name :         BatteryController.cs
// Author :            Lucas johnson
// Creation Date :     October 6, 2022
//
// Brief Description : A C# script controls the visuals of the battery object.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryController : MonoBehaviour
{
    private ObjectTypeStats ots;
    public GameObject[] lightBlocks = new GameObject[4];
    public GameObject drainLight;

    // Start is called before the first frame update
    void Start()
    {
        ots = GetComponent<ObjectTypeStats>();
        for (int i = 0; i < lightBlocks.Length; i++)
        {
            lightBlocks[i] = transform.GetChild(i + 2).gameObject;
            lightBlocks[i].SetActive(false);
        }
        for (int i = 0; i < (4 - ots.chargeLevel); i++)
        {

            lightBlocks[i].SetActive(true);

        }
    }

    public void UpdateVisuals(int chargeLevel)
    {
        for (int i = 0; i < lightBlocks.Length; i++)
        {
            lightBlocks[i].SetActive(false);
        }

        for (int i = 0; i < (4 - chargeLevel); i++)
        {
            lightBlocks[i].SetActive(true);
        }

        if(chargeLevel == 0)
        {
            TestTakeCharge.complete = true;

            if(gameObject.tag == "MMBattery")
            {
                drainLight.SetActive(true);
            }
        }
        else if(chargeLevel == 4)
        {
            TestGiveCharge.complete = true;
        }
    }
}

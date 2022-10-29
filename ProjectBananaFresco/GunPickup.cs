/*****************************************************************************
// File Name :         GunPickup.cs
// Author :            Lucas Johnson
// Creation Date :     January 27, 2022
//
// Brief Description : A C# script that handles the gun pickup item that 
                       enables the players gun, opens the door, and destroys
                       itself.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.transform.GetChild(0).gameObject.GetComponent<Shooting>().enabled = true;
        collision.gameObject.transform.GetChild(0)
            .gameObject.transform.GetChild(0)
            .gameObject.transform.GetChild(0)
            .gameObject.SetActive(true);

        GameObject.Find("Door").SetActive(false);
        Destroy(gameObject);
    }
}

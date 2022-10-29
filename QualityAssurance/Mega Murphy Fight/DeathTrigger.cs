/*****************************************************************************
// File Name :         DeathTrigger.cs
// Author :            Lucas Johnson
// Creation Date :     October 19, 2022
//
// Brief Description : A C# script that sends the player to the respawn point
                       if they enter the trigger.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    public float killRadius = 5f;

    private Transform respawnPoint;
    private Transform player;

    private void Start()
    {
        respawnPoint = GameObject.FindGameObjectWithTag("RespawnPoint").transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, player.position) < killRadius)
        {
            player.position = respawnPoint.position;
            BossFightController.instance.fightActive = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRadius);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightController : MonoBehaviour
{
    public static BossFightController instance;
    public bool fightActive = false;
    public bool fightOver = false;
    public float startRadius = 3f;

    public int murphyHP = 4;

    private Transform player;
    private MegaMurphyController mmc;
    private ComplaintController cc;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mmc = GameObject.FindGameObjectWithTag("MegaMurphyController").GetComponent<MegaMurphyController>();
        cc = GameObject.FindGameObjectWithTag("ComplaintController").GetComponent<ComplaintController>();
    }

    private void Update()
    {
        if (!fightOver && Vector3.Distance(transform.position, player.position) < startRadius)
        {
            fightActive = true;
        }
        else
        {
            fightActive = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, startRadius);
    }

    public void Attacked()
    {
        print("Successful Attack");
        murphyHP--;
        

        if(murphyHP <= 0)
        {
            mmc.DeathAnim();
            print("Murphy Killed");
            fightActive = false;
            fightOver = true;
            cc.StopHRComplaints();
        }
        else
        {
            mmc.IncrementFace();
        }
    }
}

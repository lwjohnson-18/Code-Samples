/*****************************************************************************
// File Name :         RedBookClass.cs
// Author :            Lucas Johnson
// Creation Date :     March 27, 2022
//
// Brief Description : A C# script that the controls of the red book object.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBookClass : MonoBehaviour
{
    public bool interacted = false;
    public float bookshelfMoveSpeed = 0.5f;

    private Vector3 bookshelfEndPos;

    private PlayerController pc;

    private GameController gc;

    public GameObject monkey;

    // Start is called before the first frame update
    void Start()
    {
        bookshelfEndPos = transform.parent.transform.localPosition + Vector3.left * 2.25f;
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!interacted && pc.lookingAtBook && Input.GetKeyDown(KeyCode.E) && gc.haveCipher)
        {
            monkey.GetComponent<MannequinBehaviour>().SetAgroActive(true);
            interacted = true;
            gc.haveSafeCode = true;

        }
        
        if(transform.parent.transform.localPosition != bookshelfEndPos && interacted)
        {
            float step = bookshelfMoveSpeed * Time.deltaTime;
            transform.parent.transform.localPosition = Vector3.Lerp(transform.parent.transform.localPosition, bookshelfEndPos, step);
        }
    }
}

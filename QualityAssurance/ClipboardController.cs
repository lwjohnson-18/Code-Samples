/*****************************************************************************
// File Name :         ClipboardController.cs
// Author :            Lucas Johnson
// Creation Date :     September 4, 2022
//
// Brief Description : A C# script that controls the clipboard.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipboardController : MonoBehaviour
{   
    private GameObject clipVisuals;
    private static bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        clipVisuals = transform.GetChild(0).gameObject;

        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(active)
            {
                clipVisuals.SetActive(false);
                active = false;
            }
            else
            {
                clipVisuals.SetActive(true);
                active = true;
            }
        }
    }
}

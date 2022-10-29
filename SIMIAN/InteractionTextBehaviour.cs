/*****************************************************************************
// File Name :         InteractionTextBehaviour.cs
// Author :            Lucas Johnson
// Creation Date :     March 7, 2022
//
// Brief Description : A C# script that controls the Interaction Text object.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionTextBehaviour : MonoBehaviour
{
    [Tooltip("How long the pickup text remains on screen")]
    public float duration = 1f;

    private TextMeshProUGUI tmp;

    private GameObject fogBorder;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        fogBorder = GameObject.Find("TextFogBorder");
    }

    public void GenerateInteractionText(string interactionText)
    {
        tmp.text = interactionText;
        Invoke("DisableText", duration);
    }

    public void DisableText()
    {
        tmp.enabled = false;
        fogBorder.GetComponent<Image>().enabled = false;
    }
}

/*****************************************************************************
// File Name :         ComplaintController.cs
// Author :            Lucas Johnson
// Creation Date :     September 26, 2022
//
// Brief Description : A C# script that manages the HR Complaint System;
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplaintController : MonoBehaviour
{
    public GameObject meter;
    public HRBoardScript hrb;
    public Material screenMaterial;

    public int numComplaints = 0;

    public enum ComplaintLevel
    {
        Green,
        Yellow,
        Red
    }

    public ComplaintLevel currentLevel;

    public int yellowComplaints = 5;
    public int redComplaints = 10;

    public MeshRenderer[] spectrumRenderers;
    public GameObject[] meterFaces;
    private int FaceLevel = 1;

    private Material[] spectrumMaterials;

    private bool stopComplaints = false;

    // Start is called before the first frame update
    void Start()
    {
        numComplaints = 0;

        GetMeterArrays();
    }

    void GetMeterArrays()
    {
        Transform spectrumParent = meter.transform.GetChild(7);
        Transform facesParent = meter.transform.GetChild(8);

        spectrumRenderers = new MeshRenderer[spectrumParent.childCount];
        spectrumMaterials = new Material[spectrumParent.childCount];
        meterFaces = new GameObject[facesParent.childCount];

        for (int i = 0; i < spectrumParent.childCount; i++)
        {
            spectrumRenderers[i] = spectrumParent.GetChild(i).GetComponent<MeshRenderer>();
            spectrumMaterials[i] = spectrumRenderers[i].material;
            spectrumRenderers[i].material = screenMaterial;
        }

        for (int i = 0; i < facesParent.childCount; i++)
        {
            meterFaces[i] = facesParent.GetChild(i).gameObject;
        }
    }

    public void AddComplaint(int amountToAdd = 1)
    {
        if(stopComplaints)
        {
            return;
        }

        numComplaints += amountToAdd;

        if(amountToAdd <= 0)
        {
            spectrumRenderers[0].material = screenMaterial;
            return;
        }

        hrb.PostIt(numComplaints);

        if (numComplaints < yellowComplaints)
        {
            currentLevel = ComplaintLevel.Green;
        }
        else if(numComplaints < redComplaints)
        {
            currentLevel = ComplaintLevel.Yellow;
        }
        else
        {
            currentLevel = ComplaintLevel.Red;
        }

        if(amountToAdd >= 10)
        {
            FaceLevel = 5;
        }

        UpdateSpectrum();
        UpdateFace();
    }

    void UpdateSpectrum()
    {
        if (numComplaints <= spectrumRenderers.Length)
        {
            for (int i = 0; i < numComplaints; i++)
            {
                spectrumRenderers[i].material = spectrumMaterials[i];
            }
        }
    }

    void UpdateFace()
    {
        float facePercents = redComplaints / 6f;

        if(numComplaints > facePercents * FaceLevel && FaceLevel < 6)
        {
            for (int i = 0; i < meterFaces.Length; i++)
            {
                meterFaces[i].SetActive(false);
            }

            meterFaces[FaceLevel].SetActive(true);
            FaceLevel++;
        }
    }

    public void StopHRComplaints()
    {
        for (int i = 0; i < meterFaces.Length; i++)
        {
            meterFaces[i].SetActive(false);
        }

        meterFaces[6].SetActive(true);

        currentLevel = ComplaintLevel.Green;
        stopComplaints = true;
    }
}

/*****************************************************************************
// File Name :         ObjectTypeStats.cs
// Author :            Lucas johnson
// Creation Date :     September 11, 2022
//
// Brief Description : A C# script that saves what attributes an object has
                       and handles their behaviour when they are interacted 
                       with.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTypeStats : MonoBehaviour
{
    public bool causesComplaints = false;
    public int numComplaints = 1;

    [Header("Object can be:")]
    public bool burned = false;
    public bool frozen = false;
    public bool scanned = false;
    public bool charged = false;
    public bool scaled = false;
    public bool painted = false;

    [Space]

    [Header("FrostBurn:")]
    [Range(0, 2f)]
    public float particleScale = 1f;

    public Vector3 particleOffset = new Vector3();

    [HideInInspector]
    public bool isBurned = false;
    [HideInInspector]
    public bool isFrozen = false;

    private ComplaintController cc;
    private Transform objectTransform;
    private bool isNPC = false;
    private Vector3 highestPoint;
    private MeshRenderer[] meshRenderers;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private Material[] initMaterials;

    private Material burnedMaterial;
    private Material frozenMaterial;
    private GameObject burnedParticles;
    private GameObject frozenParticles;
    private bool effectMaterialsCollected = false;

    [Header("ScanShoot:")]
    public float scaleSize = 1f;

    [Header("Tesla:")]
    [Range(0, 4)] public int chargeLevel = 0;
    public bool disableOnZeroCharge = false;
    public bool burnOnFullCharge = false;
    public Material lightOnMaterial;
    public Material lightOffMaterial;
    public AudioSource radioMusicAudioSource;

    private bool initChargeVariable = false;
    [HideInInspector]
    public bool electronicsEnabled = true;

    [Header("BigSmall:")]
    public int currentScaleIndex = 3;
    public float[] scaleLevels = { 0.25f, 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f };
    [HideInInspector]
    public Vector3 initScaleLevel;

    [Header("Paintball:")]
    [HideInInspector]
    public bool isPainted;
    private PaintballModifier pbm;
    private Material paintedMaterial;

    public void Start()
    {
        cc = GameObject.FindGameObjectWithTag("ComplaintController").GetComponent<ComplaintController>();

        if(transform.tag == "NPC")
        {
            isNPC = true;
            objectTransform = transform.GetChild(0).GetChild(0).transform;
        }
        else
        {
            objectTransform = transform;
        }

        initChargeVariable = charged;

        if(chargeLevel == 0 && gameObject.name.Contains("Lights"))
        {
            GetComponent<MeshRenderer>().material = lightOffMaterial;
        }

        initScaleLevel = transform.localScale;

        pbm = GameObject.Find("PaintballGun").GetComponent<PaintballModifier>();

        if (burned && frozen)
        {
            GetEffectMaterials();
            GetObjectStats();
        }

        if(chargeLevel <= 0)
        {
            electronicsEnabled = false;
        }

        if(painted)
        {
            GetObjectStats();
        }
    }

    public void BurnObject()
    {
        if (!isBurned && burned)
        {
            ClearStatusEffects();
            GetEffectMaterials();

            if (isNPC)
            {
                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                {
                    skinnedMeshRenderers[i].material = new Material(burnedMaterial);
                }
            }
            else
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].material = new Material(burnedMaterial);
                }
            }

            GameObject particles = Instantiate(burnedParticles, transform.position, transform.rotation * burnedParticles.transform.rotation);
            particles.transform.SetParent(transform);
            particles.transform.localPosition += highestPoint + particleOffset;
            particles.transform.localScale *= particleScale;

            AddComplaint();

            isBurned = true;
            charged = false;
        }
    }

    public void FreezeObject()
    {
        if (!isFrozen && frozen)
        {
            ClearStatusEffects();
            GetEffectMaterials();

            if (isNPC)
            {
                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                {
                    skinnedMeshRenderers[i].material = new Material(frozenMaterial);
                }
            }
            else
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].material = new Material(frozenMaterial);
                }
            }

            GameObject particles = Instantiate(frozenParticles, transform.position, transform.rotation * frozenParticles.transform.rotation);
            particles.transform.SetParent(transform);
            particles.transform.localPosition += highestPoint + particleOffset;
            particles.transform.localScale *= particleScale;

            AddComplaint();

            isFrozen = true;
            charged = false;
        }
    }

    public void PaintObject()
    {
        if(!isPainted && painted)
        {
            paintedMaterial = pbm.paintMaterial;

            if (isNPC)
            {
                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                {
                    skinnedMeshRenderers[i].material = new Material(paintedMaterial);
                }
            }
            else
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].material = new Material(paintedMaterial);
                }
            }

            isPainted = true;
        }
    }

    public void ResetObject()
    {
        ClearStatusEffects();

        if(isNPC)
        {
            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                skinnedMeshRenderers[i].material = initMaterials[i];
            }
            return;
        }

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = initMaterials[i];
        }

        if(initChargeVariable)
        {
            charged = true;
        }
    }

    public void EnableElectronics()
    {
        if(electronicsEnabled)
        {
            return;
        }

        if (gameObject.name.Contains("Computer"))
        {
            //Turn off screen
        }
        else if (gameObject.name.Contains("Desk Lamp"))
        {
            transform.GetChild(11).GetComponent<MeshRenderer>().material = lightOnMaterial;
            transform.GetChild(15).gameObject.SetActive(true);
        }
        else if (gameObject.name.Contains("Radio"))
        {
            radioMusicAudioSource.enabled = true;
        }
        else if (gameObject.name.Contains("Lights"))
        {
            GetComponent<MeshRenderer>().material = lightOnMaterial;
            FixLight.complete = true;
        }

        electronicsEnabled = true;
    }

    public void DisableElectronics(bool addComplaint)
    {
        if(!electronicsEnabled)
        {
            return;
        }

        if(gameObject.name.Contains("Computer"))
        {
            //Turn off screen
        }
        else if(gameObject.name.Contains("Desk Lamp"))
        {
            transform.GetChild(11).GetComponent<MeshRenderer>().material = lightOffMaterial;
            transform.GetChild(15).gameObject.SetActive(false);
        }
        else if(gameObject.name.Contains("Radio"))
        {
            radioMusicAudioSource.enabled = false;
        }
        else if(gameObject.name.Contains("Lights"))
        {
            GetComponent<MeshRenderer>().material = lightOffMaterial;
        }
        if (addComplaint)
        {
            AddComplaint();
        }

        electronicsEnabled = false;
    }

    private void GetEffectMaterials()
    {
        if (!effectMaterialsCollected && GameObject.Find("FrostBurn Gun"))
        {
            FrostBurnModifier fbm = GameObject.Find("FrostBurn Gun").GetComponent<FrostBurnModifier>();
            burnedMaterial = fbm.burnedMaterial;
            frozenMaterial = fbm.frozenMaterial;

            burnedParticles = fbm.burnedParticles;
            frozenParticles = fbm.frozenParticles;
            effectMaterialsCollected = true;
        }
    }

    public void AddComplaint()
    {
        if (causesComplaints)
        {
            cc.AddComplaint(numComplaints);
        }
    }

    /// <summary>
    /// Get Objects Stats including it's MeshRenderer, materials and highest point
    /// </summary>
    private void GetObjectStats()
    {
        float highestYValue = Mathf.NegativeInfinity;
        
        if(isNPC)
        {
            skinnedMeshRenderers = new SkinnedMeshRenderer[objectTransform.childCount];
            initMaterials = new Material[objectTransform.childCount];


            for (int i = 0; i < objectTransform.childCount; i++)
            {
                skinnedMeshRenderers[i] = objectTransform.GetChild(i).GetComponent<SkinnedMeshRenderer>();
                initMaterials[i] = skinnedMeshRenderers[i].material;

                float meshHeight = skinnedMeshRenderers[i].bounds.max.y;
                if (meshHeight > highestYValue)
                {
                    highestYValue = meshHeight;
                }
            }

            highestPoint = new Vector3(0, highestYValue - transform.position.y, 0);
            return;
        }

        meshRenderers = new MeshRenderer[objectTransform.childCount];
        initMaterials = new Material[objectTransform.childCount];

        for (int i = 0; i < objectTransform.childCount; i++)
        {
            if (objectTransform.GetChild(i).name.Contains("Particle"))
            {
                continue;
            }

            meshRenderers[i] = objectTransform.GetChild(i).GetComponent<MeshRenderer>();
            initMaterials[i] = meshRenderers[i].material;
            
            float meshHeight = meshRenderers[i].bounds.max.y;
            if (meshHeight > highestYValue)
            {
                highestYValue = meshHeight;
            }
        }

        highestPoint = new Vector3(0, highestYValue - objectTransform.position.y, 0);
    }

    private void ClearStatusEffects()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).tag == "ParticleEffect")
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        isBurned = false;
        isFrozen = false;
    }
}

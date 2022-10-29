/*****************************************************************************
// File Name :         IncineratorController.cs
// Author :            Lucas Johnson
// Creation Date :     October 19, 2022
//
// Brief Description : A C# script controls the behavior of the incinerator 
                       object.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncineratorController : MonoBehaviour
{
    public float minRandomDelay = 5f;
    public float maxRandomDelay = 10f;

    public float smallFireDuration = 0.25f;
    public float bigFireDuration = 1f;

    private GameObject smallParticles;
    private GameObject bigFire;

    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        smallParticles = transform.GetChild(1).gameObject;
        bigFire = transform.GetChild(2).gameObject;

        smallParticles.SetActive(false);
        bigFire.SetActive(false);
    }

    private void Update()
    {
        if(active)
        {
            return;
        }

        if(BossFightController.instance.fightActive)
        {
            active = true;
            StopAllCoroutines();
            StartCoroutine(FireBurst());
        }
    }

    IEnumerator FireBurst()
    {
        while (active)
        {
            yield return new WaitForSeconds(Random.Range(minRandomDelay, maxRandomDelay));

            for (int i = 0; i < 3; i++)
            {
                smallParticles.SetActive(true);
                yield return new WaitForSeconds(smallFireDuration);
                smallParticles.SetActive(false);
                yield return new WaitForSeconds(smallFireDuration);
            }

            bigFire.SetActive(true);
            yield return new WaitForSeconds(bigFireDuration);
            bigFire.SetActive(false);


            if(!BossFightController.instance.fightActive)
            {
                active = false;
            }

            yield return 0;
        }
    }
}

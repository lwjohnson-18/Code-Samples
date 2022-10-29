/*****************************************************************************
// File Name :         PaintballBehaviour.cs
// Author :            Lucas Johnson
// Creation Date :     January 24, 2022
//
// Brief Description : A C# script that handles what the paintball should do
                       after it has been fired.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintballBehaviour : MonoBehaviour
{
    private ColorSelection cs;
    
    [HideInInspector]
    public Color ballColor;
    private ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
        cs = FindObjectOfType<ColorSelection>().GetComponent<ColorSelection>();
        InitializeColor();
    }

    private void InitializeColor()
    {
        ballColor = gameObject.GetComponent<SpriteRenderer>().color;

        if(cs.currentColor == cs.paintColors[0])
        {
            ballColor = Color.grey;
            ps.startColor = Color.grey;
            gameObject.name = "Grey Paintball";
        }
        else if (cs.currentColor == cs.paintColors[1])
        {
            ballColor = Color.red;
            ps.startColor = Color.red;
            gameObject.name = "Red Paintball";
        }
        else if (cs.currentColor == cs.paintColors[2])
        {
            ballColor = Color.blue;
            ps.startColor = Color.blue;
            gameObject.name = "Blue Paintball";
        }
        else if (cs.currentColor == cs.paintColors[3])
        {
            ballColor = Color.green;
            ps.startColor = Color.green;
            gameObject.name = "Green Paintball";
        }
        else if (cs.currentColor == cs.paintColors[4])
        {
            ballColor = Color.yellow;
            ps.startColor = Color.yellow;
            gameObject.name = "Yellow Paintball";
        }

        gameObject.GetComponent<SpriteRenderer>().color = ballColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}

/*****************************************************************************
// File Name :         ColorSelection.cs
// Author :            Lucas Johnson
// Creation Date :     January 24, 2022
//
// Brief Description : A C# script that handles what color on the paintball
                       gun is selected and what subsequent paintball should
                       be fired.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColorSelection : MonoBehaviour
{
    //this is so the cosmetic script can access it
    public int colorIndex;

    public List<Color> paintColors = new List<Color>() { Color.gray };

    public bool redLocked = false;
    public bool blueLocked = false;
    public bool greenLocked = false;
    public bool yellowLocked = false;

    public Color currentColor = Color.gray;

    private Scene scene;

    // Start is called before the first frame update
    void Start()
    {
        scene = SceneManager.GetActiveScene();
        AddColors();
    }

    private void AddColors()
    {
        if(!redLocked)
        {
            paintColors.Add(Color.red);
        }
        if(!blueLocked)
        {
            paintColors.Add(Color.blue);
        }
        if (!greenLocked)
        {
            paintColors.Add(Color.green);
        }
        if (!yellowLocked)
        {
            paintColors.Add(Color.yellow);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ColorInput();
        UpdateGunColor();
    }

    public void ColorInput()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentColor = paintColors[0];
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2) && !redLocked)
        {
            currentColor = paintColors[1];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && !blueLocked)
        {
            currentColor = paintColors[2];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && !greenLocked)
        {
            currentColor = paintColors[3];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && !yellowLocked)
        {
            currentColor = paintColors[4];
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") != 0f || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            int indexChange = 0;
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.E))
            {
                indexChange = 1;
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0 || Input.GetKeyDown(KeyCode.Q))
            {
                indexChange = -1;
            }

            int currentIndex = paintColors.IndexOf(currentColor);

            currentIndex += indexChange;

            if (currentIndex > paintColors.Count - 1)
            {
                currentIndex = 0;
            }
            else if (currentIndex < 0)
            {
                currentIndex = paintColors.Count - 1;
            }

            currentColor = paintColors[currentIndex];
            colorIndex = currentIndex;
        }
    }

    private void UpdateGunColor()
    {
        gameObject.GetComponent<SpriteRenderer>().color = currentColor;
    }
}

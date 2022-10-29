/*****************************************************************************
// File Name :         PauseMenuBehaviour.cs
// Author :            Lucas Johnson
// Creation Date :     March 23, 2022
//
// Brief Description : Controls the pause menu.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuBehaviour : MonoBehaviour
{
    public GameObject pauseMenuVisuals;
    public GameObject player;
    public Slider sensitivitySlider;

    public bool isPaused = false;

    private MapController mc;
    private DeathScreenBehaviour dsb;
    private static readonly string sensPref = "sensitivityPref";

    // Start is called before the first frame update
    void Start()
    {
        mc = GameObject.Find("MapController").GetComponent<MapController>();
        dsb = GameObject.Find("Death Screen").GetComponent<DeathScreenBehaviour>();
        sensitivitySlider.value = PlayerPrefs.GetFloat("sensitityPref");
    }

    // Update is called once per frame
    void Update()
    {
        if(!mc.mapIsOpened && Input.GetKeyDown(KeyCode.Escape) && !dsb.dead)
        {
            if(isPaused)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenuVisuals.SetActive(true);
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        pauseMenuVisuals.SetActive(false);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UpdateMouseSensitivity()
    {
        player.GetComponent<PlayerController>().mouseSensitivity = sensitivitySlider.value;

        PlayerPrefs.SetFloat(sensPref, sensitivitySlider.value);
    }

    public void LoadMainMenu()
    {
        isPaused = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("StartMenu");
    }
}

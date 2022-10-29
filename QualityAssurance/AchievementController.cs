/*****************************************************************************
// File Name :         AchievementController.cs
// Author :            Lucas Johnson
// Creation Date :     October 27, 2022
//
// Brief Description : A C# script that controls the achievement system.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementController : MonoBehaviour
{
    public static AchievementController instance;

    public GameObject achievementObjectPrefab;
    public List<Achievement> achievements = new List<Achievement>();

    private Transform achievementListParent;

    private bool spawned = false;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void LoadAchievements()
    {
        if(!spawned)
        {
            SpawnAchievements();
        }
        else
        {
            UpdateAchievements();
        }
    }

    void SpawnAchievements()
    {
        achievementListParent = GameObject.FindGameObjectWithTag("AchievementList").transform;

        foreach (Achievement achievement in achievements)
        {
            Transform spawnedAchievement = Instantiate(achievementObjectPrefab, achievementListParent).transform;
            spawnedAchievement.GetChild(0).GetComponent<Image>().sprite = achievement.achievementImage;
            
            if(!achievement.completed)
            {
                if (achievement.hideTitle)
                {
                    spawnedAchievement.GetChild(1).GetComponent<TextMeshProUGUI>().text = "???";
                }
                else
                {
                    spawnedAchievement.GetChild(1).GetComponent<TextMeshProUGUI>().text = achievement.title;
                    spawnedAchievement.GetChild(1).GetComponent<TextMeshProUGUI>().fontSize = achievement.titleFontSize;
                }

                spawnedAchievement.GetChild(2).GetComponent<TextMeshProUGUI>().text = achievement.hintText;
            }
            else
            {
                spawnedAchievement.GetComponent<Image>().color = new Color32(100, 100, 100, 100);
                spawnedAchievement.GetChild(0).GetComponent<Image>().color = Color.white;
                spawnedAchievement.GetChild(1).GetComponent<TextMeshProUGUI>().text = achievement.title;
                spawnedAchievement.GetChild(2).GetComponent<TextMeshProUGUI>().text = achievement.completedText;
            }

            spawnedAchievement.GetChild(2).GetComponent<TextMeshProUGUI>().fontSize = achievement.textFontSize;
        }

        spawned = true;
    }

    void UpdateAchievements()
    {
        for (int i = 0; i < achievements.Count; i++)
        {
            if(achievements[i].completed)
            {
                Transform achievementToUpdate = achievementListParent.GetChild(i).transform;
                achievementToUpdate.GetComponent<Image>().color = new Color32(100, 100, 100, 100);
                achievementToUpdate.GetChild(0).GetComponent<Image>().color = Color.white;
                achievementToUpdate.GetChild(1).GetComponent<TextMeshProUGUI>().text = achievements[i].title;
                achievementToUpdate.GetChild(1).GetComponent<TextMeshProUGUI>().fontSize = achievements[i].titleFontSize;
                achievementToUpdate.GetChild(2).GetComponent<TextMeshProUGUI>().text = achievements[i].completedText;
            }
        }
    }

}

[System.Serializable]
public class Achievement
{
    public Sprite achievementImage;
    public string title;
    public float titleFontSize = 20f;
    [TextArea]
    public string hintText;
    [TextArea]
    public string completedText;
    public float textFontSize = 14f;
    public bool hideTitle = false;
    public bool completed = false;
}

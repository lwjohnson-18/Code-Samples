/*****************************************************************************
// File Name :         ObjectiveController.cs
// Author :            Lucas johnson
// Creation Date :     September 13, 2022
//
// Brief Description : A C# script that objective mechanic.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveController : MonoBehaviour
{
    public static ObjectiveController instance;

    public GameObject EODController;

    [Tooltip("Reference to the clipboard objective prefab")]
    public GameObject clipboardObjectivePrefab;

    public List<ObjectiveList> objectiveTabs = new List<ObjectiveList>();
    public int pageNum = 0;
    public bool frostburncomplete = false;
    public bool scanshootcomplete = false;


    private TextMeshProUGUI titleText;
    private Transform clipboardObjectiveList;
    public GameObject[] currentDisplayedObjectiveTexts;

    private bool displayed = false;

    private GameObject completedStamp;

    private void Awake()
    {
        instance = this;

        if(!EndOfDayController.instance)
        {
            Instantiate(EODController);
        }
        EndOfDayController.instance.StartTimer();
    }

    // Start is called before the first frame update
    void Start()
    {
        titleText = GameObject.FindGameObjectWithTag("ClipboardTitle").GetComponent<TextMeshProUGUI>();

        // Get the clipboard list to be able to display objectives
        clipboardObjectiveList = GameObject.FindGameObjectWithTag("ObjectiveList").transform;

        completedStamp = transform.GetChild(0).GetChild(0).GetChild(2).gameObject;
        completedStamp.SetActive(false);

        transform.GetChild(0).gameObject.SetActive(false);
    }

    void Update()
    {
        if (objectiveTabs.Count > 0)
        {
            if (!displayed)
            {
                DisplayObjectivePage();
            }
        }


        if(Input.GetKeyDown(KeyCode.X) && pageNum > 0 && transform.GetChild(0).gameObject.activeSelf)
        {
            ChangeObjectivePage(-1);
        }
        else if(Input.GetKeyDown(KeyCode.V) && pageNum < objectiveTabs.Count - 1 && transform.GetChild(0).gameObject.activeSelf)
        {
            ChangeObjectivePage(1);
        }

        UpdateObjectiveCompletion();
    }

    /// <summary>
    /// Add a new set of objectives to the list of pages
    /// </summary>
    /// <param name="objectiveObject">Object that has the objective scripts to add</param>
    /// <param name="pageTitle">Title that will be displayed on the page</param>
    public void CollectObjectives(GameObject objectiveObject, string pageTitle)
    {
        ObjectiveList addedObjectiveList = new ObjectiveList();
        addedObjectiveList.objectives = objectiveObject.GetComponents<Objective>();
        addedObjectiveList.title = pageTitle;

        objectiveTabs.Add(addedObjectiveList);
    }

    private void DisplayObjectivePage()
    {
        // Get objective scripts
        Objective[] currentObjectiveTab = objectiveTabs[pageNum].objectives;
        currentDisplayedObjectiveTexts = new GameObject[currentObjectiveTab.Length];

        // For each objective script...
        for (int i = 0; i < currentDisplayedObjectiveTexts.Length; i++)
        {
            // Spawn an new objective item on the clipboard and add it to the current active objectives list
            currentDisplayedObjectiveTexts[i] = Instantiate(clipboardObjectivePrefab, clipboardObjectiveList);
            currentDisplayedObjectiveTexts[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentObjectiveTab[i].GUIText;
        }

        titleText.text = objectiveTabs[pageNum].title;
        
        completedStamp.SetActive(false);
        if(objectiveTabs[pageNum].completed)
        {
            completedStamp.SetActive(true);
        }

        displayed = true;
    }

    public void ChangeObjectivePage(int changeNum)
    {
        foreach (var objective in currentDisplayedObjectiveTexts)
        {
            Destroy(objective);
        }

        displayed = false;
        pageNum += changeNum;
    }

    private void UpdateObjectiveCompletion()
    {
        for (int i = 0; i < objectiveTabs.Count; i++)
        {
            int objectivesCompleted = 0;
            for (int j = 0; j < objectiveTabs[i].objectives.Length; j++)
            {
                if(objectiveTabs[i].objectives[j].IsAchieved())
                {
                    if (i == pageNum && currentDisplayedObjectiveTexts[j].transform.GetChild(0).GetChild(0))
                    {
                        currentDisplayedObjectiveTexts[j].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    }

                    objectivesCompleted++;
                    if (!objectiveTabs[i].completed && objectivesCompleted >= objectiveTabs[i].objectives.Length)
                    {
                        print("All " + objectiveTabs[i].title + " Objectives Complete");
                        objectiveTabs[i].completed = true;
                        completedStamp.SetActive(true);
                        if(objectiveTabs[i].completed && i == 0){
                            print("frostburner complete");
                            frostburncomplete = true;
                        }
                        if(objectiveTabs[i].completed && i == 1){
                            scanshootcomplete = true;
                        }
                    }

                    continue;
                }
                objectivesCompleted = 0;
            }
        }
    }

    public int CalculateTotalObjectivesAchieved()
    {
        int totalObjectivesAchieved = 0;
        for (int i = 0; i < objectiveTabs.Count; i++)
        {
            for (int j = 0; j < objectiveTabs[i].objectives.Length; j++)
            {
                if (objectiveTabs[i].objectives[j].IsAchieved())
                {
                    totalObjectivesAchieved++;
                }
            }
        }
        return totalObjectivesAchieved;
    }

    public int CalculateTotalProductsTested()
    {
        int totalProductsTested = 0;
        for (int i = 0; i < objectiveTabs.Count; i++)
        {
            if (objectiveTabs[i].completed)
            {
                totalProductsTested++;
            }
        }
        if(EndOfDayController.instance.productsTested[4])
        {
            totalProductsTested++;
        }

        return totalProductsTested;
    }

    public bool[] CalculateWhichProductsTested()
    {
        bool[] productsTested = new bool[] { false, false, false, false, false };
        for (int i = 0; i < objectiveTabs.Count; i++)
        {
            if (objectiveTabs[i].completed)
            {
                if(objectiveTabs[i].title == "FrostBurn")
                {
                    productsTested[0] = true;
                }
                else if (objectiveTabs[i].title == "Scan-N-Shoot")
                {
                    productsTested[1] = true;
                }
                else if (objectiveTabs[i].title == "Tesla Gun")
                {
                    productsTested[2] = true;
                }
                else if (objectiveTabs[i].title == "Big Small Gun")
                {
                    productsTested[3] = true;
                }
            }
        }
        productsTested[4] = EndOfDayController.instance.productsTested[4];

        return productsTested;
    }

    public bool returnFrostBurn(){
        return frostburncomplete;
    }
    public bool returnScanShoot(){
        return scanshootcomplete;
    }
}

[System.Serializable]
public class ObjectiveList
{
    public string title;
    public Objective[] objectives;
    public bool completed = false;
}

public abstract class Objective : MonoBehaviour
{
    public string GUIText;
    public abstract bool IsAchieved();
}
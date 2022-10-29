using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfDayController : MonoBehaviour
{
    public static EndOfDayController instance;

    public int numObjectivesCompleted = 0; // Out of 12
    public int numProductsTested = 0; // Out of 5
    public bool[] productsTested = new bool[5];
    public int hrComplaints = 0;
    public float shiftTime = 0;
    public string[] endings;

    public enum StatType
    {
        Objective,
        Product,
        Complaint,
        Ending,
        Grade
    }

    public enum Products
    {
        None,
        FrostBurn,
        ScanShoot,
        Tesla,
        BigSmall,
        Paintball
    }

    /// <summary>
    /// Text Refrences:
    /// 0 = ObjectivesCompleted
    /// 1 = ProductsTested
    /// 2 = HrComplaints
    /// 3 = ShiftTime
    /// 4 = Ending Text
    /// 5 = Grade Text
    /// </summary>
    private TextMeshProUGUI[] textRefrences;

    private Transform silhouetteParent;
    private GameObject[] silhouettes = new GameObject[5];

    private float startTime = 0;
    private string ending;
    private string grade;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void DisplayEODReport()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GetRefrences();
        ApplyEnding();
        CaluclateGrade();
        PopulateEODText();
    }

    private void GetRefrences()
    {
        textRefrences = new TextMeshProUGUI[6];
        GameObject textFieldParent = GameObject.FindGameObjectWithTag("EODRefrence");

        for (int i = 0; i < textFieldParent.transform.childCount; i++)
        {
            if (textFieldParent.transform.GetChild(i).TryGetComponent(out TextMeshProUGUI textComponent))
            {
                textRefrences[i] = textComponent;
            }
        }

        silhouetteParent = GameObject.FindGameObjectWithTag("SilhouetteParent").transform;

        for (int i = 0; i < silhouetteParent.childCount; i++)
        {
            silhouettes[i] = silhouetteParent.GetChild(i).gameObject;
        }
    }

    private string FormatShiftTime()
    {
        shiftTime = Time.unscaledTime - startTime;
        float minutes = Mathf.Floor(shiftTime / 60);
        float seconds = Mathf.RoundToInt(shiftTime % 60);

        string minutesString = minutes.ToString();
        string secondsString = seconds.ToString();

        if(minutes < 10)
        {
            minutesString = "0" + minutesString;
        }
        if(seconds < 10)
        {
            secondsString = "0" + secondsString;
        }

        return minutesString + ":" + secondsString;
    }

    private void ApplyEnding()
    {

        if(BossFightController.instance.fightOver)
        {
            ending = endings[4];
        }
        else if(numProductsTested < 3)
        {
            ending = endings[0];
        }
        else if(numProductsTested == 3)
        {
            ending = endings[1];
        }
        else if(numProductsTested == 4)
        {
            ending = endings[2];
        }
        else if (numProductsTested > 4)
        {
            ending = endings[3];
        }
    }

    private void CaluclateGrade()
    {
        float gainedTotal = numObjectivesCompleted + numProductsTested - (hrComplaints * 0.25f);
        float maxTotal = 17f;

        float gradePercentage = gainedTotal / maxTotal;

        if(gradePercentage > 0.9f)
        {
            grade = "A";
        }
        else if(gradePercentage > 0.8f)
        {
            grade = "B";
        }
        else if (gradePercentage > 0.7f)
        {
            grade = "C";
        }
        else if (gradePercentage > 0.6f)
        {
            grade = "B";
        }
        else
        {
            grade = "F";
        }
    }

    private void PopulateEODText()
    {
        textRefrences[0].text = numObjectivesCompleted + "/12";
        textRefrences[1].text = numProductsTested + "/5";
        textRefrences[2].text = hrComplaints.ToString();
        textRefrences[3].text = FormatShiftTime();
        textRefrences[4].text = ending;
        textRefrences[5].text = grade;

        for (int i = 0; i < silhouettes.Length; i++)
        {
            silhouettes[i].SetActive(!productsTested[i]);
        }
    }

    public void IncrementStat(StatType statType, Products product = Products.None)
    {
        if (statType == StatType.Objective)
        {
            numObjectivesCompleted++;
        }
        else if (statType == StatType.Product)
        {
            numProductsTested++;
            switch (product)
            {
                case Products.None:
                    break;
                case Products.FrostBurn:
                    productsTested[0] = true;
                    break;
                case Products.ScanShoot:
                    productsTested[1] = true;
                    break;
                case Products.Tesla:
                    productsTested[2] = true;
                    break;
                case Products.BigSmall:
                    productsTested[3] = true;
                    break;
                case Products.Paintball:
                    productsTested[4] = true;
                    break;
                default:
                    break;
            }
        }
        else if (statType == StatType.Complaint)
        {
            hrComplaints++;
        }
    }

    public void StartTimer()
    {
        startTime = Time.unscaledTime;
    }
}

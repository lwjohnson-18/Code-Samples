/*****************************************************************************
// File Name :         ExitClass.cs
// Author :            Samuel Dwyer and Lucas Johnson
// Creation Date :     March 29, 2022
//
// Brief Description : Handles opening the exit doors and plays cutscene
*****************************************************************************/
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitClass : MonoBehaviour
{
    private GameObject player;
    private PlayerController pc;
    private GameController gc;

    private void Start()
    {
        player = GameObject.Find("Player");
        pc = player.GetComponent<PlayerController>();
        gc = GameObject.Find("GameController").GetComponent<GameController>();
    }

    public void OpenDoor()
    {
        var gc = GameObject.Find("GameController").GetComponent<GameController>();
        if (gc.haveFoyerKey)
        {
            var leftDoor = gameObject.transform.GetChild(0);
            var rightDoor = gameObject.transform.GetChild(1);

            var rotation1 = leftDoor.transform.rotation.eulerAngles;
            var rotation2 = rightDoor.transform.rotation.eulerAngles;

            rotation1.y += 90;
            rotation2.y -= 90;
            leftDoor.transform.rotation = Quaternion.Euler(rotation1);
            rightDoor.transform.rotation = Quaternion.Euler(rotation2);
            
            EndCutScene();
        }
        else
        {
            var it = GameObject.Find("Interaction Text").GetComponent<InteractionTextBehaviour>();
            it.GenerateInteractionText("The exit door needs a key!");
        }
    }

    private void EndCutScene()
    {
        var endObjs = GameObject.Find("Ending Objects");
        var fog = GameObject.Find("TextFogBorder");
        var text = GameObject.Find("Interaction Text");

        fog.SetActive(false);
        text.SetActive(false);

        player.GetComponent<PlayerController>().frozen = true;
        player.transform.GetChild(1).gameObject.SetActive(true);
        player.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        endObjs.transform.GetChild(0).gameObject.SetActive(true);
        gc.end = true;

        foreach(GameObject monkey in gc.mannequins)
        {
            Destroy(monkey);
        }
    }

    private void Update()
    {
        if(gc.end)
        {
            float step = 0.2f * Time.deltaTime;
            pc.visEffect.weight = Mathf.Lerp(pc.visEffect.weight, 1, step);
            if (pc.visEffect.weight >= 0.9f)
            {
                SceneManager.LoadScene("EndState");
            }
        }
    }
}

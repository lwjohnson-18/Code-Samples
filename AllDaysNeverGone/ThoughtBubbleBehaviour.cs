using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThoughtBubbleBehaviour : MonoBehaviour
{
    public GameObject redX;
    public float interactRange = 2f;

    private GameController gc;
    private Transform player;
    private GameObject spawnpointParent;
    [HideInInspector]
    public int numXSpawned = 0;
    private bool destroyable = false;
    [HideInInspector]
    public Vector3 prevPos;

    private float baseSpeed;
    [Range(0, 1)]
    public float speedReduction;
    private float reducedSpeed;

    public List<string> thoughts = new List<string>();
    private TMPro.TextMeshProUGUI text;

    private void Awake()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        spawnpointParent = GameObject.Find("Thought Spawnpoints");
        prevPos = gc.prevThoughtPos;
        SelectThoughtSpawnpoint();

        text = gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        text.text = thoughts[Random.Range(0, thoughts.Count)];

        baseSpeed = player.gameObject.GetComponent<PlayerController>().maxWalkSpeed * GameController.playerSpeedPercentage;
        reducedSpeed = baseSpeed * speedReduction;
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnX", 1f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.position + Vector3.up * 2, Vector3.up);
        SlowPlayerSpeed();

        if (destroyable && numXSpawned <= 0)
        {
            gc.SpawnIntrusiveThought(transform.position);
            player.gameObject.GetComponent<PlayerController>().currentWalkSpeed = baseSpeed;
            Destroy(gameObject);
        }
    }

    public void SelectThoughtSpawnpoint()
    {
        List<Transform> spawnpoints = new List<Transform>();

        for (int i = 0; i < spawnpointParent.transform.childCount; i++)
        {
            spawnpoints.Add(spawnpointParent.transform.GetChild(i).transform);
        }

        bool loop = true;
        while (loop)
        {
            Vector3 newPos = spawnpoints[Random.Range(0, spawnpoints.Count)].position;
            if (newPos != prevPos)
            {
                transform.position = newPos;
                loop = false;
            }
        }
    }

    public void SpawnX()
    {
        destroyable = true;

        if (numXSpawned < 10)
        {
            Vector2 spawnPos = Random.insideUnitCircle.normalized * 0.5f;
            GameObject spawnedX = Instantiate(redX, transform);
            spawnedX.transform.localPosition = spawnPos;
            numXSpawned++;
        }
    }

    public void SlowPlayerSpeed()
    {
        player.gameObject.GetComponent<PlayerController>().currentWalkSpeed = reducedSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * 2f, interactRange);
    }
}

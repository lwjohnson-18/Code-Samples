/*****************************************************************************
// File Name :         BossStateController.cs
// Author :            Lucas Johnson
// Creation Date :     September 14, 2022
//
// Brief Description : A C# script that manages the boss's AI states.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossStateController : MonoBehaviour
{
    public GameObject[] bodyParts;
    public Transform[] standingPose;
    public Transform[] sittingPose;

    public Transform sitPoint;
    public Transform gaurdPoint;
    public Transform[] patrolPoints;

    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;
    public float instantDetectRadius;

    public enum State
    {
        Sitting,
        Gaurd,
        Patrol,
        Chase,
        Burned,
        Frozen
    }

    public State state;

    public Transform waterCooler;

    private bool playerInView;

    private Transform[] initialStandingPose;
    private Transform player;

    private CapsuleCollider capsuleCollider;
    private NavMeshAgent agent;
    private Animator anim;

    private ComplaintController cc;
    private ObjectTypeStats ots;

    private int destPoint = 0;

    private bool slowed = false;

    private void Awake()
    {
        initialStandingPose = standingPose;
    }

    private void Start()
    {
        playerInView = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        capsuleCollider = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        anim = transform.GetChild(0).GetComponent<Animator>();

        cc = GameObject.FindGameObjectWithTag("ComplaintController").GetComponent<ComplaintController>();
        ots = GetComponent<ObjectTypeStats>();

        StartCoroutine("CallDetectPlayerInFOV", .2f);

        state = State.Sitting;

        NextState();
    }

    void NextState()
    {
        switch (state)
        {
            case State.Sitting:
                StartCoroutine(SittingState());
                break;
            case State.Gaurd:
                StartCoroutine(GaurdState());
                break;
            case State.Patrol:
                StartCoroutine(PatrolState());
                break;
            case State.Chase:
                StartCoroutine(ChaseState());
                break;
            case State.Burned:
                StartCoroutine(BurnedState());
                break;
            case State.Frozen:
                StartCoroutine(FrozenState());
                break;
        }
    }

    IEnumerator SittingState()
    {
        //capsuleCollider.enabled = false;
        agent.enabled = false;
        anim.enabled = false;

        transform.position = sitPoint.position;
        transform.rotation = sitPoint.rotation;
        UpdatePos(sittingPose);

        while(state == State.Sitting)
        {
            if(cc.currentLevel != ComplaintController.ComplaintLevel.Green)
            {
                state = State.Gaurd;
            }

            yield return 0;
        }

        NextState();
    }

    IEnumerator GaurdState()
    {
        capsuleCollider.enabled = true;
        agent.enabled = true;
        anim.enabled = true;

        UpdatePos(initialStandingPose);
        bodyParts[0].transform.rotation = Quaternion.Euler(0, -90, 90);

        while (state == State.Gaurd)
        {
            if (cc.currentLevel == ComplaintController.ComplaintLevel.Yellow)
            {
                agent.destination = gaurdPoint.position;

                if(anim.enabled && transform.position == agent.destination)
                {
                    anim.enabled = false;
                    UpdatePos(initialStandingPose);
                    bodyParts[0].transform.rotation = Quaternion.Euler(0, 90, 90);
                    transform.rotation = gaurdPoint.rotation;
                }
            }
            else
            {
                state = State.Patrol;
            }
            yield return 0;
        }
        NextState();
    }

    IEnumerator PatrolState()
    {
        anim.enabled = true;

        while (state == State.Patrol)
        {
            //OnUpdate
            if (!playerInView)
            {
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    GoToNextPoint();
                }
                if(ots.isFrozen && !slowed)
                {
                    state = State.Frozen;
                }
                else if (ots.isBurned)
                {
                    state = State.Burned;
                }

                if (cc.currentLevel == ComplaintController.ComplaintLevel.Green)
                {
                    state = State.Sitting;
                }
            }
            else
            {
                state = State.Chase;
            }
            yield return 0;
        }
        NextState();
    }

    IEnumerator ChaseState()
    {
        while (state == State.Chase)
        {
            //OnUpdate
            if (playerInView)
            {
                agent.SetDestination(player.position);
                
                if (ots.isFrozen && !slowed)
                {
                    state = State.Frozen;
                }
                else if (ots.isBurned)
                {
                    state = State.Burned;
                }
            }
            else
            {
                state = State.Patrol;
            }
            yield return 0;
        }
        NextState();
    }

    IEnumerator BurnedState()
    {
        while (state == State.Burned)
        {
            //OnUpdate
            if (ots.isBurned)
            {
                agent.SetDestination(waterCooler.position + Vector3.back);
                if(Vector3.Distance(transform.position, waterCooler.position + Vector3.back) < 1f)
                {
                    agent.isStopped = true;
                    yield return new WaitForSeconds(0.5f);
                    ots.isBurned = false;
                    ots.ResetObject();
                    agent.isStopped = false;
                }
            }
            else
            {
                state = State.Patrol;
            }
            yield return 0;
        }
        NextState();
    }

    IEnumerator FrozenState()
    {
        while(state == State.Frozen)
        {
            agent.speed /= 2;
            StartCoroutine(ClearFrozenEffect());
            slowed = true;

            state = State.Patrol;

            yield return 0;
        }
        NextState();
    }

    IEnumerator ClearFrozenEffect()
    {
        yield return new WaitForSecondsRealtime(3f);
        ots.ResetObject();
        agent.speed *= 2;
        ots.isFrozen = false;
        slowed = false;
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }

        agent.destination = patrolPoints[destPoint].position;

        destPoint = (destPoint + 1) % patrolPoints.Length;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void DetectPlayerInFOV()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > viewRadius)
        {
            return;
        }

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        if(Vector3.Angle(transform.forward, dirToPlayer) < viewAngle/2 || distanceToPlayer < instantDetectRadius)
        {
            Debug.DrawLine(transform.position, player.position, Color.yellow, 0.2f);

            if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer))
            {
                if(distanceToPlayer < 1f && cc.currentLevel == ComplaintController.ComplaintLevel.Red)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    SceneManager.LoadScene("LoseScene");
                }
                playerInView = true;
                return;
            }
        }
        playerInView = false;
    }

    IEnumerator CallDetectPlayerInFOV(float raycastRate)
    {
        while(true)
        {
            yield return new WaitForSeconds(raycastRate);
            DetectPlayerInFOV();
        }
    }

    private void UpdatePos(Transform[] poseArray)
    {
        for (int i = 0; i < bodyParts.Length; i++)
        {
            bodyParts[i].transform.localRotation = poseArray[i].localRotation;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private const string isMoving = "isMoving";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    [SerializeField]
    private Animator animator;

    public EnemyState DefaultState;
    private EnemyState _state;

    public Transform target;
    public float updateSpeed = 0.1f;
    public float IdleLocationRadius = 4f;
    public float IdleMovespeedMultiplier = 0.5f;

    public EnemyLineOfSightChecker lineOfSightChecker;
    public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
    public StateChangeEvent OnStateChange;
    public NavMeshTriangulation triangulation;

    private int WaypointIndex = 0;
    private Vector3[] waypoints = new Vector3[4];

    private AgentLinkMover linkMover;
    private NavMeshAgent agent;
    private Coroutine followCoroutine;

    public EnemyState State
    {
        get { return _state; }
        set
        {
            OnStateChange?.Invoke(_state, value);
            _state = value;
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        linkMover = GetComponent<AgentLinkMover>();

        linkMover.OnLinkStart += HandleLinkStart;
        linkMover.OnLinkEnd += HandleLinkEnd;

        lineOfSightChecker.onGainSight += HandleGainSight;
        lineOfSightChecker.onLoseSight += HandleLoseSight;

        OnStateChange += HandleStateChange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLineStrip(waypoints, true);
    }

    private void OnDisable()
    {
        _state = DefaultState;
    }

    private void HandleGainSight(Player player)
    {
        State = EnemyState.Chase;
    }

    private void HandleLoseSight(Player player)
    {
        State = DefaultState;
    }

    private void HandleLinkStart()
    {
        animator.SetTrigger(Jump);
    }

    private void HandleLinkEnd()
    {
        animator.SetTrigger(Landed);
    }

    private void HandleStateChange(EnemyState oldState, EnemyState newState)
    {
        if (oldState == newState)
            return;

        if (followCoroutine != null)
            StopCoroutine(followCoroutine);

        if (oldState == EnemyState.Idle)
        {
            agent.speed /= IdleMovespeedMultiplier;
        }

        switch (newState)
        {
            case EnemyState.Idle:
                followCoroutine = StartCoroutine(DoIdleMotion());
                break;
            case EnemyState.Patrol:
                followCoroutine = StartCoroutine(DoPatrolMotion());
                break;
            case EnemyState.Chase:
                followCoroutine = StartCoroutine(followTarget());
                break;
        }
    }
   
    public void StartChasing()
    {
        //followCoroutine = StartCoroutine(followTarget());
        State = EnemyState.Patrol;
    }

    public void Spawn()
    {
        for (int i = 0; i < waypoints.Length; i++) 
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(triangulation.vertices[Random.Range(0, triangulation.vertices.Length)], out hit, 2f, agent.areaMask))
            {
                waypoints[i] = hit.position;
            }
            else
            {
                Debug.LogError("Unable to find position for navmesh newr Triangulation vertex!");
            }
        }
        OnStateChange?.Invoke(EnemyState.Spawn, DefaultState);
    }

    void Update()
    {
        animator.SetBool(isMoving, agent.velocity.magnitude > 0.01f);
    }

    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds wait = new(updateSpeed);

        agent.speed *= IdleMovespeedMultiplier;

        while (true)
        {
            if (!agent.enabled || !agent.isOnNavMesh)
            {
                yield return wait;
            }
            else if(agent.remainingDistance <= agent.stoppingDistance)
            {
                Vector2 point = Random.insideUnitCircle * IdleLocationRadius;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(agent.transform.position + new Vector3(point.x,0,point.y), out hit, 2f, agent.areaMask))
                {
                    agent.SetDestination(hit.position);
                }
            }
            yield return wait;
        }
    }

    private IEnumerator DoPatrolMotion()
    {
        WaitForSeconds wait = new(updateSpeed);

        yield return new WaitUntil(() => agent.enabled && agent.isOnNavMesh);
        agent.SetDestination(waypoints[WaypointIndex]);

        while (true)
        {
            if (agent.isOnNavMesh && agent.enabled && agent.remainingDistance <= agent.stoppingDistance)
            {
                WaypointIndex++;

                if(WaypointIndex >= waypoints.Length) 
                    WaypointIndex = 0;
                agent.SetDestination(waypoints[WaypointIndex]);
            }
            yield return wait;
        }
    }

    private IEnumerator followTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(updateSpeed);

        while (enabled && agent.enabled)
        {
            agent.SetDestination(target.transform.position);
            yield return wait;
        }
    }
}

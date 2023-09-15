using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    public Transform target;
    public float updateSpeed = 0.1f;
    [SerializeField]
    private Animator animator;
    private AgentLinkMover linkMover;

    private NavMeshAgent agent;

    private const string isMoving = "isMoving";
    private const string Jump = "Jump";
    private const string Landed = "Landed";

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        linkMover = GetComponent<AgentLinkMover>();

        //linkMover.OnLinkStart += HandleLinkStart;
        //linkMover.OnLinkEnd += HandleLinkEnd;
    }

    private void HandleLinkStart()
    {
        animator.SetTrigger(Jump);
    }

    private void HandleLinkEnd()
    {
        animator.SetTrigger(Landed);
    }

   
    void Start()
    {
        StartCoroutine(followTarget());
    }

    void Update()
    {
        animator.SetBool(isMoving, agent.velocity.magnitude > 0.01f);
    }

    private IEnumerator followTarget()
    {
        WaitForSeconds wait = new WaitForSeconds(updateSpeed);

        while (enabled)
        {
            agent.SetDestination(target.transform.position);
            yield return wait;
        }
    }
}

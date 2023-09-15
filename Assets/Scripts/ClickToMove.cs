using UnityEngine;
using UnityEngine.AI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Animator animator;
    private AgentLinkMover linkMover;

    private const string isWalking = "isWalking";
    private const string Jump = "Jump";
    private const string Landed = "Landed";


    NavMeshAgent m_Agent;
    RaycastHit m_HitInfo = new RaycastHit();

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();

        linkMover = GetComponent<AgentLinkMover>();

        linkMover.OnLinkStart += HandleLinkStart;
        linkMover.OnLinkEnd += HandleLinkEnd;
    }

    private void HandleLinkStart()
    {
        animator.SetTrigger(Jump);
    }

    private void HandleLinkEnd()
    {
        animator.SetTrigger(Landed);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
                m_Agent.destination = m_HitInfo.point;
        }
        animator.SetBool(isWalking, m_Agent.velocity.magnitude > 0.01f);
    }
}
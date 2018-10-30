using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CapsuleCollider))]
public class Patrollers : MonoBehaviourExtended
{
    [SerializeField] private Transform pointOne;
    [SerializeField] private Transform pointTwo;
    [SerializeField] private Transform pointThree;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerLastSighting;
    [SerializeField] private float detectRange;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private LayerMask obstacles;
    [SerializeField] private bool playerInSight;
    [SerializeField] private float attackRange;

    private NavMeshAgent navAgent;
    private Transform target;

    private CapsuleCollider capsuleCollider;
    private float halfColliderSize;
    

    protected override void Start()
    {
        base.Start();

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.destination = pointOne.position;
        target = pointOne;
        navAgent.speed = patrolSpeed;
        capsuleCollider = GetComponent<CapsuleCollider>();

        halfColliderSize = capsuleCollider.height / 2;
    }

    private void Update()
    {
        if (IsRendering() == false)
            return;

        Vector3 linePos = transform.position;
        linePos.y += halfColliderSize;

        Debug.DrawRay(linePos, transform.forward * detectRange, Color.red);
 
        Patrol();
    }

    private void Patrol()
    {
        if(playerInSight == false)
        {
            if (((player.position - transform.position).sqrMagnitude < detectRange * detectRange) && (target != player))
            {
                Debug.Log("in range");

                Vector3 linePos = transform.position;
                Vector3 linePosTarget = player.position;
                linePos.y += halfColliderSize;
                linePosTarget.y += halfColliderSize;
                Vector3 directionToTarget = player.position - linePos;

                if (!Physics.Linecast(linePos, player.position, obstacles))
                {
                    Debug.Log("can see");
                    Debug.DrawLine(linePos, player.position);

                    float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
                    if (Mathf.Abs(angleToTarget) > 90)
                    {
                        Debug.Log("Target is in front Me");
                        playerInSight = true;
                        playerLastSighting = player.transform;
                    }
                }
            }

        }


        if(playerLastSighting != null)
        {
            target = playerLastSighting;
            navAgent.destination = target.position;
        }

        if (target == pointOne)
        {
            if ((pointOne.position - transform.position).sqrMagnitude < .5 * .5)
            {
                navAgent.destination = pointTwo.position;
                target = pointTwo;
            }
        }

        if (target == pointTwo)
        {
            if ((pointTwo.position - transform.position).sqrMagnitude < .5 * .5)
            {
                navAgent.destination = pointThree.position;
                target = pointThree;
            }
        }

        if (target == pointThree)
        {
            if ((pointThree.position - transform.position).sqrMagnitude < .5 * .5)
            {
                navAgent.destination = pointOne.position;
                target = pointOne;
            }
        }
        
    }

}

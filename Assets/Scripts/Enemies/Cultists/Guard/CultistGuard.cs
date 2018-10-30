using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CultistGuard : MoveableEntity
{
    [Header("Target Detection")]
    [SerializeField] private float detectionRadius;
    [SerializeField] private float detectionInterval;
    [SerializeField] private LayerMask detectionLayer;

    [Header("Movement Speeds")]
    [SerializeField] private float runSpeed;

    [Header("Attack Timing")]
    [SerializeField] private float attackTime;

    [Header("Alert Telegraphing")]
    [SerializeField] private GameObject alertObject;
    [SerializeField] private AudioClip alertClip;
    [SerializeField] private Transform alertPosition;
    [SerializeField] private float alertDisplayTime;

    private float detectionCount;

    private float alertDisplayCount;

    private GuardAnimation guardAnimation;
    private CultistMelee cultistMelee;
    private bool waitingForUnsheathed;
    private bool waitingForSheathed;
    private bool inRange;
    private bool canAttack;

    private float walkSpeed;
    private float attackCount;

    protected override void Start()
    {
        base.Start();

        if (alertPosition == null)
            alertPosition = transform;

        guardAnimation = GetComponentInChildren<GuardAnimation>();
        walkSpeed = agent.speed;

        canAttack = true;

        cultistMelee = GetComponentInChildren<CultistMelee>();
    }

    protected override void Update()
    {
        if (currentStateType == EntityState.Idle || currentStateType == EntityState.ReturnToOrigin)
            Detect();

        if (currentStateType == EntityState.Alert)
            SeekTarget();

        if(currentStateType == EntityState.MoveToTarget && inRange)
            CheckAttack();

        base.Update();
    }

    public override void UpdateDestination()
    {
        agent.SetDestination(detectedPoint);
    }

    public override void DestinationReached()
    {
        detectionCount = detectionInterval;
        guardAnimation.SwordDrawnIdle();
        Detect();

        if(inRange == false)
        {
            inRange = true;
            attackCount = 0;
        }
    }

    public override void AnimateRotation()
    {
        guardAnimation.Turn();
    }

    public override void AnimateWalk()
    {
        inRange = false;

        if (currentStateType == EntityState.Alert)
        {
            agent.speed = walkSpeed;
            guardAnimation.Walk();
        }
        else if (currentStateType == EntityState.MoveToTarget)
        {
            agent.speed = runSpeed;
            guardAnimation.Run();
        }
    }

    public void UnsheathedSword()
    {
        waitingForUnsheathed = false;
        ChangeEntityState(EntityState.Alert);
    }

    public void SeathedSword()
    {
        waitingForSheathed = false;

        if (returnsToOrigin)// && (originPoint - transform.position).sqrMagnitude > minDestinationDistance * minDestinationDistance)
            ChangeEntityState(EntityState.ReturnToOrigin);
        else
        {
            guardAnimation.Idle();
            ChangeEntityState(EntityState.Idle);
        }
    }

    public void FinishedAttack()
    {
        canAttack = true;
    }

    private void Detect()
    {
        if (detectionCount < detectionInterval)
        {
            detectionCount += Time.deltaTime;
            return;
        }

        detectionCount = 0f;

        Collider[] inRangeColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
        bool detected = false;

        for (int i = 0; i < inRangeColliders.Length; i++)
        {
            if (Physics.Linecast(EyePosition, inRangeColliders[i].transform.position, environmentLayer))
                continue;

            detectedPoint = inRangeColliders[i].transform.position;
            detected = true;
            break;
        }

        if (detected == false)
        {
            if (currentStateType == EntityState.Alert && waitingForSheathed == false)
            {
                waitingForSheathed = true;
                guardAnimation.Sheathe();
            }

            return;
        }

        if (currentStateType != EntityState.Alert && waitingForUnsheathed == false)
        {
            waitingForUnsheathed = true;
            guardAnimation.Unsheathe();
        }
    }

    private void CheckAttack()
    {
        if (canAttack == false)
            return;

        if(attackCount < attackTime)
        {
            attackCount += Time.deltaTime;
            return;
        }

        attackCount = 0f;
        canAttack = false;
        guardAnimation.Attack();
    }

    private void SeekTarget()
    {
        Collider[] inRangeColliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
        bool detected = false;

        for (int i = 0; i < inRangeColliders.Length; i++)
        {
            if (Physics.Linecast(EyePosition, inRangeColliders[i].transform.position, environmentLayer))
                continue;

            target = inRangeColliders[i].gameObject;
            detected = true;
            break;
        }

        if (detected)
        {
            ChangeEntityState(EntityState.MoveToTarget);
        }
        else if (agent.destination == transform.position && waitingForSheathed == false)
        {
            waitingForSheathed = true;
            guardAnimation.Sheathe();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (Gizmos.color != Color.red)
            Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void SpawnAlert()
    {
        if (alertObject)
            SimplePool.Spawn(alertObject, alertPosition.position, alertObject.transform.rotation);

        if (alertClip)
            AudioSource.PlayClipAtPoint(alertClip, transform.position);
    }
}

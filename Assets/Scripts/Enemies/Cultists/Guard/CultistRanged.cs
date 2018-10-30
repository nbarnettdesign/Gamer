using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CultistRanged : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField]
    private float minDestinationDistance;
    [SerializeField] private float destinationCheckTime;
    [SerializeField] private float newDestinationRadius;

    [Header("Rotation")]
    [SerializeField]
    private float minFacingAngle;
    [SerializeField] private float rotationSpeed;

    [Header("Movement")]
    [SerializeField]
    private float strafeSpeed;
    [Tooltip("The name of the nav mesh area that the cultist will stay on when attacking the player")]
    [SerializeField] private string attackingAreaName;

    [Header("Projectile")]
    [SerializeField] private Minion minionToSpawn;
    [SerializeField] private float spawnAnimationDelay;

    [Header("Summoning Portal")]
    [SerializeField] private float castDelayTime;
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private Transform portalParent;
    [SerializeField] private float portalOpenTime;
    [SerializeField] private AnimationCurve portalOpenCurve;
    [SerializeField] private float portalCloseTime;
    [SerializeField] private AnimationCurve portalCloseCurve;

    [Header("Targeting")]
    [SerializeField] private float maxTargetingDistance;
    [SerializeField] private float targetDistanceCheckTime;
    [SerializeField] private LayerMask canSeeTargetMask;
    [SerializeField] private float eyeHeight;
    [SerializeField] private float lostPlayerPortalCloseTime;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip chantingSound;
    [SerializeField] private AudioClip minionSpawnSound;
    [SerializeField] private AudioClip portalOpenSound;
    [SerializeField] private AudioClip portalClosedSound;

    private float destinationCheckCount;
    private bool atDestination;
    private bool facedTarget;

    private GameObject portal;
    private bool portalOpening;
    private IEnumerator summoningRoutine;
    private bool summoningStarted;
    private float originalPortalScale;

    private Transform target;
    private Transform projectileParent;
    private NavMeshAgent agent;

    private Animator animator;
    private bool fromSpawner;
    private float originalSpeed;
    private float originalAngularSpeed;

    private void OnEnable()
    {
        if (agent)
        {
            agent.speed = originalSpeed;
            agent.angularSpeed = originalAngularSpeed;
        }
    }

    public void Init(Transform target, Vector3 destination, Transform projectileParent, bool fromSpawner = false)
    {
        this.target = target;
        this.projectileParent = projectileParent;

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            originalSpeed = agent.speed;
            originalAngularSpeed = agent.angularSpeed;
        }

        if (atDestination)
            atDestination = false;

        if (facedTarget)
            facedTarget = false;

        agent.SetDestination(destination);
        summoningStarted = false;

        animator = GetComponentInChildren<Animator>();

        if (animator)
            animator.SetBool("Run", true);

        this.fromSpawner = fromSpawner;

        if (portalPrefab)
        {
            if (portalParent == null)
                portalParent = transform;

            if (portal == null)
            {
                portal = SimplePool.Spawn(portalPrefab, portalParent.position, portalPrefab.transform.rotation);
                portal.transform.SetParent(portalParent);
                originalPortalScale = portal.transform.localScale.x;
            }

            for (int i = 0; i < portal.transform.childCount; i++)
            {
                portal.transform.GetChild(i).localScale = Vector3.zero;
            }

            portal.transform.localScale = Vector3.zero;
            portal.SetActive(false);
        }
    }

    private void Update()
    {
        CheckDestination();
        CheckRotation();
        CheckSummon();
    }

    private void CheckDestination()
    {
        if (agent == null || atDestination)
            return;

        if (destinationCheckCount < destinationCheckTime)
        {
            destinationCheckCount += Time.deltaTime;
            return;
        }

        destinationCheckCount = 0f;

        if ((agent.destination - transform.position).sqrMagnitude <= minDestinationDistance * minDestinationDistance)
        {
            atDestination = true;

            agent.SetDestination(transform.position);
            animator.SetBool("Run", false);

            var rangedAreaMask = agent.areaMask & ~(1 << 0);
            agent.areaMask = rangedAreaMask;

            if (agent.angularSpeed != 0f)
                agent.angularSpeed = 0f;

            if (agent.speed != strafeSpeed)
                agent.speed = strafeSpeed;
        }
    }

    private void MoveToNewDestination(Vector3 newLocation)
    {
        atDestination = false;
        facedTarget = false;

        NavMeshHit hit;
        NavMesh.SamplePosition(newLocation, out hit, newDestinationRadius, NavMesh.GetAreaFromName(attackingAreaName));

        // Set it to our destination
        agent.SetDestination(hit.position);

        if (animator && agent)
            animator.SetBool(agent.speed == 0 ? "Strafe" : "Run", true);
    }

    private void CheckRotation()
    {
        if (agent == null || atDestination == false)
            return;

        Vector3 adjustedTargetPosition = target.position;
        adjustedTargetPosition.y = transform.position.y;

        // convert to direction
        adjustedTargetPosition = adjustedTargetPosition - transform.position;

        float angle = Vector3.Angle(adjustedTargetPosition, transform.forward);

        if (angle > minFacingAngle)
        {
            float step = rotationSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, adjustedTargetPosition, step, 0f);

            transform.rotation = Quaternion.LookRotation(newDirection);

            // Make sure we don't rotate on the x, to stop the dancing
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
        else if (facedTarget == false)
            facedTarget = true;
    }

    private void CheckSummon()
    {
        if (CanSeeTarget() == false)
        {
            if(summoningStarted && portalOpening && summoningRoutine != null)
            {
                summoningStarted = false;
                animator.SetBool("SummonStart", false);
                StopCoroutine(summoningRoutine);
                ClosePortal(lostPlayerPortalCloseTime);
                this.Invoke(TurnOffPortal, lostPlayerPortalCloseTime);
            }
                 
            return;
        }

        if (agent == null || facedTarget == false || summoningStarted)
            return;

        summoningRoutine = Summon();
        StartCoroutine(summoningRoutine);
    }

    private bool CanSeeTarget()
    {
        if (target == null)
            return false;

       RaycastHit hit;

       if(Physics.Linecast(new Vector3(transform.position.x, transform.position.y + eyeHeight, transform.position.z), 
           target.position, out hit, canSeeTargetMask))
        {
            if (hit.transform.tag == "Player" || hit.transform.tag == "PlayerHead")
                return true;

            return false;
        }

        return true;
    }

    private IEnumerator Summon()
    {
        summoningStarted = true;

        yield return new WaitForSeconds(castDelayTime);

        //Start animation
        if (animator)
        {
            animator.SetBool("Run", false);
            animator.SetBool("SummonStart", true);
        }

        if (chantingSound)
            AudioSource.PlayClipAtPoint(chantingSound, transform.position);

        yield return new WaitForSeconds(spawnAnimationDelay);

        portalOpening = true;

        if (portal)
        {
            portal.SetActive(true);

            if (portalOpenSound)
                AudioSource.PlayClipAtPoint(portalOpenSound, transform.position);

            for (int i = 0; i < portal.transform.childCount; i++)
            {
                StartCoroutine(LerpController.Instance.Scale(portal.transform.GetChild(i), Vector3.zero,
                    Vector3.one * originalPortalScale, portalOpenTime, portalOpenCurve));
            }

            StartCoroutine(LerpController.Instance.Scale(portal.transform, Vector3.zero,
                    Vector3.one * originalPortalScale, portalOpenTime, portalOpenCurve));
        }

        yield return new WaitForSeconds(portalOpenTime);

        portalOpening = false;

        GameObject m = SimplePool.Spawn(minionToSpawn.gameObject, portalParent.transform.position,
            transform.rotation);

        m.GetComponent<Minion>().GiveTarget(target);
        m.transform.SetParent(projectileParent);

        if (minionSpawnSound)
            AudioSource.PlayClipAtPoint(minionSpawnSound, transform.position);

        if (portal)
        {
            ClosePortal(portalCloseTime);
        }

        yield return new WaitForSeconds(portalCloseTime);

        //Return to idle
        if (animator)
            animator.SetBool("SummonStart", false);

        MoveToNewDestination(transform.position + (Random.insideUnitSphere * newDestinationRadius));
        summoningStarted = false;
    }

    private void ClosePortal(float time)
    {
        if (portalClosedSound)
            AudioSource.PlayClipAtPoint(portalClosedSound, transform.position);

        for (int i = 0; i < portal.transform.childCount; i++)
        {
            StartCoroutine(LerpController.Instance.Scale(portal.transform.GetChild(i), portal.transform.GetChild(i).localScale,
                Vector3.zero, time, portalCloseCurve));
        }

        StartCoroutine(LerpController.Instance.Scale(portal.transform, portal.transform.localScale,
                Vector3.zero, time, portalCloseCurve));
    }

    private void TurnOffPortal()
    {
        if (portal)
            portal.SetActive(false);
    }

    private void OnDisable()
    {
        if (agent == null)
            return;

        var fullMask = agent.areaMask | (1 << 0);
        agent.areaMask = fullMask;
    }
}

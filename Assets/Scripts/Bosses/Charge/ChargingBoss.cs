using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(StunImpactable))]
[RequireComponent(typeof(DamageableParent))]
public class ChargingBoss : MonoBehaviourExtended {
    [Header("Targeting")]
    [SerializeField] private string targetTag;
    [SerializeField] private float rangeCheckTime;
    [SerializeField] private float minTargetingRange;
    [SerializeField] private float maxTargetingRange;
    [SerializeField] private float minFacingAngle;
    [SerializeField] private float chargeCooldownTime;
    [SerializeField] private float sphereCastRadius;

    [Header("Target Reevaluation")]
    [SerializeField] private float overlapSphereSize;
    [SerializeField] private LayerMask targetLayer;

    [Header("Movement")]
    [SerializeField] private float minMoveSpeed;
    [SerializeField] private float maxMoveSpeed;
    [Tooltip("When charging the amount to increase move speed by per second")]
    [SerializeField] private float speedPerSecond;
    [SerializeField] private float overshotVelocity;

    [Header("Acceleration")]
    [SerializeField] private float minAccelerationSpeed;
    [SerializeField] private float maxAccelerationSpeed;
    [Tooltip("When charging the amount to increase acceleration by per second")]
    [SerializeField] private float accelerationPerSecond;
    [Tooltip("Amount to multiply acceleration per second by while breaking")]
    [SerializeField] private float breakingScalar;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed;

    [Header("Stun Handling")]
    [SerializeField] private float environmentDetectionRange;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float environmentStunTime;
    [Tooltip("Stun only if the navMesh agents velocity magnitude is above this when the agent finds an environment piece")]
    [Range(0f, 20f), SerializeField] private float environmentImpactMagnitude;
    [SerializeField] private SoundPlayOneshot environmentImpactSound;
    [SerializeField] private GameObject environmentImpactParticle;

    [Header("Environement Damage")]
    [SerializeField] private float pillarDamage;

    [Header("Prop Interaction")]
    [SerializeField] private LayerMask propLayer;
    [SerializeField] private float propImpactVelocityModifier;
    [SerializeField] private float propDamage;
    [SerializeField] private float propDetectionRange;
    [SerializeField] private float propDetectionRadius;
    [SerializeField] private bool preventVerticalChange;

    [Header("Player Damage")]
    [SerializeField] private float playerDamageDistance;
    [SerializeField] private float playerDamage;
    [SerializeField] private bool instaKillPlayer;

    [Header("Shield")]
    [SerializeField] private GameObject shieldObject;
    [SerializeField] private float shieldActivateSpeed;
    [SerializeField] private float shieldActivateTime;
    [SerializeField] private AnimationCurve activationCurve;
    [SerializeField] private float shieldDeactivateTime;
    [SerializeField] private AnimationCurve deactivateCurve;

    private NavMeshAgent agent;
    private Transform target;
    private Vector3 previousTargetPosition;
    private bool inRange;
    private float rangeCheckCount;

    private bool isCharging;
    private bool hasOvershot;

    private bool isStunned;
    private float currentStunLength;
    private float stunCount;

    private bool hasReset;

    private Animator animationController;

    protected override void Start () {
        base.Start();

        target = GameObject.FindWithTag(targetTag).transform;

        Player player = target.GetComponent<Player>();

        if (player.rigSteamVR != null && player.rigSteamVR.activeInHierarchy)
            target = target.GetComponentInChildren<BodyCollider>().transform;
        else
            target = target.GetComponentInChildren<PlayerController>().transform;

        previousTargetPosition = target.position;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        animationController = GetComponentInChildren<Animator>();

        CheckInRange();
        ResetAgent();
    }

    private void Update () {
        if (IsRendering() == false || Stunned())
            return;

        CheckInRange();

        if (inRange == false)
            return;

        CheckDamageRange();
        CheckPropRange();
        CheckEnvironmentRange();
        UpdateMovement();
    }

    public void Stun(float stunTime) {
        isStunned = true;
        StopAgent();
        ResetAgent();
        agent.isStopped = true;

        stunCount = 0f;
        currentStunLength = stunTime;

        animationController.SetBool("Stunned", true);
    }

    private void CheckInRange () {
        if (rangeCheckCount < rangeCheckTime) {
            rangeCheckCount += Time.deltaTime;
            return;
        }

        rangeCheckCount = 0f;
        inRange = TargetInRange();
    }

    private void CheckDamageRange() {
        if (target == null)
            return;

        Vector3 point = transform.position;
        point.y = target.position.y;

        RaycastHit hit;
        if (Physics.SphereCast(point, sphereCastRadius, transform.forward, out hit, playerDamageDistance, targetLayer)) {
            target.GetComponent<Affectable>().Damage(playerDamage, transform.position, agent.velocity, false, instaKillPlayer);
        }
    }

    private void CheckEnvironmentRange() {
        if (isCharging == false || agent.velocity.magnitude < environmentImpactMagnitude)
            return;

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, out hit, environmentDetectionRange, environmentLayer)) {
            if (hit.transform.GetComponent<Damageable>()) {
                DamageChildren dChild = hit.transform.GetComponentInParent<DamageChildren>();

                if (dChild != null)
                    dChild.ApplyDamage(pillarDamage, transform.position, agent.velocity);
            }

            if (environmentImpactSound)
                environmentImpactSound.Play();

            if (environmentImpactParticle)
                SimplePool.Spawn(environmentImpactParticle, transform.position, Quaternion.identity);

            Stun(environmentStunTime);
        }
    }

    private void CheckPropRange() {
        if (isCharging == false)
            return;

        RaycastHit[] hitColliders = Physics.SphereCastAll(transform.position, propDetectionRadius, 
            transform.forward, propDetectionRange, propLayer);

        foreach (RaycastHit hit in hitColliders) {
            if (hit.rigidbody == null)
                continue;

            Vector3 velocity = agent.velocity * propImpactVelocityModifier;

            if (preventVerticalChange)
                velocity.y = 0f;

            hit.rigidbody.AddExplosionForce(velocity.magnitude * propImpactVelocityModifier, transform.position, propDetectionRadius);

            if (agent.speed >= maxMoveSpeed) {
                Affectable e = hit.transform.GetComponent<Affectable>();

                if(e)
                    e.Damage(propDamage, transform.position, agent.velocity);
            }
        }
    }

    private void UpdateMovement() {
        if (agent == null || target == null)
            return;

        Vector3 targetDirection = agent.destination - transform.position;
        targetDirection.y = transform.position.y;

        if (isCharging == false) {
            if (hasOvershot)
                ResetAgent();

            if (FacingTarget(targetDirection, minFacingAngle)) {
                StartCharge();
            } else {
                float step = rotationSpeed * Time.deltaTime;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0f);

                transform.rotation = Quaternion.LookRotation(newDirection);

                if (target.position != previousTargetPosition)
                    ReevaluateTarget();

                // Make sure we don't rotate on the x, to stop the dancing
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }
        } else {
            float agentVelocity = agent.velocity.magnitude * 0.1f;

            animationController.SetFloat("MoveSpeed", agent.velocity.magnitude * 0.1f);

            if(agentVelocity >= shieldActivateSpeed && shieldObject != null && shieldObject.activeInHierarchy == false) {
                StartCoroutine(ToggleShield(1f, shieldActivateTime, activationCurve, true));
            }

            if (transform.position != agent.destination && hasOvershot == false) {
                Accelerate();

                if (Vector3.Dot((agent.destination - transform.position).normalized, transform.forward) < 0) {
                    agent.SetDestination(transform.position);
                    hasOvershot = true;
                }
            } else if (hasOvershot) {
                Decelerate();
                agent.SetDestination(transform.position);

                if (agent.velocity.sqrMagnitude <= overshotVelocity * overshotVelocity && hasReset == false) {
                    StopAgent();
                    hasReset = true;
                    this.Invoke(ResetAgent, chargeCooldownTime);
                }
            }
        }
    }

    private bool FacingTarget (Vector3 targetDirection, float facingAngle) {
        if (target == null)
            return false;

        float angle = Vector3.Angle(targetDirection, transform.forward);

        if (angle < facingAngle)
        {
            if (animationController.GetBool("Turn") == false)
            {
                animationController.SetBool("Turn", false);
            }

            return true;
        }
        else {
            // Double check with a raycast
            Vector3 rayPos = transform.position;
            rayPos.y = target.position.y;

            RaycastHit hit;
            if (Physics.SphereCast(rayPos, sphereCastRadius, transform.forward, out hit, Mathf.Infinity, targetLayer))
                return true;
        }
        
        if(animationController.GetBool("Turn") == false)
        {
            animationController.SetBool("Turn", true);
        }

        return false;
    }

    private bool Stunned() {
        if (isStunned) {
            stunCount += Time.deltaTime;

            if (agent.velocity != Vector3.zero)
                agent.velocity = Vector3.zero;

            if (stunCount < currentStunLength)
                return true;

            isStunned = false;
            agent.isStopped = false;
            isCharging = false;

            animationController.SetBool("Stunned", false);
            ResetAgent();
        }
        return false;
    }

    private void StartCharge () {
        if (agent == null)
            return;

        isCharging = true;
        animationController.SetBool("Charging", true);
        animationController.SetFloat("MoveSpeed", 0);
        animationController.SetBool("Turn", false);

        agent.SetDestination(target.position);
    }

    private void Accelerate () {
        if (agent.acceleration < maxAccelerationSpeed)
            agent.acceleration += accelerationPerSecond * Time.deltaTime;

        if (agent.speed <= maxMoveSpeed)
            agent.speed += speedPerSecond * Time.deltaTime;
    }

    private void Decelerate () {
        if (agent.speed > 0)
            agent.speed -= speedPerSecond * breakingScalar * Time.deltaTime;

        agent.velocity = Vector3.ClampMagnitude(agent.velocity, agent.speed);
    }

    private bool TargetInRange () {
        if (target == null)
            return false;

        return (target.position - transform.position).sqrMagnitude <= maxTargetingRange * maxTargetingRange;
    }

    private void StopAgent () {
        agent.SetDestination(transform.position);
        agent.velocity = Vector3.zero;
    }

    private void ResetAgent () {
        hasReset = false;
        agent.speed = minMoveSpeed;
        agent.acceleration = minAccelerationSpeed;
        isCharging = false;
        hasOvershot = false;

        if (shieldObject)
            StartCoroutine(ToggleShield(0f, shieldDeactivateTime, deactivateCurve, false));

        agent.SetDestination(target.position);
        animationController.SetBool("Charging", false);
    }

    private void ReevaluateTarget () {
        List<Collider> hitColliders = Physics.OverlapSphere(agent.destination, overlapSphereSize, targetLayer).ToList();

        if (hitColliders.Exists(h => h.gameObject == target.gameObject) == false) {
            agent.destination = transform.position;
            hasOvershot = true;
        }

        previousTargetPosition = target.position;
    }

    private IEnumerator ToggleShield(float endingScale, float toggleTime, AnimationCurve scaleCurve, bool activate) {
        if (activate)
            shieldObject.SetActive(true);

        Vector3 originalSize = Vector3.one * shieldObject.transform.localScale.x;
        Vector3 endSize = Vector3.one * endingScale;
        float t = 0f;

        while (t <= toggleTime) {
            t += Time.deltaTime;
            shieldObject.transform.localScale = Vector3.LerpUnclamped(originalSize, endSize, scaleCurve.Evaluate(t/toggleTime));

            yield return null;
        }

        if (activate == false)
            shieldObject.SetActive(false);
    }
}

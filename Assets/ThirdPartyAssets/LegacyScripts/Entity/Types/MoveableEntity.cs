using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveableEntity : Entity {
    [SerializeField] protected EntityState startingState;
	[SerializeField] protected float minDestinationDistance;
    [SerializeField] protected float minLookAngle;
    [SerializeField] protected float eyeHeight;
    [SerializeField] protected float rotationSpeed;
	[SerializeField] protected EntityWanderSettings wanderSettings;
	[SerializeField] protected EntityFleeSettings fleeSettings;
    [SerializeField] protected bool returnsToOrigin;
    [SerializeField] protected LayerMask environmentLayer;
    
    public NavMeshAgent Agent { get { return agent; } }
	public float MinDestinationDistance { get { return minDestinationDistance; } }
    public float MinLookAngle { get { return minLookAngle; } }
    public float RotationSpeed { get { return rotationSpeed; } }
	public EntityWanderSettings WanderSettings { get { return wanderSettings; } }
	public EntityFleeSettings FleeSettings { get { return fleeSettings; } }
    public Transform FleeingFrom { get { return fleeingFrom; } }
    public Vector3 OriginPoint { get { return originPoint; } }
    public Vector3 DetectedPoint { get { return detectedPoint; } }
    public GameObject Target { get { return target; } }
    public Vector3 EyePosition { get { return new Vector3(transform.position.x, transform.position.y + eyeHeight, transform.position.z); } }

    public LayerMask EnvironmentLayer { get { return environmentLayer; } }
    public bool IsFrozen { get { return isFrozen; } }

    protected bool isFrozen;
    protected float originalSpeed;
    protected NavMeshAgent agent;
	protected IEntityState previousState;
    protected IEntityState currentState;
    protected EntityState currentStateType;

    protected Vector3 detectedPoint;

    protected Transform fleeingFrom;
    protected Vector3 originPoint;

    protected GameObject target;

    protected override void Start () {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        currentStateType = startingState;
        ChangeEntityState(startingState);

        if(returnsToOrigin)
            originPoint = transform.position;
    }

    protected virtual void Update () {
        UpdateEntityState();
    }

    public virtual void UpdateDestination() {}
    public virtual void DestinationReached() {}
    public virtual void AnimateRotation() {}
    public virtual void AnimateWalk() {}

    public void AdjustSpeed (float speedAdjustment) {
        agent.speed += speedAdjustment;
    }

    public void AdjustSpeed (float speedAdjustment, float time) {
        AdjustSpeed(speedAdjustment);
        this.Invoke(ResetSpeed, time);
    }

	public void SetSpeed(float newSpeed) {
		agent.speed = newSpeed;
	}

    public void ResetSpeed () {
        agent.speed = originalSpeed;
    }

    public void Freeze (float freezeTime) {
        agent.speed = 0f;
        isFrozen = true;
        this.Invoke(Unfreeze, freezeTime);
    }

	public virtual void ToPreviousState() {
		currentState = previousState;
		currentState.OnStateEnter(this);
	}

    protected virtual void UpdateEntityState () {
        CheckForDanger();

        if(currentState != null)
		    currentState.OnStateUpdate(this);
	}

    protected void ChangeEntityState (EntityState newState) {
        if (currentState != null) {
            currentState.OnStateExit(this);
			previousState = currentState;
		}

        currentStateType = newState;
        currentState = EntityStateController.Instance.GetEntityState(newState);
        currentState.OnStateEnter(this);
    }

    protected virtual void CheckForDanger () {
        if (fleeSettings == null)
            return;

        Collider[] d = Physics.OverlapSphere(transform.position, fleeSettings.DangerDetectionRadius, fleeSettings.DangerousLayers);
        List<Transform> dangerous = new List<Transform>();

        for (int i = 0; i < d.Length; i++) {
            if (d[i].GetComponent<Valve.VR.InteractionSystem.FireSource>() != null) {
                dangerous.Add(d[i].transform);
            }
        }

        if (dangerous.Count <= 0)
            fleeingFrom = null;    
        else if (dangerous.Count == 1)
            fleeingFrom = dangerous[0].transform;
        else {
            float dist = (dangerous[0].position - transform.position).sqrMagnitude;
            Transform closestTransform = dangerous[0];

            for (int i = 1; i < dangerous.Count; i++) {
                float tempDist = (dangerous[i].position - transform.position).sqrMagnitude;

                if(tempDist < dist) {
                    dist = tempDist;
                    closestTransform = dangerous[i];
                }
            }

            fleeingFrom = closestTransform;
        }
        
        if(fleeingFrom && currentStateType != EntityState.Flee) {
            currentStateType = EntityState.Flee;
            ChangeEntityState(currentStateType);
        } else if(fleeingFrom == null && currentStateType != startingState) {
            currentStateType = startingState;
            ChangeEntityState(currentStateType);
        }
    }

    private void Unfreeze () {
        isFrozen = false;
        ResetSpeed();
    }
}

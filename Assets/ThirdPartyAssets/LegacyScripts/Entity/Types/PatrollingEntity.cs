using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrollingEntity : MoveableEntity {
	[Header("Patrolling Settings")]
	// Public because of editor script PatrollingEntityEditor.cs
	[Tooltip("If false the points under entity controller will be used")] public bool manualPatrolPoints;
	[SerializeField] private int patrolPointNumber;
	// Public because of editor script PatrollingEntityEditor.cs
	[SerializeField] public List<Vector3> patrolPoints;
	[Tooltip("Range to check for points on the navmesh"), SerializeField] private float checkDistance;
	[SerializeField] private bool showPath;

	public List<Vector3> PatrolPoints { get { return patrolPoints; } }
	public Vector3 CurrentPatrolPoint {
		get { return currentPatrolPoint; }
		set {
			if (agent != null)
				agent.SetDestination(value);
			currentPatrolPoint = value;
		}
	}

	protected Vector3 currentPatrolPoint;

	protected override void Start() {
		if (manualPatrolPoints && patrolPoints == null || manualPatrolPoints && patrolPoints.Count <= 0) {
			Debug.LogError(string.Format("{0} is set to have manual patrol points, but none were specified!", name));
			return;
		}

		if (manualPatrolPoints == false || patrolPoints == null || patrolPoints.Count <= 0) {
			patrolPoints = PatrolController.Instance.GetPatrolPoints(transform.position, patrolPointNumber);
		}
        base.Start();
	}

    public void GivePatrolPoints (List<Vector3> points) {
        patrolPoints = new List<Vector3>(points);
    }

    private void OnDrawGizmosSelected() {
		if (showPath) {
			Gizmos.color = Color.red;

			for (int i = 0; i < patrolPoints.Count; i++) {
				if (i == patrolPoints.Count - 1) {
					Gizmos.DrawLine(patrolPoints[i], patrolPoints[0]);
				} else
					Gizmos.DrawLine(patrolPoints[i], patrolPoints[i + 1]);
			}
		}

		if (manualPatrolPoints && patrolPoints != null) {
			Gizmos.color = Color.blue;

			for (int i = 0; i < patrolPoints.Count; i++) {
				Gizmos.DrawSphere(patrolPoints[i], 0.2f);
			}
		}
	}
}

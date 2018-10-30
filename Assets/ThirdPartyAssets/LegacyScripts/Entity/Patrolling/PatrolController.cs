using System.Collections.Generic;
using UnityEngine;

public class PatrolController : Singleton<PatrolController> {
	// This list is public because of PatrolControllerEditor.cs
	public List<Vector3> patrolPoints;

	[Header("Gizmos")]
	[SerializeField] private bool showGizmos;
	[SerializeField] private float gizmoSize;
	[SerializeField] private Color gizmoColour;

	public Vector3 GetRandomPatrolPoint() {
		return patrolPoints[Random.Range(0, patrolPoints.Count)];
	}

	public Vector3 GetRandomPatrolPoint(Vector3 position, float range) {
		List<Vector3> pointsInRange = patrolPoints.FindAll(p => (position - p).sqrMagnitude <= range * range);
		return pointsInRange[Random.Range(0, pointsInRange.Count)];
	}

	public List<Vector3> GetPatrolPoints(Vector3 position, int number) {
		List<Vector3> points = new List<Vector3> {
			GetClosestPatrolPoint(position)
		};

		for (int i = 1; i < number; i++) {
			points.Add(GetClosestPatrolPoint(points[i - 1]));
		}

		return points;
	}

	public Vector3 GetClosestPatrolPoint(Vector3 position) {
		float dist = Mathf.Infinity;
		Vector3 p = Vector3.zero;

		for (int i = 0; i < patrolPoints.Count; i++) {
			if (patrolPoints[i] == position)
				continue;

			float tempDistance = (position - patrolPoints[i]).sqrMagnitude;

			if(tempDistance < dist) {
				p = patrolPoints[i];
				dist = tempDistance;
			}
		}

		return p;
	}

	private void OnDrawGizmosSelected() {
		if (showGizmos == false)
			return;

		if (Gizmos.color != gizmoColour)
			Gizmos.color = gizmoColour;

		for (int i = 0; i < patrolPoints.Count; i++) {
			Gizmos.DrawSphere(patrolPoints[i], gizmoSize);
		}
	}
}

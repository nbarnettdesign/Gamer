using System.Collections.Generic;
using UnityEngine;

public class EntityPatrol : IEntityState {
	private List<Vector3> patrolPoints;

	public void OnStateEnter(MoveableEntity entity) {
		PatrollingEntity e = entity as PatrollingEntity;

		patrolPoints = e.PatrolPoints;

		e.CurrentPatrolPoint = patrolPoints[0];
		entity.Agent.SetDestination(e.CurrentPatrolPoint);
	}

	public void OnStateUpdate(MoveableEntity entity) {
		if ((entity.Agent.destination - entity.transform.position).sqrMagnitude <= entity.MinDestinationDistance * entity.MinDestinationDistance) {
			NextPatrolPoint(entity as PatrollingEntity);
		}
	}

	public void OnStateExit(MoveableEntity entity) { }

	private void NextPatrolPoint(PatrollingEntity entity) {
		for (int i = 0; i < patrolPoints.Count; i++) {
			if (entity.CurrentPatrolPoint == patrolPoints[i]) {
                if (i == patrolPoints.Count - 1) {
					entity.CurrentPatrolPoint = patrolPoints[0];
                    break;
                }
                else {
					entity.CurrentPatrolPoint = patrolPoints[i + 1];
                    break;
                }
			}
		}

        entity.Agent.SetDestination(entity.CurrentPatrolPoint);
	}
}

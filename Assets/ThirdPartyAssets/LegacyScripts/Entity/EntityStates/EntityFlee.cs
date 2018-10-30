using UnityEngine;
using UnityEngine.AI;

public class EntityFlee : IEntityState {
	private Transform fleeFromTransform;

    public void OnStateEnter (MoveableEntity entity) {
        entity.Agent.SetDestination(GetFleeFromPosition(entity));
		fleeFromTransform = entity.FleeingFrom;
		entity.SetSpeed(entity.FleeSettings.FleeSpeed);
    }

    public void OnStateUpdate (MoveableEntity entity) {
		if(entity.FleeingFrom != fleeFromTransform) {
			fleeFromTransform = entity.FleeingFrom;
			entity.Agent.SetDestination(GetFleeFromPosition(entity));
		}

        if ((entity.transform.position - entity.Agent.destination).sqrMagnitude <= entity.MinDestinationDistance * entity.MinDestinationDistance)
			entity.ToPreviousState();
    }

    public void OnStateExit (MoveableEntity entity) {
		entity.ResetSpeed();
    }

    private Vector3 GetFleeFromPosition (MoveableEntity entity) {
        Vector3 newDestination = (entity.FleeingFrom.position - entity.transform.position).normalized;
		newDestination = -newDestination * entity.FleeSettings.FleeDistance;

		NavMeshHit hit;
		NavMesh.SamplePosition(entity.transform.position + newDestination, out hit, entity.FleeSettings.DangerDetectionRadius / 2f, NavMesh.AllAreas);

		return hit.position == Vector3.positiveInfinity ? entity.transform.position + newDestination : hit.position;

	}
}

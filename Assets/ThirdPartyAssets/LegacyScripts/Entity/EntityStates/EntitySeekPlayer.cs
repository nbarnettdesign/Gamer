using UnityEngine;
using UnityEngine.AI;

public class EntitySeekPlayer : IEntityState {
	private Transform target;
	private Vector3 targetLocation;
	private Vector3 previousPosition;
	private EntityController entityController;

	private bool recheckTargetLocation;
	private float recheckTime;
	private float recheckCount;

	public void OnStateEnter(MoveableEntity entity) {
		entityController = EntityController.Instance;
		recheckTime = EntitySeekerController.Instance.TargetRecheckTime;

		UpdateTarget(entity);

		previousPosition = targetLocation;
	} 

	public void OnStateUpdate(MoveableEntity entity) {
		// If we can't get to the target just go to the teleport point closest to us
		if (entity.Agent.pathPending == false && entity.Agent.pathStatus != NavMeshPathStatus.PathComplete)
			targetLocation = entityController.SpawnController.GetClosestTeleportPoint(entity.transform.position);

		if (recheckTargetLocation)
			RecheckTarget(entity);

		if ((entity.transform.position - targetLocation).sqrMagnitude <= entity.MinDestinationDistance * entity.MinDestinationDistance) {
			targetLocation = entity.transform.position;
			recheckTargetLocation = true;
		}

		if(targetLocation != previousPosition) {
			entity.Agent.SetDestination(targetLocation);
			previousPosition = targetLocation;
		}
	}

	public void OnStateExit(MoveableEntity entity) {
		
	}

	private void RecheckTarget(MoveableEntity entity) {
		recheckCount += Time.deltaTime;

		if(recheckCount >= recheckTime) {
			recheckCount = 0f;
			recheckTargetLocation = false;
			UpdateTarget(entity);
		}
	}

	private void UpdateTarget(MoveableEntity entity) {
		target = entityController.SeekerTarget;
		targetLocation = entityController.GetSeekerTargetPosition();
		entity.Agent.SetDestination(target.position);
	}
}

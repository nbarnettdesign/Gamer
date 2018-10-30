using UnityEngine;
using UnityEngine.AI;

public class EntityMoveRandom : IEntityState {
    private float currentWalkTime;
    private float walkTimeCount;

    public void OnStateEnter (MoveableEntity entity) {
        SetWalkTime(entity.WanderSettings.MinWalkTime, entity.WanderSettings.MaxWalkTime);
        entity.Agent.SetDestination(GetNewDestination(entity));
    }

    public void OnStateUpdate (MoveableEntity entity) {
		if (entity.Agent.pathPending == false && entity.Agent.pathStatus != NavMeshPathStatus.PathComplete)
			GetNewDestination(entity);

        if((entity.transform.position - entity.Agent.destination).sqrMagnitude <= entity.MinDestinationDistance * entity.MinDestinationDistance) {
            SetWalkTime(entity.WanderSettings.MinWalkTime, entity.WanderSettings.MaxWalkTime);
            entity.Agent.SetDestination(GetNewDestination(entity));
            return;
        }

        walkTimeCount += Time.deltaTime;

        if (walkTimeCount >= currentWalkTime) {
            SetWalkTime(entity.WanderSettings.MinWalkTime, entity.WanderSettings.MaxWalkTime);
            entity.Agent.SetDestination(GetNewDestination(entity));
        }
    }

    public void OnStateExit (MoveableEntity entity) {}

    private void SetWalkTime (float minWalkTime, float maxWalkTime) {
        currentWalkTime = Random.Range(minWalkTime, maxWalkTime);
        walkTimeCount = 0f;
    }

    private Vector3 GetNewDestination (MoveableEntity entity) {
        Vector3 destination = NavMeshController.Instance.RandomNavSphere(entity.transform.position, entity.WanderSettings.NewDestinationRadius);

        if ((destination - entity.transform.position).sqrMagnitude <= entity.MinDestinationDistance * entity.MinDestinationDistance) {
            destination = (entity.Agent.destination - entity.transform.position).normalized;
            destination *= entity.WanderSettings.NewDestinationRadius / 2f;

            NavMeshHit hit;
            NavMesh.SamplePosition(entity.transform.position + destination, out hit, entity.WanderSettings.NewDestinationRadius, -1);
			destination = hit.position;
        }

        return destination;
    }
}

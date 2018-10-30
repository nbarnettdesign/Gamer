using UnityEngine;

public class EntitySeekTarget : IEntityState {
    private bool staticTarget;
    private Transform target;

    private float moveTime;
    private float stopTime;
    private bool isMoving;
    private float count;

    public void OnStateEnter (MoveableEntity entity) {
        if (entity is SeekingEntity) {
            SeekingEntity s = entity as SeekingEntity;
            target = entity.Target.transform;
            staticTarget = s.StaticTarget;
            moveTime = s.MoveTime;
            stopTime = s.StopTime;
        } else { // Fallback in case of non seeking entity used
            target = EntityController.Instance.SeekerTarget;
            staticTarget = true;
            moveTime = 0f;
            stopTime = 0f;
        }

        entity.Agent.SetDestination(target.position);
        isMoving = stopTime == 0f;
        entity.Agent.isStopped = !isMoving;
    }

    public void OnStateUpdate (MoveableEntity entity) {
        if (isMoving) {
            count += Time.deltaTime;
            if(count >= moveTime) {
                count = 0f;
                isMoving = false;
                entity.Agent.isStopped = true;
            }
        } else {
            count += Time.deltaTime;
            if (count >= stopTime) {
                count = 0f;
                isMoving = true;
                entity.Agent.isStopped = false;
            }
        }

        if(staticTarget == false && entity.Agent.destination != target.position &&
            (target.position - entity.transform.position).sqrMagnitude > entity.MinDestinationDistance * entity.MinDestinationDistance)
            entity.Agent.SetDestination(target.position);

        if (entity.Agent.destination != entity.transform.position &&
            (target.position - entity.transform.position).sqrMagnitude <= entity.MinDestinationDistance * entity.MinDestinationDistance) {
            entity.Agent.SetDestination(entity.transform.position);
        }
    }

    public void OnStateExit (MoveableEntity entity) {
        
    }
}

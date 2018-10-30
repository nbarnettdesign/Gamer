using UnityEngine;

public class EntityAlert : IEntityState
{
    Vector3 previousDestination;
    bool updatedRotationAnimation;
    bool updatedWalkAnimation;

    public void OnStateEnter(MoveableEntity entity)
    {
        entity.Agent.SetDestination(entity.transform.position);
        previousDestination = entity.DetectedPoint;
    }
    public void OnStateUpdate(MoveableEntity entity)
    {
        if (entity.DetectedPoint != previousDestination)
            entity.Agent.SetDestination(entity.transform.position);
        else if (entity.Agent.destination != entity.transform.position && (entity.Agent.destination - entity.EyePosition).sqrMagnitude < entity.MinDestinationDistance * entity.MinDestinationDistance)
        {
            entity.Agent.SetDestination(entity.transform.position);
            entity.DestinationReached();
            return;
        }
        else if (entity.Agent.destination == previousDestination)
            return;

        if (entity.Agent.destination == entity.transform.position)
        {
            Vector3 targetDirection = previousDestination - entity.EyePosition;

            if (Vector3.Angle(targetDirection, entity.transform.forward) > entity.MinLookAngle)
            {
                if (updatedRotationAnimation == false)
                {
                    updatedWalkAnimation = false;
                    updatedRotationAnimation = true;
                    entity.AnimateRotation();
                }

                float step = entity.RotationSpeed * Time.deltaTime;
                Vector3 newDirection = Vector3.RotateTowards(entity.transform.forward, targetDirection, step, 0f);

                entity.transform.rotation = Quaternion.LookRotation(newDirection);
                entity.transform.rotation = Quaternion.Euler(0, entity.transform.rotation.eulerAngles.y, entity.transform.rotation.eulerAngles.z);
            }
            else
            {
                if(updatedWalkAnimation == false)
                {
                    updatedWalkAnimation = true;
                    updatedRotationAnimation = false;
                    entity.AnimateWalk();
                }

                entity.Agent.SetDestination(previousDestination);
            }
        }
    }

    public void OnStateExit(MoveableEntity entity)
    {

    }
}

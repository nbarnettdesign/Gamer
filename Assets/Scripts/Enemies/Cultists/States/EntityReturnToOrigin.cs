using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityReturnToOrigin : IEntityState
{
    public void OnStateEnter(MoveableEntity entity)
    {
        entity.Agent.SetDestination(entity.transform.position);
    }

    public void OnStateUpdate(MoveableEntity entity)
    {
        if (entity.Agent.destination == entity.transform.position)
        {
            Vector3 targetDirection = entity.OriginPoint - entity.transform.position;
            targetDirection.y = entity.transform.position.y;

            if (Vector3.Angle(targetDirection, entity.transform.forward) > entity.MinLookAngle)
            {
                float step = entity.RotationSpeed * Time.deltaTime;
                Vector3 newDirection = Vector3.RotateTowards(entity.transform.forward, targetDirection, step, 0f);

                entity.transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else
                entity.Agent.SetDestination(entity.OriginPoint);
        }
        else
        {
            Vector3 direction = entity.OriginPoint - entity.transform.position;
            direction.y = entity.transform.position.y;

            if (direction.sqrMagnitude <= entity.MinDestinationDistance * entity.MinDestinationDistance)
                entity.DestinationReached();
        }
    }

    public void OnStateExit(MoveableEntity entity)
    {
        
    }
}

using UnityEngine;

public class EntityMoveToTarget : IEntityState
{
    bool updatedRotationAnimation;
    bool updatedWalkAnimation;

    public void OnStateEnter(MoveableEntity entity)
    {
        entity.Agent.SetDestination(entity.transform.position);
    }

    public void OnStateUpdate(MoveableEntity entity)
    {
        RaycastHit hit = new RaycastHit();

        Vector3 heading = entity.Target.transform.position - entity.transform.position;
        float distance = heading.magnitude;

        Vector3 rayPos = entity.EyePosition;
        rayPos.y -= 1;

        if (Physics.SphereCast(rayPos, 0.5f, heading, out hit,
            distance, entity.EnvironmentLayer))
        {
            if (updatedWalkAnimation == false)
            {
                updatedWalkAnimation = true;
                updatedRotationAnimation = false;
                entity.AnimateWalk();
            }

            entity.Agent.SetDestination(entity.Target.transform.position);
        }
        else
        {
            Vector3 targetDirection = entity.Target.transform.position - entity.EyePosition;

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

                if (entity.Agent.destination != entity.transform.position)
                    entity.Agent.SetDestination(entity.transform.position);
            }
            else
            {
                if ((entity.Target.transform.position - entity.EyePosition).sqrMagnitude > entity.MinDestinationDistance * entity.MinDestinationDistance)
                {
                    if (updatedWalkAnimation == false)
                    {
                        updatedWalkAnimation = true;
                        updatedRotationAnimation = false;
                        entity.AnimateWalk();
                    }

                    entity.Agent.SetDestination(entity.Target.transform.position);
                }
                else
                {
                    if (entity.Agent.destination != entity.transform.position)
                    {
                        entity.Agent.SetDestination(entity.transform.position);
                        entity.DestinationReached();
                    }
                }
            }
        }
    }

    public void OnStateExit(MoveableEntity entity)
    {

    }
}

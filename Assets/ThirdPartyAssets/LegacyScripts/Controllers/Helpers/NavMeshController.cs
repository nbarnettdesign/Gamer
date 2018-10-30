using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : Singleton<NavMeshController>
{
    public Vector3 RandomNavSphere(Vector3 origin, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;

        randomDirection += origin;
        randomDirection.y = origin.y;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, radius, -1);
        return navHit.position;
    }
}

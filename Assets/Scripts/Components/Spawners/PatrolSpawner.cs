using UnityEngine;

public class PatrolSpawner : ObjectSpawner
{
    protected override void SpawnObject(Vector3 location)
    {
        GameObject spawn = SimplePool.Spawn(spawnObjects[Random.Range(0, spawnObjects.Count)], location, Quaternion.identity);
        spawn.transform.SetParent(spawnwedObjectParent);

        PatrollingEntity p = spawn.GetComponent<PatrollingEntity>();

        if (p == null)
        {
            Debug.LogError(string.Format("{0} was spawned from a patrol spawner but has no PatrollingEntity component", spawn.name));
            return;
        }

        p.GivePatrolPoints(spawnPoints);
    }
}

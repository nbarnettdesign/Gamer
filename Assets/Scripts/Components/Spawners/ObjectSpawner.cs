using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviourExtended
{
    [SerializeField] protected List<GameObject> spawnObjects;
    [Header("Spawning Settings")]
    [SerializeField] protected int initialSpawnNumber;
    [SerializeField] protected float randomSpawnOffset;
    [SerializeField] protected int maxSpawnNumber;
    // Public due to editor script ObjectSpawnerEditor.cs
    public List<Vector3> spawnPoints;
    [SerializeField] protected float timeBetweenSpawns;
    [SerializeField] protected Transform targetObject;
    [SerializeField] protected Transform spawnwedObjectParent;
    [Tooltip("The range from the target to check for spawn locations")]
    [SerializeField] protected float spawnRange;

    protected float spawnCount;

    protected override void Start()
    {
        base.Start();

        if (spawnwedObjectParent == null || maxSpawnNumber > 0)
        {
            spawnwedObjectParent = transform;
        }

        if (initialSpawnNumber > 0)
        {
            for (int i = 0; i < initialSpawnNumber; i++)
            {
                SpawnObject(GetSpawnPoint());
            }
        }
    }

    protected virtual void Update()
    {
        if (IsRendering() == false)
            return;

        UpdateSpawnTime();
    }

    private void UpdateSpawnTime()
    {
        if (maxSpawnNumber > 0 && spawnwedObjectParent.childCount > maxSpawnNumber) return;

        spawnCount += Time.deltaTime;

        if (spawnCount > timeBetweenSpawns)
        {
            spawnCount = 0f;
            SpawnObject(GetSpawnPoint());
        }
    }

    protected virtual Vector3 GetSpawnPoint()
    {
        //If we don't have a target object just spawn at a random spawn point
        if (targetObject == null)
            return spawnPoints[Random.Range(0, spawnPoints.Count)] + Random.insideUnitSphere * randomSpawnOffset;

        //Do we have any spawn points in range of the target?
        //If we do spawn at a random point within range of the target
        List<Vector3> pointsInRange = new List<Vector3>();

        foreach (Vector3 p in spawnPoints)
        {
            if ((targetObject.position - p).sqrMagnitude <= spawnRange * spawnRange)
                pointsInRange.Add(p);
        }

        if (pointsInRange.Count > 0)
        {
            return pointsInRange[Random.Range(0, pointsInRange.Count)];
        }

        //If we don't, spawn at the closest spawn point to the target
        Vector3 point = Vector3.zero;
        float distSqr = Mathf.Infinity;

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            float tempDistSqr = (spawnPoints[i] - targetObject.position).sqrMagnitude;

            if (tempDistSqr < distSqr)
            {
                point = spawnPoints[i];
                distSqr = tempDistSqr;
            }
        }

        return point + Random.insideUnitSphere * randomSpawnOffset;
    }

    protected virtual void SpawnObject(Vector3 location)
    {
        GameObject spawn = SimplePool.Spawn(spawnObjects[Random.Range(0, spawnObjects.Count)], location, Quaternion.identity);
        spawn.transform.SetParent(spawnwedObjectParent);
    }
}

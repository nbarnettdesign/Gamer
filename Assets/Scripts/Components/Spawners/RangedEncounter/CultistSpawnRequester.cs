using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistSpawnRequester : MonoBehaviour {
    [SerializeField] private float requestDelay;
    [SerializeField] private bool requestOnStart;
    [Tooltip("The time between checking if the point has a minion")]
    [SerializeField] private float minionCheckTime;

    private DoorSpawnPoint[] validSpawnLocations;
    private float checkCount;
    private RangedCultistSpawner spawner;
    private GameObject currentCultist;
    private bool requestedCulist;
    private bool shouldRequest;

    private void Start()
    {
        if (requestOnStart)
        {
            StartRequesting();
            this.Invoke(StartRequestMinion, requestDelay);
        }
    }

    private void Update()
    {
        CheckMinion();
    }

    public void Init(RangedCultistSpawner spawner, List<DoorSpawnPoint> spawnPoints)
    {
        this.spawner = spawner;

        // We only care about the closest 2 spawn locations
        validSpawnLocations = new DoorSpawnPoint[2];

        DoorSpawnPoint closest = spawnPoints[0];
        DoorSpawnPoint secondClosest = spawnPoints[0];
        float closestDistSqr = Mathf.Infinity;
        float secondClosestDistSqr = Mathf.Infinity;

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            float temp = (spawnPoints[i].transform.position - transform.position).sqrMagnitude;

            if (closestDistSqr > temp)
            {
                closestDistSqr = temp;
                closest = spawnPoints[i];
            } else if (secondClosestDistSqr > temp)
            {
                secondClosestDistSqr = temp;
                secondClosest = spawnPoints[i];
            }
        }

        validSpawnLocations[0] = closest;
        validSpawnLocations[1] = secondClosest;
    }

    public void GiveCultist(GameObject cultist)
    {
        currentCultist = cultist;
    }

    public void StartRequesting()
    {
        shouldRequest = true;
    }

    public void StopRequesting()
    {
        shouldRequest = false;
    }

    private void StartRequestMinion()
    {
        if (spawner == null)
            return;

        // Get a spawn location
        DoorSpawnPoint spawnPoint = validSpawnLocations[Random.Range(0, validSpawnLocations.Length)];

        // Open its door
       // spawnPoint.OpenDoor();
        // Wait for door open time and
        // Actually get the minion to spawn
        StartCoroutine(RequestMinion(spawnPoint.transform.position, spawnPoint.DoorOpenTime));
    }

    private IEnumerator RequestMinion(Vector3 spawnPoint, float delay)
    {
        yield return new WaitForSeconds(delay);

        currentCultist = spawner.RequestSpawn(this, spawnPoint);
        requestedCulist = false;
    }

    private void CheckMinion()
    {
        if (currentCultist && currentCultist.activeInHierarchy == false)
            currentCultist = null;

        if (shouldRequest  == false|| currentCultist || requestedCulist)
            return;

        if(checkCount < minionCheckTime)
        {
            checkCount += Time.deltaTime;
            return;
        }

        checkCount = 0f;

        if(currentCultist == null)
        {
            requestedCulist = true;
            this.Invoke(StartRequestMinion, requestDelay);
        }
    }
}

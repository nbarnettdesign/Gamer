using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RangedCultistSpawner : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private int spawnNumber;
    [SerializeField] private List<DoorSpawnPoint> spawnPoints;
    [SerializeField] private List<CultistSpawnRequester> standingLocations;
    [SerializeField] private bool spawnOnStart;
    [SerializeField] private UnityEvent onSpawnFinished;
    [SerializeField] private Transform spawnedParent;


    [SerializeField] private GameObject cultistPrefab;

    private Transform target;
    private bool spawn;
    private int currentSpawnNumber;
    private int spawnCount;

    private void Start()
    {
        if (spawnOnStart)
            spawn = true;

        for (int i = 0; i < standingLocations.Count; i++)
        {
            standingLocations[i].Init(this, spawnPoints);

            if(spawn)
                standingLocations[i].StartRequesting();
        }

        target = FindObjectOfType<DamageablePlayer>().transform;

        if (spawnedParent == null)
            spawnedParent = transform;
    }

    public void StartSpawns()
    {
        if (spawn == true || spawnCount >= spawnNumber)
            return;

        spawnCount = 0;
        spawn = true;

        if (standingLocations == null || standingLocations.Count <= 0)
            return;

        for (int i = 0; i < standingLocations.Count; i++)
        {
            standingLocations[i].StartRequesting();
        }
    }

    public void StopSpawn()
    {
        spawn = false;

        if (standingLocations == null || standingLocations.Count <= 0)
            return;

        for (int i = 0; i < standingLocations.Count; i++)
        {
            standingLocations[i].StopRequesting();
        }
    }

    public GameObject RequestSpawn(CultistSpawnRequester spawnRequester, Vector3 spawnLocation)
    {
        if (spawnCount >= spawnNumber)
        {
            onSpawnFinished.Invoke();
            StopSpawn();
            return null;
        }

        currentSpawnNumber++;

        spawnCount++;
        GameObject cultist = SimplePool.Spawn(cultistPrefab, spawnLocation, Quaternion.identity);

        // Make sure the cultist is looking in the direction of the way they are going
        cultist.transform.LookAt(spawnRequester.transform);

        cultist.GetComponent<CultistRanged>().Init(target, spawnRequester.transform.position, spawnedParent, true);

        return cultist;
    }
}

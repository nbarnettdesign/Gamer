using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class WaveSpawner : MonoBehaviourExtended {
    [Header("Waves")]
    [Tooltip("If set to 0 will continue to spawn indefinately")]
    [SerializeField] private int numberOfWaves;
    [Tooltip("The number of objects to spawn at once")]
    [SerializeField] private int spawnAtOnce;
    [SerializeField] private List<Wave> waves;
    [SerializeField] private bool shuffleWaves;

    [Header("Spawning Delaying")]
    [SerializeField] private float spawnDelay;
    [SerializeField] private float spawnIncreasePerSpawn;
    [SerializeField] private float maxSpawnDelay;

    [Header("Targeting")]
    [SerializeField] private float maxSpawnRange;
    [Tooltip("Time, in seconds until a range check is performed")]
    [SerializeField] private float spawnRangeCheckTime;
    [SerializeField] private bool targetPlayer = true;

    [Header("Debugging")]
    [SerializeField] private Color gizmoColour;
    [SerializeField] private bool displaySpawnRange;

    private GameObject target;

    private Transform[] spawnPoints;
    private int spawnPointCount;
    private float currentSpawnTime;
    private float currentSpawnCount;

    private bool shouldSpawn;
    private int wavesSpawned;
    private float rangeCheckCount;

    private Wave currentWave;
    private int currentWaveNum;
    private int waveCount;

    protected override void Start()
    {
        base.Start();

        if(transform.childCount <= 0)
        {
            Debug.LogError("Wave spawner needs children to convert to spawn points!");
            return;
        }
        else
        {
            spawnPoints = new Transform[transform.childCount];
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                spawnPoints[i] = transform.GetChild(i);
            }
        }

        currentSpawnTime = spawnDelay;

        if (targetPlayer)
        {
            target = GameObject.FindWithTag("Player");

            if (target.transform.root)
            {
                target = target.transform.root.GetComponentInChildren<BodyCollider>().gameObject;
            }
            else if (target.GetComponentInChildren<BodyCollider>())
            {
                target = target.GetComponentInChildren<BodyCollider>().gameObject;
            }
        }

        if (shuffleWaves)
        {
            waves.Shuffle();
        }
    }

    private void Update()
    {
        if (IsRendering() == false || shouldSpawn == false)
            return;

        CheckSpawnRange();

        // This is here just to make sure we don't keep on updating
        // spawns on the same frame as CheckSpawnRange switching shouldSpawn
        // to false when the target goes out of range
        if (shouldSpawn == false)
            return;

        UpdateSpawn();
    }

    public void StartSpawn()
    {
        Debug.Log("StartSpawn");
        shouldSpawn = true;
    }

    private void CheckSpawnRange()
    {
        if (target == null) return;

        if(rangeCheckCount < spawnRangeCheckTime)
        {
            rangeCheckCount++;
            return;
        }

        rangeCheckCount = 0f;

        if ((target.transform.position - transform.position).sqrMagnitude > maxSpawnRange * maxSpawnRange)
            shouldSpawn = false;
    }

    private void UpdateSpawn()
    {
        if(currentSpawnCount < currentSpawnTime)
        {
            currentSpawnCount += Time.deltaTime;
            return;
        }

        currentSpawnCount = 0f;
        Spawn();
    }

    private void Spawn()
    {
        if(currentWave == null)
            NextWave();

        for (int i = 0; i < spawnAtOnce; i++)
        {
            GameObject spawnObject = currentWave.GetNextObject();

            if (spawnObject == null)
            {
                NextWave();

                if (shouldSpawn == false)
                    return;

                spawnObject = currentWave.GetNextObject();
            }

            if (spawnObject == null)
                return;

            GameObject spawnedObject = SimplePool.Spawn(spawnObject, NextSpawnPoint(), Quaternion.identity);

            if (spawnedObject.GetComponentInChildren<Minion>())
                spawnedObject.GetComponentInChildren<Minion>().GiveTarget(target.transform);
        }
    }

    private void NextWave()
    {
        currentWave = waves[currentWaveNum];
        currentWaveNum++;

        if (currentWaveNum >= waves.Count)
            currentWaveNum = 0;

        waveCount++;

        if (waveCount > numberOfWaves)
            shouldSpawn = false;
    }

    private Vector3 NextSpawnPoint()
    {
        Vector3 pos = spawnPoints[spawnPointCount].position;

        spawnPointCount++;

        if (spawnPointCount >= spawnPoints.Length)
        {
            spawnPoints.Shuffle();
            spawnPointCount = 0;
        }

        return pos;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (numberOfWaves <= 0)
            numberOfWaves = 1;

        if (spawnAtOnce <= 0)
            spawnAtOnce = 1;
    }

    private void OnDrawGizmos()
    {
        if (displaySpawnRange == false)
            return;

        if (Gizmos.color != gizmoColour)
            Gizmos.color = gizmoColour;

        Gizmos.DrawWireSphere(transform.position, maxSpawnRange);
    }
#endif
}

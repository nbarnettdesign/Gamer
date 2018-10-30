using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;
using UnityEngine;


public class ArmedSpawner : MonoBehaviourExtended
{
    [Header("Settings")]
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private float detectionRange;
    [SerializeField] private float rangeCheckTime;
    [SerializeField] private float viewThreshold = 1.5f;
    [SerializeField] private float spawnCooldownTime = 1.0f;

    [Header("Minion Settings")]
    [SerializeField] private GameObject spawnedMinionPrefab;

    private float spawnTimer;

    private List<MinionSpawner> minionSpawners;
    private int minionCount;

    private Player player;
    private Transform playerTransform;

    private bool isEnabled;
    private bool targetInRange;
    private float rangeTimer;

    //NEEDTO 
    //
    // Add feedback for spawning, about to spawn and not ready to spawn
    // Make line renderer always visible (idle rotation) 

    protected override void Start()
    {
        base.Start();

        //Populate masked horrors from children
        minionSpawners = new List<MinionSpawner>();
        transform.parent.GetComponentsInChildren(minionSpawners);
        minionSpawners.Shuffle();

        player = Player.Instance;
        // Improve this
        if(player.GetComponentInChildren<BodyCollider>())
            playerTransform = player.GetComponentInChildren<BodyCollider>().transform;

        rangeTimer = rangeCheckTime;
    }

    public void GetDead()
    {
        Destroy(this);
    }

    private void Update()
    {
        if (IsRendering() == false)
            return;

        TargetInRange();
        UpdateSpawn();
    }

    public void Reset()
    {
        for (int i = 0; i < minionSpawners.Count; i++)
        {
            minionSpawners[i].ResetSpawn();
        }
    }

    private void TargetInRange()
    {
        if(rangeTimer < rangeCheckTime) {
            rangeTimer += Time.deltaTime;
            return;
        }

        rangeTimer = 0f;

        targetInRange = (player.transform.position - transform.position).sqrMagnitude 
            <= detectionRange * detectionRange;
    }

    private void UpdateSpawn()
    {
        if (targetInRange == false || playerTransform == null)
            return;

        // Fire countdownTimer
        if (spawnTimer < spawnCooldownTime) {
            spawnTimer += Time.deltaTime;
            return;
        }

        spawnTimer = 0f;

        // Can we see the player?
        if (Vector3.Angle(playerTransform.position - transform.position, transform.forward) < viewThreshold) {
            //check if we can see player without hitting environment


            RaycastHit hit;
            if(Physics.Raycast(transform.position, player.headCollider.transform.position - transform.position, out hit, detectionRange, detectionMask))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    SpawnNextMinion(playerTransform);
                }
            }
        }
    }

    private void SpawnNextMinion(Transform target)
    {
        while (minionCount < minionSpawners.Count)
        {
            if (minionSpawners[minionCount].CanSpawn)
            {
                minionSpawners[minionCount].SpawnMinion(target);
                minionCount++;
                break; 
            }
            else
                minionCount++;
        }

        if (minionCount >= minionSpawners.Count)
        {
            minionSpawners.Shuffle();
            minionCount = 0;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<Arrow>())
        {
            Reset();
        }
    }
}

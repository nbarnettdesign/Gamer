using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSpawner : MonoBehaviour {
    [SerializeField] private Minion minionToSpawn;
    [SerializeField] private Transform target;
    [SerializeField] private float spawnDelay;
    [SerializeField] private bool autoSpawn;
    [SerializeField] private bool delayFirstSpawn;

    [Header("Spawning Offset")]
    [SerializeField] private float offsetRadius;
    [SerializeField] private Axis forwardDirection;
    [SerializeField] private bool drawGizmo;
    [SerializeField] private bool selectionOnly;
    [SerializeField] private Color gizmoColour;

    [Header("Portal Scaling")]
    [SerializeField] private float closeScaleTime;
    [SerializeField] private AnimationCurve closeCurve;

    [Header("Burst Spawning")]
    [SerializeField] private float burstSpawnNumber;
    [SerializeField] private AudioClip burstSpawnSound;
    [Tooltip("Delay between each spawn on a burst")]
    [SerializeField] private float burstDelay;

    private Transform portal;

    private float spawnCount;
    private bool shouldSpawn;

    IEnumerator test;

    private void Start()
    {
        if (autoSpawn)
            shouldSpawn = true;

        if(delayFirstSpawn == false)
            spawnCount = spawnDelay;

        portal = transform.GetChild(0);
    }

    private void Update()
    {
        CheckSpawn();
    }

    public void StartSpawn()
    {
        shouldSpawn = true;
    }

    public void DeactivatePortal()
    {
        shouldSpawn = false;

        if (portal == null)
            return;

        for (int i = 0; i < portal.transform.childCount; i++)
        {
            StartCoroutine(LerpController.Instance.Scale(portal.GetChild(i), portal.GetChild(i).localScale, 
                Vector3.zero, closeScaleTime, closeCurve));
        }

        StartCoroutine(LerpController.Instance.Scale(portal, portal.localScale,
                Vector3.zero, closeScaleTime, closeCurve));

        this.Invoke(TurnOffPortal, closeScaleTime);
    }

    public void BurstSpawn()
    {
        StartCoroutine(Burst());
    }

    private void CheckSpawn()
    {
        if (shouldSpawn == false)
            return;

        if(spawnCount < spawnDelay)
        {
            spawnCount += Time.deltaTime;
            return;
        }

        spawnCount = 0f;
        Spawn();
    }

    private void Spawn()
    {
        Vector3 spawnLocation = transform.position + (Random.insideUnitSphere * offsetRadius);

        switch (forwardDirection)
        {
            case Axis.X:
                spawnLocation.x = transform.position.x;
                break;
            case Axis.Y:
                spawnLocation.y = transform.position.y;
                break;
            case Axis.Z:
                spawnLocation.z = transform.position.z;
                break;
            default:
                break;
        }

        Minion minion = SimplePool.Spawn(minionToSpawn.gameObject, spawnLocation, 
            minionToSpawn.transform.rotation).GetComponent<Minion>();

        minion.GiveTarget(target);
    }

    private void TurnOffPortal()
    {
        if (portal == null)
            return;

        portal.gameObject.SetActive(false);
    }

    private IEnumerator Burst()
    {
        if (burstSpawnSound)
            AudioSource.PlayClipAtPoint(burstSpawnSound, transform.position);

        for (int i = 0; i < burstSpawnNumber; i++)
        {
            Spawn();
            yield return new WaitForSeconds(burstDelay);
        }
    }

#if UNITY_EDITOR
    private void DrawGizmo()
    {
        if (Gizmos.color != gizmoColour)
            Gizmos.color = gizmoColour;

        Gizmos.DrawSphere(transform.position, offsetRadius);
    }

    private void OnDrawGizmos()
    {
        if (drawGizmo && selectionOnly == false)
            DrawGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (drawGizmo && selectionOnly)
            DrawGizmo();
    }
#endif
}

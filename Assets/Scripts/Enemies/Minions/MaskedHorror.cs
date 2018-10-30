using UnityEngine;

public class MaskedHorror : Minion
{
    [Header("Explode Distance")]
    [SerializeField] private float distanceCheckTime;
    [SerializeField] private float minTargetDistance; 

    private TrailRenderer trail;
    private GameObject minionToSpawn;

    private bool canSpawn;
    private float timer;
    private float spawnCooldownTime;
    private float distanceCheckCount;

    private void OnEnable()
    {
        if (trail)
            trail.Clear();
    }

    protected override void Start()
    {
        base.Start();
        trail = GetComponentInChildren<TrailRenderer>();
    }

    private void Update()
    {
        MoveTowardsTarget();
        CheckRange();
    }

    private void MoveTowardsTarget()
    {
        if (target == null)
            return;

        Vector3 relativePos = (target.position + offset) - transform.position;

        if(Vector3.Dot(relativePos.normalized, transform.forward) < 0)
        {
            DespawnObject();
        }

        Quaternion rot = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, homingSensitivity);
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    private void CheckRange()
    {
        if (target == null)
            return;

        if(distanceCheckCount < distanceCheckTime)
        {
            distanceCheckCount += Time.deltaTime;
            return;
        }

        distanceCheckCount = 0f;

        if ((target.position - transform.position).sqrMagnitude <= minTargetDistance * minTargetDistance)
            Explode();
    }

    private void Explode()
    {
        target.GetComponent<DamageablePlayer>().Damage(damage, transform.position, transform.forward);
        DespawnObject();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawner : MonoBehaviourExtended
{
    [SerializeField] private GameObject minionToSpawn;
    [SerializeField] private float chargeTime;
    [SerializeField] private float spawnCooldownTime = 3.0f;

    [SerializeField] private ParticleSystem glowParticle;
    [SerializeField] private ParticleSystem spawnParticle;
    [SerializeField] private AnimationCurve maskGrowCurve;

    public bool CanSpawn { get { return canSpawn; } }
    private bool canSpawn;
    private float spawnTimer;

    private bool shouldSpawn;
    private float chargeCount;
    private Transform target;

    protected override void Start()
    {
        base.Start();

        if(glowParticle)
            glowParticle.Stop();

        transform.localScale = Vector3.zero;
        canSpawn = true;
    }

    void Update()
    {
        if (IsRendering() == false)
            return;

        UpdateCooldown();
        UpdateSpawn();
    }

    public void SpawnMinion(Transform target)
    {
        if (glowParticle)
            glowParticle.Play();

        StartCoroutine(GrowMask());

        this.target = target;
        canSpawn = false;
        shouldSpawn = true;
    }

    public void ResetSpawn()
    {
        if (shouldSpawn == false)
            return;

        transform.localScale = Vector3.zero;
        chargeCount = 0f;
    }

    private void UpdateCooldown()
    {
        if (canSpawn || shouldSpawn)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnCooldownTime)
            canSpawn = true;
    }

    private void UpdateSpawn()
    {
        if (shouldSpawn == false)
            return;

        chargeCount += Time.deltaTime;

        if(chargeCount >= chargeTime)
        {
            chargeCount = 0f;
            shouldSpawn = false;

            glowParticle.Stop();
            transform.localScale = Vector3.zero;

            GameObject GO = SimplePool.Spawn(minionToSpawn, transform.position, transform.rotation);
            GO.GetComponent<Minion>().GiveTarget(target);
        }
    }

    private IEnumerator GrowMask()
    {
        float t = 0f;

        while (t <= chargeTime)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, maskGrowCurve.Evaluate(t/chargeTime));
            yield return null;
        }

        if (spawnParticle)
        {
            SimplePool.Spawn(spawnParticle.gameObject, transform.position, transform.rotation);
        }
    }
}

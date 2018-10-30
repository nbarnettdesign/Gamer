using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Explodeable : Affectable
{
    [Header("Explosion")]
    [SerializeField] private float explodeRadius;
    [SerializeField] private LayerMask explodeLayer;
    [SerializeField] private float explosionForce;
    [SerializeField] private float explodeDamage;
    [SerializeField] private float explodeTime;
    [SerializeField] private GameObject explosionParticle;
    [SerializeField] private List<GameObject> explodedObjects;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private List<ExplodeEffect> explosionEffects;
    [SerializeField] private bool explodeOnImpact;
    [SerializeField] private float impactExplodeVelocity;
    [SerializeField] private bool stickOnImpact;

    [Header("Debugging")]
    [SerializeField] private bool drawExplodeRadius;

    public bool ExplodeOnImpact { get { return explodeOnImpact; } }

    private bool hasExploded;
    private Throwable throwable;
    private Rigidbody rbody;

    private bool hasTriggeredExplosion;

    private void Awake()
    {
        throwable = GetComponent<Throwable>();
        rbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (hasTriggeredExplosion || throwable == null || throwable.HasThrown == false) return;

        hasTriggeredExplosion = true;
        this.Invoke(TriggerExplosion, explodeTime);
    }

    public override void Explode(float explosionForce, Vector3 explosionOrigin, Vector3 impactVelocity)
    {
        StartExplosion();
    }

    public void ForceExplode()
    {
        TriggerExplosion();
    }

    public override void FireExposure(FireStrength strength)
    {
        StartExplosion();
    }

    private void StartExplosion()
    {
        if (explodeOnImpact)
        {
            TriggerExplosion();
        }
        else
        {
            this.Invoke(TriggerExplosion, explodeTime);
        }
    }

    private void TriggerExplosion()
    {
        if (hasExploded) return;

        hasExploded = true;

        if (explosionParticle != null)
        {
            SimplePool.Spawn(explosionParticle, transform.position, Quaternion.identity);
        }

        if (explodedObjects != null && explodedObjects.Count > 0)
        {
            for (int i = 0; i < explodedObjects.Count; i++)
            {
                SimplePool.Spawn(explodedObjects[i], transform.position, Quaternion.identity);
            }
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explodeRadius, explodeLayer);

        if (hitColliders.Length > 0 && explodeDamage > 0)
        {
            foreach (Collider c in hitColliders)
            {
                if (c.transform == transform) continue;

                Affectable[] affectables = c.GetComponents<Affectable>();

                for (int i = 0; i < affectables.Length; i++)
                {
                    if (explodeDamage > 0)
                    {
                        affectables[i].Damage(explodeDamage, transform.position,
                            (c.transform.position - transform.position).normalized * explosionForce, true);
                    }

                    affectables[i].Explode(explosionForce, transform.position, (affectables[i].transform.position - transform.position) * explosionForce);

                    if (c.GetComponent<Rigidbody>() != null)
                    {
                        c.GetComponent<Rigidbody>().AddExplosionForce(explosionForce * 200, transform.position, explodeRadius * 2);
                    }
                }
            }
        }

        for (int i = 0; i < explosionEffects.Count; i++)
        {
            if (explosionEffects[i] == null)
                continue;

            explosionEffects[i].Trigger(transform.position, explodeRadius);
        }

        StartCoroutine(Remove());
    }

    private void Stick(Transform stickToTransform)
    {
        transform.SetParent(stickToTransform);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private IEnumerator Remove()
    {
        if (throwable != null && throwable.AttachedToHand != null)
            throwable.AttachedToHand.DetachObject(gameObject, throwable.restoreOriginalParent);

        yield return new WaitForEndOfFrame();
        SimplePool.Despawn(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rbody && explodeOnImpact && rbody.velocity.magnitude >= impactExplodeVelocity)
        {
            TriggerExplosion();
        }
        else if (throwable)
        {
            if (throwable.HasThrown && explodeOnImpact)
                TriggerExplosion();
            else if (throwable.HasThrown && stickOnImpact)
                Stick(collision.transform);
        }
    }

    protected void OnValidate()
    {
        if (explodeOnImpact)
        {
            if (stickOnImpact)
            {
                stickOnImpact = false;
            }
        }

        if (stickOnImpact)
        {
            if (explodeOnImpact)
            {
                explodeOnImpact = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (drawExplodeRadius)
        {
            Gizmos.DrawWireSphere(transform.position, explodeRadius);
        }
    }
}


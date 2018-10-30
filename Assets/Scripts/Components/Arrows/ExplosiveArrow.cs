using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ExplosiveArrow : Arrow
{
    [Header("Explosive Options")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionForce;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private List<ExplodeEffect> explosionEffects;

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (explosionEffect != null)
        {
            SimplePool.Spawn(explosionEffect, transform.position, Quaternion.identity);
        }

        Affectable[] affectables = collision.transform.GetComponents<Affectable>();

        for (int i = 0; i < affectables.Length; i++)
        {
            affectables[i].Explode(explosionForce, transform.position, ArrowHeadRB.velocity);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            affectables = hitColliders[i].GetComponents<Affectable>();

            for (int j = 0; j < affectables.Length; j++)
            {
                affectables[j].Damage(damage, transform.position, ArrowHeadRB.velocity * explosionForce, true);
                affectables[j].Explode(explosionForce, transform.position, ArrowHeadRB.velocity * explosionForce);
            }
        }

        if (explosionEffects.Count > 0)
        {
            for (int i = 0; i < explosionEffects.Count; i++)
            {
                explosionEffects[i].Trigger(transform.position, explosionRadius);
            }
        }

        SimplePool.Despawn(gameObject);
    }
}

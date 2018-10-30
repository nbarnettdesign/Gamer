using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviourExtended
{
    [SerializeField] protected float damage;
    [SerializeField] protected LayerMask damagingMask;
    [SerializeField] protected bool instaKill;

    private void OnTriggerEnter(Collider other)
    {
        if (damagingMask == (damagingMask | (1 << other.gameObject.layer)))
        {
            Affectable a = other.transform.GetComponentInChildren<Affectable>();

            if (a)
                a.Damage(damage, transform.position, transform.position, false, instaKill);
        }
    }
}

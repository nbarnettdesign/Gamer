using System.Collections.Generic;
using UnityEngine;

public class DamageChildren : MonoBehaviour {
    private List<Damageable> damageables;

    private void Start() {
        damageables = new List<Damageable>();
        GetComponentsInChildren(damageables);
    }

    public void ApplyDamage(float damage, Vector3 impactPosition, Vector3 impactVelocity) {
        foreach (Damageable d in damageables) {
            d.Damage(damage, impactPosition, impactVelocity);
        }
    }
}

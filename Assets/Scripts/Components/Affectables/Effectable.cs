using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Effectable : MonoBehaviour {
    [Header("Effected by Settings")]
    [SerializeField] protected bool inherritSettings;
	[SerializeField] protected bool explosionImmune;
	[SerializeField] protected bool explosionOnly;
    [SerializeField] protected bool arrowImmune;
    [SerializeField] protected bool impactImmune;

    public bool InherritSettings { get { return inherritSettings; } }
    public bool ExplosionImmune { get { return explosionImmune; } }
    public bool ExplosionOnly { get { return explosionOnly; } }
    public bool ArrowImmune { get { return arrowImmune; } }
    public bool ImpactImmune { get { return impactImmune; } }

    protected virtual void Start() {
        if (inherritSettings) {
            List<Effectable> effectables = GetComponents<Effectable>().ToList();

            Effectable inherritFrom = effectables.Find(e => e != this && e.InherritSettings == false);

            if (inherritFrom != null) {
                explosionImmune = inherritFrom.ExplosionImmune;
                explosionOnly = inherritFrom.ExplosionOnly;
                arrowImmune = inherritFrom.ArrowImmune;
                impactImmune = inherritFrom.ImpactImmune;
            } else
                Debug.LogError(string.Format("{0} is trying to inherrit effectable settings but has nothing to inherrit!", name));
        }
    }

	public virtual void Damage(float amount, Vector3 impactPosition, Vector3 impactVelocity, bool fromExplosion = false, bool instaKill = false) { }
	public virtual void Explode(float explosionForce, Vector3 explosionOrigin, Vector3 impactVelocity) { }
	public virtual void Impacted(ObjectWeight weight, Vector3 impactPoint, Vector3 impactVelocity) { }
	public virtual void FireExposure() { }
	public virtual void Heal(float amount) { }

	protected bool ExplosionCheck(bool fromExplosion) {
		return (explosionImmune && fromExplosion || explosionOnly && fromExplosion == false);
	}

	protected virtual void OnValidate() {
		if (explosionImmune) {
			if (explosionOnly)
				explosionOnly = false;
		} else if (explosionOnly) {
			if (explosionImmune)
				explosionImmune = false;
		}
	}
}

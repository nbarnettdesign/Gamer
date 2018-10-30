using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Affectable : MonoBehaviourExtended {
    [Header("Affected by Settings")]
    [SerializeField] protected bool inherritSettings;
	[SerializeField] protected bool explosionImmune;
    [SerializeField] protected bool arrowImmune;
    [SerializeField] protected bool impactImmune;

    public bool InherritSettings { get { return inherritSettings; } }
    public bool ExplosionImmune { get { return explosionImmune; } }
    public bool ExplosionOnly { get { return impactImmune && arrowImmune; } }
    public bool ArrowImmune { get { return arrowImmune; } }
    public bool ImpactImmune { get { return impactImmune; } }

    protected override void Start() {
        base.Start();

        if (inherritSettings) {
            List<Affectable> affectables = GetComponents<Affectable>().ToList();

            Affectable inherritFrom = affectables.Find(e => e != this && e.InherritSettings == false);

            if (inherritFrom != null) {
                explosionImmune = inherritFrom.ExplosionImmune;
                arrowImmune = inherritFrom.ArrowImmune;
                impactImmune = inherritFrom.ImpactImmune;
            } else
                Debug.LogError(string.Format("{0} is trying to inherrit affectable settings but has nothing to inherrit!", name));
        }
    }

	public virtual void Damage(float amount, Vector3 impactPosition, Vector3 impactVelocity, bool fromExplosion = false, bool instaKill = false) { }
	public virtual void Explode(float explosionForce, Vector3 explosionOrigin, Vector3 impactVelocity) { }
	public virtual void Impacted(ObjectWeight weight, Vector3 impactPoint, Vector3 impactVelocity) { }
	public virtual void FireExposure(FireStrength strength) { }
	public virtual void Heal(float amount) { }

	protected bool ExplosionCheck(bool fromExplosion) {
		return (explosionImmune && fromExplosion || ExplosionOnly && fromExplosion == false);
	}
}

using UnityEngine;

public class Breakable : Affectable {
	[SerializeField] private GameObject brokenObject;
	[SerializeField] private LayerMask brokenLayer;
	[SerializeField] private float minBreakForce;

	public override void Damage(float amount, Vector3 impactPosition, Vector3 impactVelocity, bool fromExplosion = false, bool instaKill = false) {
        if (ExplosionCheck(fromExplosion))
            return;

        Break(impactVelocity.magnitude, impactPosition, false);
	}

	private void Break(float breakForce, Vector3 damageOrigin, bool fromExplosion) {
		if (breakForce < minBreakForce || ExplosionOnly && fromExplosion == false)
			return;

		if (GetComponent<Rigidbody>() != null)
			GetComponent<Rigidbody>().isKinematic = true;
		if (GetComponent<Collider>() != null)
			GetComponent<Collider>().enabled = false;

		if (brokenObject != null) {
			GameObject obj = SimplePool.Spawn(brokenObject, transform.position, transform.rotation);

			if (fromExplosion) {
				foreach (Transform child in obj.transform) {
                    Rigidbody r = child.GetComponent<Rigidbody>();

                    if (r == null) continue;

                    r.AddExplosionForce(breakForce, damageOrigin,
                            (damageOrigin - transform.position).sqrMagnitude);
				}
			} else {
                foreach (Transform child in obj.transform)
                {
                    Rigidbody r = child.GetComponent<Rigidbody>();

                    if (r == null) continue;

                    r.AddForceAtPosition((damageOrigin - transform.position) * breakForce,
							child.position);
                }
			}
		}

		SimplePool.Despawn(gameObject);
	}
}

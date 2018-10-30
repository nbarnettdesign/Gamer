using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class Impactable : Affectable {
	[SerializeField] protected ObjectWeight breakableWeight;
	[Range(0f,1f),SerializeField] protected float minBreakVelocity;
	[SerializeField] protected GameObject brokenObject;
	[SerializeField] protected SoundPlayOneshot brokenSound;
	[SerializeField] protected GameObject brokenParticle;

    [SerializeField] private UnityEvent onBroken;

	public override void Impacted(ObjectWeight weight, Vector3 impactPoint, Vector3 impactVelocity) {
		if (weight >= breakableWeight && (minBreakVelocity == 0 || impactVelocity.magnitude >= minBreakVelocity))
			Break(impactPoint, impactVelocity);
	}

	private void Break(Vector3 impactPoint, Vector3 impactVelocity) {
		if(brokenObject != null) {
			GameObject broken = SimplePool.Spawn(brokenObject, transform.position, transform.rotation);
			Rigidbody rbody = broken.GetComponent<Rigidbody>();

			if (rbody != null)
				rbody.AddForceAtPosition(impactVelocity, impactPoint);

			foreach (Transform child in broken.transform) {
				rbody = child.GetComponent<Rigidbody>();

				if (rbody != null)
					rbody.AddForceAtPosition(impactVelocity, impactPoint);
			}
		}

        onBroken.Invoke();
        Remove();
	}

    protected virtual void Remove() {
        if (brokenParticle != null)
            SimplePool.Spawn(brokenParticle, transform.position, transform.rotation);

        if (brokenSound != null)
            brokenSound.Play();

        SimplePool.Despawn(gameObject);
    }
}

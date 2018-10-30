using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Impacter : MonoBehaviour {
	[SerializeField] private ObjectWeight weight;

	private Rigidbody rBody;

	private void Start() {
		rBody = GetComponent<Rigidbody>();
	}

	private void OnCollisionEnter(Collision collision) {
        if (rBody == null || rBody.velocity.magnitude < 0.01f)
            return;

		Affectable e = collision.transform.GetComponent<Affectable>();

		if (e != null && e.ImpactImmune == false) {
            e.Impacted(weight, collision.contacts[0].point, rBody.velocity);
        }
	}
}

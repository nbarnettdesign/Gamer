using UnityEngine;

namespace Valve.VR.InteractionSystem {
	public class DestroyOnTriggerEnter : MonoBehaviour {
		[SerializeField] private string tagFilter;

		private bool useTag;
		private void Start() {
			if (!string.IsNullOrEmpty(tagFilter))
				useTag = true;
		}

		private void OnTriggerEnter(Collider collider) {
			if (!useTag || (useTag && collider.gameObject.tag == tagFilter))
				SimplePool.Despawn(collider.gameObject.transform.root.gameObject);
		}
	}
}

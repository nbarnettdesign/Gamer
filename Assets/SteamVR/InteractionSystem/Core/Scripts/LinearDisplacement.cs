using UnityEngine;

namespace Valve.VR.InteractionSystem {
	public class LinearDisplacement : MonoBehaviour {
		[SerializeField] private Vector3 displacement;
		[SerializeField] private LinearMapping linearMapping;

		private Vector3 initialPosition;

		private void Start() {
			initialPosition = transform.localPosition;

			if (linearMapping == null)
				linearMapping = GetComponent<LinearMapping>();
		}

		private void Update() {
			if (linearMapping)
				transform.localPosition = initialPosition + linearMapping.value * displacement;
		}
	}
}

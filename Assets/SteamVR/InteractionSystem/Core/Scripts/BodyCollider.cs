using UnityEngine;

namespace Valve.VR.InteractionSystem {
	[RequireComponent(typeof(CapsuleCollider))]
	public class BodyCollider : MonoBehaviour {
		public Transform head;

		private CapsuleCollider capsuleCollider;

		void Awake() {
			capsuleCollider = GetComponent<CapsuleCollider>();
		}

		void FixedUpdate() {
			float distanceFromFloor = Vector3.Dot(head.localPosition, Vector3.up);
			capsuleCollider.height = Mathf.Max(capsuleCollider.radius, distanceFromFloor);

            Vector3 newPos = head.localPosition - 0.5f * distanceFromFloor * Vector3.up;
            newPos.y = 0f;

            transform.localPosition = newPos;
		}
	}
}

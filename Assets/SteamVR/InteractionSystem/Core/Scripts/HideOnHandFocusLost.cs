using UnityEngine;

namespace Valve.VR.InteractionSystem {
	public class HideOnHandFocusLost : Hoverable {
		public override void OnHandFocusLost(Hand hand) {
			gameObject.SetActive(false);
		}
	}
}

using UnityEngine;

namespace Valve.VR.InteractionSystem {
	public class SleepOnAwake : MonoBehaviour {
		void Awake() {
			Rigidbody rigidbody = GetComponent<Rigidbody>();
			if (rigidbody)
				rigidbody.Sleep();
		}
	}
}

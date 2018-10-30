using UnityEngine;

namespace Valve.VR.InteractionSystem {
	public class DontDestroyOnLoad : MonoBehaviour {
		private void Awake() {
			DontDestroyOnLoad(this);
		}
	}
}

using UnityEngine;

namespace Valve.VR.InteractionSystem {
	public class DebugUI : MonoBehaviour {
		private Player player;

		static private DebugUI _instance;
		static public DebugUI instance {
			get {
				if (_instance == null) {
					_instance = FindObjectOfType<DebugUI>();
				}
				return _instance;
			}
		}

		private void Start() {
			player = Player.Instance;
		}

		private void OnGUI() {
#if !HIDE_DEBUG_UI
			player.Draw2DDebug();
#endif
		}
	}
}

﻿using UnityEngine.Events;

namespace Valve.VR.InteractionSystem {
	public static class CustomEvents {
		[System.Serializable]
		public class UnityEventSingleFloat : UnityEvent<float> {}

		[System.Serializable]
		public class UnityEventHand : UnityEvent<Hand> {}
	}
}

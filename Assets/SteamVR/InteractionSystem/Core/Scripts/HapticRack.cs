using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem {
	[RequireComponent(typeof(Interactable))]
	public class HapticRack : Hoverable {
		[Tooltip("The linear mapping driving the haptic rack")]
		[SerializeField] private LinearMapping linearMapping;

		[Tooltip("The number of haptic pulses evenly distributed along the mapping")]
		[SerializeField] private int teethCount = 128;

		[Tooltip("Minimum duration of the haptic pulse")]
		[SerializeField] private int minimumPulseDuration = 500;

		[Tooltip("Maximum duration of the haptic pulse")]
		[SerializeField] private int maximumPulseDuration = 900;

		[Tooltip("This event is triggered every time a haptic pulse is made")]
		[SerializeField] private UnityEvent onPulse;

		private Hand hand;
		private int previousToothIndex = -1;

		void Awake() {
			if (linearMapping == null)
				linearMapping = GetComponent<LinearMapping>();
		}

		public override void OnHandHoverBegin(Hand hand) {
            base.OnHandHoverBegin(hand);

            this.hand = hand;
		}

		public override void OnHandHoverEnd(Hand hand) {
			this.hand = null;
		}

		void Update() {
			int currentToothIndex = Mathf.RoundToInt(linearMapping.value * teethCount - 0.5f);
			if (currentToothIndex != previousToothIndex) {
				Pulse();
				previousToothIndex = currentToothIndex;
			}
		}

		private void Pulse() {
			if (hand && (hand.controller != null) && (hand.GetStandardInteractionButton())) {
				ushort duration = (ushort)Random.Range(minimumPulseDuration, maximumPulseDuration + 1);
				hand.controller.TriggerHapticPulse(duration);

				onPulse.Invoke();
			}
		}
    }
}

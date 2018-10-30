using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem {
	[RequireComponent(typeof(Interactable))]
	public class InteractableButtonEvents : MonoBehaviour {
		[SerializeField] private UnityEvent onTriggerDown;
		[SerializeField] private UnityEvent onTriggerUp;
		[SerializeField] private UnityEvent onGripDown;
		[SerializeField] private UnityEvent onGripUp;
		[SerializeField] private UnityEvent onTouchpadDown;
		[SerializeField] private UnityEvent onTouchpadUp;
		[SerializeField] private UnityEvent onTouchpadTouch;
		[SerializeField] private UnityEvent onTouchpadRelease;

		private void Update() {
			for (int i = 0; i < Player.Instance.handCount; i++) {
				Hand hand = Player.Instance.GetHand(i);

				if (hand.controller != null) {
					if (hand.controller.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger))
						onTriggerDown.Invoke();

					if (hand.controller.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger))
						onTriggerUp.Invoke();

					if (hand.controller.GetPressDown(EVRButtonId.k_EButton_Grip))
						onGripDown.Invoke();

					if (hand.controller.GetPressUp(EVRButtonId.k_EButton_Grip))
						onGripUp.Invoke();

					if (hand.controller.GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad))
						onTouchpadDown.Invoke();

					if (hand.controller.GetPressUp(EVRButtonId.k_EButton_SteamVR_Touchpad))
						onTouchpadUp.Invoke();

					if (hand.controller.GetTouchDown(EVRButtonId.k_EButton_SteamVR_Touchpad))
						onTouchpadTouch.Invoke();

					if (hand.controller.GetTouchUp(EVRButtonId.k_EButton_SteamVR_Touchpad))
						onTouchpadRelease.Invoke();
				}
			}

		}
	}
}

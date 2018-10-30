using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem {
	[RequireComponent(typeof(Interactable))]
	public class InteractableHoverEvents : Hoverable {
		[SerializeField] private UnityEvent onHandHoverBegin;
		[SerializeField] private UnityEvent onHandHoverEnd;
		[SerializeField] private UnityEvent onAttachedToHand;
		[SerializeField] private UnityEvent onDetachedFromHand;

		public override void OnHandHoverBegin(Hand hand) {
			onHandHoverBegin.Invoke();
		}

		public override void OnHandHoverEnd(Hand hand) {
			onHandHoverEnd.Invoke();
		}

		public override void OnAttachedToHand(Hand hand) {
			onAttachedToHand.Invoke();
		}

		public override void OnDetachedFromHand(Hand hand) {
			onDetachedFromHand.Invoke();
		}
	}
}

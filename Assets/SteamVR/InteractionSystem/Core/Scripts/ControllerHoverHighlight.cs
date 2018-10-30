using UnityEngine;

namespace Valve.VR.InteractionSystem {
	public class ControllerHoverHighlight : Hoverable {
		[SerializeField] private Material highLightMaterial;
		[SerializeField] private bool fireHapticsOnHightlight = true;
		[SerializeField] private ushort hapticPulseAmount;

		private Hand hand;

		private MeshRenderer bodyMeshRenderer;
		private MeshRenderer trackingHatMeshRenderer;
		private SteamVR_RenderModel renderModel;
		private bool renderModelLoaded = false;

		SteamVR_Events.Action renderModelLoadedAction;

		private void Start() {
			hand = GetComponentInParent<Hand>();
		}

		private void Awake() {
			renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction(OnRenderModelLoaded);
		}

		private void OnEnable() {
			renderModelLoadedAction.enabled = true;
		}

		private void OnDisable() {
			renderModelLoadedAction.enabled = false;
		}

		public override void OnHandInitialized(int deviceIndex) {
			renderModel = gameObject.AddComponent<SteamVR_RenderModel>();
			renderModel.SetDeviceIndex(deviceIndex);
			renderModel.updateDynamically = false;
		}

		private void OnRenderModelLoaded(SteamVR_RenderModel renderModel, bool success) {
			if (renderModel != this.renderModel)
				return;

			Transform bodyTransform = transform.Find("body");
			if (bodyTransform != null) {
				bodyTransform.gameObject.layer = gameObject.layer;
				bodyTransform.gameObject.tag = gameObject.tag;
				bodyMeshRenderer = bodyTransform.GetComponent<MeshRenderer>();
				bodyMeshRenderer.material = highLightMaterial;
				bodyMeshRenderer.enabled = false;
			}

			Transform trackingHatTransform = transform.Find("trackhat");
			if (trackingHatTransform != null) {
				trackingHatTransform.gameObject.layer = gameObject.layer;
				trackingHatTransform.gameObject.tag = gameObject.tag;
				trackingHatMeshRenderer = trackingHatTransform.GetComponent<MeshRenderer>();
				trackingHatMeshRenderer.material = highLightMaterial;
				trackingHatMeshRenderer.enabled = false;
			}

			foreach (Transform child in transform) {
				if ((child.name != "body") && (child.name != "trackhat"))
					Destroy(child.gameObject);
			}

			renderModelLoaded = true;
		}

		public void OnParentHandHoverBegin(Interactable other) {
			if (isActiveAndEnabled == false)
				return;

			if (other.transform.parent != transform.parent)
				ShowHighlight();
		}

		public void OnParentHandHoverEnd(Interactable other) {
			HideHighlight();
		}

		public override void OnParentHandInputFocusAcquired() {
			if (isActiveAndEnabled == false)
				return;

			if (hand.hoveringInteractable && hand.hoveringInteractable.transform.parent != transform.parent)
				ShowHighlight();
		}

		public override void OnParentHandInputFocusLost() {
			HideHighlight();
		}

		public void ShowHighlight() {
			if (renderModelLoaded == false)
				return;

			if (fireHapticsOnHightlight)
				hand.controller.TriggerHapticPulse(hapticPulseAmount);

			if (bodyMeshRenderer != null)
				bodyMeshRenderer.enabled = true;

			if (trackingHatMeshRenderer != null)
				trackingHatMeshRenderer.enabled = true;
		}
		
		public void HideHighlight() {
			if (renderModelLoaded == false)
				return;

			if (fireHapticsOnHightlight)
				hand.controller.TriggerHapticPulse(hapticPulseAmount);

			if (bodyMeshRenderer != null)
				bodyMeshRenderer.enabled = false;

			if (trackingHatMeshRenderer != null)
				trackingHatMeshRenderer.enabled = false;
		}
	}
}

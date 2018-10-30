using UnityEngine;

namespace Valve.VR.InteractionSystem {
	public class LinearAudioPitch : MonoBehaviour {
		[SerializeField] private LinearMapping linearMapping;
		[SerializeField] private AnimationCurve pitchCurve;
		[SerializeField] private float minPitch;
		[SerializeField] private float maxPitch;
		[SerializeField] private bool applyContinuously = true;

		private AudioSource audioSource;

		private void Awake() {
			if (audioSource == null)
				audioSource = GetComponent<AudioSource>();

			if (linearMapping == null)
				linearMapping = GetComponent<LinearMapping>();
		}

		private void Update() {
			if (applyContinuously)
				Apply();
		}

		private void Apply() {
			float y = pitchCurve.Evaluate(linearMapping.value);
			audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, y);
		}
	}
}

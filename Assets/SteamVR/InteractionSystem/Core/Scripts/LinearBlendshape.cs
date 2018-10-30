﻿using UnityEngine;

namespace Valve.VR.InteractionSystem {
	public class LinearBlendshape : MonoBehaviour {
		[SerializeField] private LinearMapping linearMapping;
		[SerializeField] private SkinnedMeshRenderer skinnedMesh;

		private float lastValue;

		private void Awake() {
			if (skinnedMesh == null)
				skinnedMesh = GetComponent<SkinnedMeshRenderer>();

			if (linearMapping == null)
				linearMapping = GetComponent<LinearMapping>();
		}

		private void Update() {
			float value = linearMapping.value;

			//No need to set the blend if our value hasn't changed.
			if (value != lastValue) {
				float blendValue = Util.RemapNumberClamped(value, 0f, 1f, 1f, 100f);
				skinnedMesh.SetBlendShapeWeight(0, blendValue);
			}

			lastValue = value;
		}
	}
}

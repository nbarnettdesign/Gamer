using UnityEngine;

namespace Valve.VR.InteractionSystem {
	public class SpawnAndAttachToHand : MonoBehaviour {
		[SerializeField] private Hand hand;
		[SerializeField] private GameObject prefab;

		public Hand Hand { get { return hand; } }
		public GameObject Prefab { get { return prefab; } }

		public void SpawnAndAttach(Hand passedInhand) {
			Hand handToUse = passedInhand;
			if (passedInhand == null)
				handToUse = hand;

			if (handToUse == null)
				return;

			GameObject prefabObject = Instantiate(prefab);
			handToUse.AttachObject(prefabObject);
		}
	}
}

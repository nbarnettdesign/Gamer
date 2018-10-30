using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem {
	[RequireComponent(typeof(Interactable))]
	public class Longbow : Hoverable {
		public enum Handedness { Left, Right };

		[SerializeField] private Handedness currentHandGuess = Handedness.Left;
		[SerializeField] private Transform pivotTransform;
		[SerializeField] private Transform handleTransform;

		[SerializeField] private Transform nockTransform;
		[SerializeField] private Transform nockRestTransform;

		[SerializeField] private ItemPackage arrowHandItemPackage;
		[SerializeField] private GameObject arrowHandPrefab;

		[SerializeField] private bool nocked;
		[SerializeField] private bool pulled;

		[SerializeField] private float arrowMinVelocity = 3f;
		[SerializeField] private float arrowMaxVelocity = 30f;

		[SerializeField] private float drawOffset = 0.06f;

		[SerializeField] private LinearMapping bowDrawLinearMapping;
		[SerializeField] private SoundBowClick drawSound;

		[SerializeField] private SoundPlayOneshot arrowSlideSound;
		[SerializeField] private SoundPlayOneshot releaseSound;
		[SerializeField] private SoundPlayOneshot nockSound;

		public Transform NockTransform { get { return nockTransform; } }
		public Transform NockRestTransform { get { return nockRestTransform; } }
        public bool Nocked { get { return nocked; } }
        public bool Pulled { get { return pulled; } }
        public Hand Hand { get { return hand; } }

		private float timeOfPossibleHandSwitch = 0f;
		private readonly float timeBeforeConfirmingHandSwitch = 1.5f;
		private bool possibleHandSwitch = false;

		private Hand hand;
		private ArrowHand arrowHand;

		private const float minPull = 0.05f;
		private const float maxPull = 0.5f;
		private float nockDistanceTravelled = 0f;
		private readonly float hapticDistanceThreshold = 0.01f;
		private float lastTickDistance;
		private const float bowPullPulseStrengthLow = 100;
		private const float bowPullPulseStrengthHigh = 500;
		private Vector3 bowLeftVector;

		private float arrowVelocity = 30f;

		private float minStrainTickTime = 0.1f;
		private float maxStrainTickTime = 0.5f;
		private float nextStrainTick = 0;

		private bool lerpBackToZeroRotation;
		private float lerpStartTime;
		private float lerpDuration = 0.15f;
		private Quaternion lerpStartRotation;

		private float nockLerpStartTime;
		private Quaternion nockLerpStartRotation;

		private bool deferNewPoses = false;
		private Vector3 lateUpdatePos;
		private Quaternion lateUpdateRot;

		private float drawTension;

        private HandBowAttachment handAttachment;

		SteamVR_Events.Action newPosesAppliedAction;

		public override void OnAttachedToHand(Hand attachedHand) {
            hand = attachedHand;
            hand.GetComponent<HandModel>().BowAttached();


            currentHandGuess = hand.startingHandType == Hand.HandType.Left ? Handedness.Left : Handedness.Right;

            DoHandednessCheck();
        }

        public override void OnDetachedFromHand(Hand hand)
        {
            gameObject.SetActive(false);
            hand.GetComponent<HandModel>().BowDetached();
        }

        void Awake() {
            newPosesAppliedAction = SteamVR_Events.NewPosesAppliedAction(OnNewPosesApplied);
        }

		void OnEnable() {
			newPosesAppliedAction.enabled = true;
		}

		void OnDisable() {
			newPosesAppliedAction.enabled = false;
		}

		void LateUpdate() {
			if (deferNewPoses) {
				lateUpdatePos = transform.position;
				lateUpdateRot = transform.rotation;
			}
		}

		private void OnNewPosesApplied() {
			if (deferNewPoses) {
				// Set longbow object back to previous pose position to avoid jitter
				transform.position = lateUpdatePos;
				transform.rotation = lateUpdateRot;

				deferNewPoses = false;
			}
		}

		public override void HandAttachedUpdate(Hand hand) {
			// Reset transform since we cheated it right after getting poses on previous frame
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;

            Handedness currType = currentHandGuess;

			// Update handedness guess
			EvaluateHandedness();

            if (currType != currentHandGuess)
                DoHandednessCheck();

			if (nocked) {
				deferNewPoses = true;

				Vector3 nockToarrowHand = (arrowHand.ArrowNockTransform.parent.position - nockRestTransform.position); // Vector from bow nock transform to arrowhand nock transform - used to align bow when drawing

				// Align bow
				// Time lerp value used for ramping into drawn bow orientation
				float lerp = Util.RemapNumberClamped(Time.time, nockLerpStartTime, (nockLerpStartTime + lerpDuration), 0f, 1f);

				float pullLerp = Util.RemapNumberClamped(nockToarrowHand.magnitude, minPull, maxPull, 0f, 1f); // Normalized current state of bow draw 0 - 1

				Vector3 arrowNockTransformToHeadset = ((Player.Instance.hmdTransform.position + (Vector3.down * 0.05f)) - arrowHand.ArrowNockTransform.parent.position).normalized;
				Vector3 arrowHandPosition = (arrowHand.ArrowNockTransform.parent.position + ((arrowNockTransformToHeadset * drawOffset) * pullLerp)); // Use this line to lerp arrowHand nock position
																																					  //Vector3 arrowHandPosition = arrowHand.arrowNockTransform.position; // Use this line if we don't want to lerp arrowHand nock position

				Vector3 pivotToString = (arrowHandPosition - pivotTransform.position).normalized;
				Vector3 pivotToLowerHandle = (handleTransform.position - pivotTransform.position).normalized;
				bowLeftVector = -Vector3.Cross(pivotToLowerHandle, pivotToString);
				pivotTransform.rotation = Quaternion.Lerp(nockLerpStartRotation, Quaternion.LookRotation(pivotToString, bowLeftVector), lerp);

				// Move nock position
				if (Vector3.Dot(nockToarrowHand, -nockTransform.forward) > 0) {
					float distanceToarrowHand = nockToarrowHand.magnitude * lerp;

					nockTransform.localPosition = new Vector3(0f, 0f, Mathf.Clamp(-distanceToarrowHand, -maxPull, 0f));

					nockDistanceTravelled = -nockTransform.localPosition.z;

					arrowVelocity = Util.RemapNumber(nockDistanceTravelled, minPull, maxPull, arrowMinVelocity, arrowMaxVelocity);

					drawTension = Util.RemapNumberClamped(nockDistanceTravelled, 0, maxPull, 0f, 1f);

					bowDrawLinearMapping.value = drawTension; // Send drawTension value to LinearMapping script, which drives the bow draw animation

					if (nockDistanceTravelled > minPull)
						pulled = true;
					else
						pulled = false;

					if ((nockDistanceTravelled > (lastTickDistance + hapticDistanceThreshold)) || nockDistanceTravelled < (lastTickDistance - hapticDistanceThreshold)) {
						ushort hapticStrength = (ushort)Util.RemapNumber(nockDistanceTravelled, 0, maxPull, bowPullPulseStrengthLow, bowPullPulseStrengthHigh);
                        hand.controller.TriggerHapticPulse(hapticStrength);
						hand.otherHand.controller.TriggerHapticPulse(hapticStrength);

						drawSound.PlayBowTensionClicks(drawTension);

						lastTickDistance = nockDistanceTravelled;
					}

					if (nockDistanceTravelled >= maxPull) {
						if (Time.time > nextStrainTick) {
							hand.controller.TriggerHapticPulse(700);
							hand.otherHand.controller.TriggerHapticPulse(700);

							drawSound.PlayBowTensionClicks(drawTension);

							nextStrainTick = Time.time + Random.Range(minStrainTickTime, maxStrainTickTime);
						}
					}
				} else {
					nockTransform.localPosition = new Vector3(0f, 0f, 0f);
					bowDrawLinearMapping.value = 0f;
				}
			} else {
				if (lerpBackToZeroRotation) {
					float lerp = Util.RemapNumber(Time.time, lerpStartTime, lerpStartTime + lerpDuration, 0, 1);

					pivotTransform.localRotation = Quaternion.Lerp(lerpStartRotation, Quaternion.identity, lerp);

					if (lerp >= 1)
						lerpBackToZeroRotation = false;
				}
			}
		}

		public void ArrowReleased() {
			nocked = false;
			hand.HoverUnlock(GetComponent<Interactable>());

            if(arrowHand)
			    hand.otherHand.HoverUnlock(arrowHand.GetComponent<Interactable>());

			if (releaseSound != null)
				releaseSound.Play();

			StartCoroutine(ResetDrawAnim());
		}

		private IEnumerator ResetDrawAnim() {
			float startTime = Time.time;
			float startLerp = drawTension;

			while (Time.time < (startTime + 0.02f)) {
				float lerp = Util.RemapNumberClamped(Time.time, startTime, startTime + 0.02f, startLerp, 0f);
                bowDrawLinearMapping.value = lerp;
				yield return null;
			}
			bowDrawLinearMapping.value = 0;
			yield break;
		}

		public float GetArrowVelocity() {
			return arrowVelocity;
		}

		public void StartRotationLerp() {
			lerpStartTime = Time.time;
			lerpBackToZeroRotation = true;
			lerpStartRotation = pivotTransform.localRotation;

			Util.ResetTransform(nockTransform);
		}

		public void StartNock(ArrowHand currentArrowHand) {
			arrowHand = currentArrowHand;
			hand.HoverLock(GetComponent<Interactable>());
			nocked = true;
			nockLerpStartTime = Time.time;
			nockLerpStartRotation = pivotTransform.rotation;

			// Sound of arrow sliding on nock as it's being pulled back
			arrowSlideSound.Play();

			// Decide which hand we're drawing with and lerp to the correct side
			DoHandednessCheck();
		}


		//-------------------------------------------------
		private void EvaluateHandedness() {
			Hand.HandType handType = hand.startingHandType;

			if (handType == Hand.HandType.Left)// Bow hand is further left than arrow hand.
			{
				// We were considering a switch, but the current controller orientation matches our currently assigned handedness, so no longer consider a switch
				if (possibleHandSwitch && currentHandGuess == Handedness.Left)
					possibleHandSwitch = false;

				// If we previously thought the bow was right-handed, and were not already considering switching, start considering a switch
				if (!possibleHandSwitch && currentHandGuess == Handedness.Right) {
					possibleHandSwitch = true;
					timeOfPossibleHandSwitch = Time.time;
				}

				// If we are considering a handedness switch, and it's been this way long enough, switch
				if (possibleHandSwitch && Time.time > (timeOfPossibleHandSwitch + timeBeforeConfirmingHandSwitch)) {
					currentHandGuess = Handedness.Left;
					possibleHandSwitch = false;
				}
			} else // Bow hand is further right than arrow hand
			  {
				// We were considering a switch, but the current controller orientation matches our currently assigned handedness, so no longer consider a switch
				if (possibleHandSwitch && currentHandGuess == Handedness.Right)
					possibleHandSwitch = false;

				// If we previously thought the bow was right-handed, and were not already considering switching, start considering a switch
				if (!possibleHandSwitch && currentHandGuess == Handedness.Left) {
					possibleHandSwitch = true;
					timeOfPossibleHandSwitch = Time.time;
				}

				// If we are considering a handedness switch, and it's been this way long enough, switch
				if (possibleHandSwitch && Time.time > (timeOfPossibleHandSwitch + timeBeforeConfirmingHandSwitch)) {
					currentHandGuess = Handedness.Right;
					possibleHandSwitch = false;
				}
			}
		}
		
		private void DoHandednessCheck() {
			// Based on our current best guess about hand, switch bow orientation and arrow lerp direction
			if (currentHandGuess == Handedness.Left)
				pivotTransform.localScale = new Vector3(1f, 1f, 1f);
			else
				pivotTransform.localScale = new Vector3(1f, -1f, 1f);
		}

		public void ArrowInPosition() {
			DoHandednessCheck();

			if (nockSound != null)
				nockSound.Play();
		}

		public void ReleaseNock() {
			// ArrowHand tells us to do this when we release the buttons when bow is nocked but not drawn far enough
			nocked = false;
			hand.HoverUnlock(GetComponent<Interactable>());
			StartCoroutine(ResetDrawAnim());
		}

		private void ShutDown() {
			if (hand != null && hand.otherHand.currentAttachedObject != null) {
				if (hand.otherHand.currentAttachedObject.GetComponent<ItemPackageReference>() != null) {
					if (hand.otherHand.currentAttachedObject.GetComponent<ItemPackageReference>().itemPackage == arrowHandItemPackage) {
						hand.otherHand.DetachObject(hand.otherHand.currentAttachedObject);
					}
				}
			}
		}

		public override void OnHandFocusLost(Hand hand) {
			gameObject.SetActive(false);
		}

        public override void OnHandFocusAcquired(Hand hand) {
			gameObject.SetActive(true);
			OnAttachedToHand(hand);
		}

		void OnDestroy() {
			ShutDown();
		}
	}
}

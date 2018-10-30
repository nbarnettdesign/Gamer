using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Valve.VR.InteractionSystem {
	[RequireComponent(typeof(Interactable))]
	public class ItemPackageSpawner : Hoverable {
		public ItemPackage ItemPackage { get { return _itemPackage; } set { CreatePreviewObject(); } }

		public ItemPackage _itemPackage;

		private bool useItemPackagePreview = true;
		private bool useFadedPreview = false;
		private GameObject previewObject;

		public bool requireTriggerPressToTake = false;
		public bool requireTriggerPressToReturn = false;
		public bool showTriggerHint = false;

		[EnumFlags]
		public Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags;
		public string attachmentPoint;

		public bool takeBackItem = false; // if a hand enters this trigger and has the item this spawner dispenses at the top of the stack, remove it from the stack

		public bool acceptDifferentItems = false;

		private GameObject spawnedItem;
		private bool itemIsSpawned = false;

		public UnityEvent pickupEvent;
		public UnityEvent dropEvent;

		public bool justPickedUpItem = false;

		private void CreatePreviewObject() {
			if (!useItemPackagePreview)
				return;

			ClearPreview();

			if (useItemPackagePreview) {
				if (ItemPackage == null)
					return;

				if (useFadedPreview == false) { // if we don't have a spawned item out there, use the regular preview
					if (ItemPackage.PreviewPrefab != null) {
						previewObject = Instantiate(ItemPackage.PreviewPrefab, transform.position, Quaternion.identity) as GameObject;
						previewObject.transform.parent = transform;
						previewObject.transform.localRotation = Quaternion.identity;
					}
				} else { // there's a spawned item out there. Use the faded preview
					if (ItemPackage.FadedPreviewPrefab != null) {
						previewObject = Instantiate(ItemPackage.FadedPreviewPrefab, transform.position, Quaternion.identity) as GameObject;
						previewObject.transform.parent = transform;
						previewObject.transform.localRotation = Quaternion.identity;
					}
				}
			}
		}

		private void Start() {
			VerifyItemPackage();
		}

		private void VerifyItemPackage() {
			if (ItemPackage == null)
				ItemPackageNotValid();

			if (ItemPackage.ItemPrefab == null)
				ItemPackageNotValid();
		}

		private void ItemPackageNotValid() {
			Debug.LogError("ItemPackage assigned to " + gameObject.name + " is not valid. Destroying this game object.");
			Destroy(gameObject);
		}

		private void ClearPreview() {
			foreach (Transform child in transform) {
				if (Time.time > 0)
					Destroy(child.gameObject);
				else
					DestroyImmediate(child.gameObject);
			}
		}

		private void Update() {
			if ((itemIsSpawned == true) && (spawnedItem == null)) {
				itemIsSpawned = false;
				useFadedPreview = false;
				dropEvent.Invoke();
				CreatePreviewObject();
			}
		}

		public override void OnHandHoverBegin(Hand hand) {
            base.OnHandHoverBegin(hand);

			ItemPackage currentAttachedItemPackage = GetAttachedItemPackage(hand);

			if (currentAttachedItemPackage == ItemPackage) // the item at the top of the hand's stack has an associated ItemPackage
				if (takeBackItem && !requireTriggerPressToReturn) // if we want to take back matching items and aren't waiting for a trigger press
					TakeBackItem(hand);

			if (!requireTriggerPressToTake) // we don't require trigger press for pickup. Spawn and attach object.
				SpawnAndAttachObject(hand);

			if (requireTriggerPressToTake && showTriggerHint)
				ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger, "PickUp");
		}

		private void TakeBackItem(Hand hand) {
			RemoveMatchingItemsFromHandStack(ItemPackage, hand);

			if (ItemPackage.PackageType == ItemPackage.ItemPackageType.TwoHanded)
				RemoveMatchingItemsFromHandStack(ItemPackage, hand.otherHand);
		}

		private ItemPackage GetAttachedItemPackage(Hand hand) {
			GameObject currentAttachedObject = hand.currentAttachedObject;

			if (currentAttachedObject == null) // verify the hand is holding something
				return null;

			ItemPackageReference packageReference = hand.currentAttachedObject.GetComponent<ItemPackageReference>();
			if (packageReference == null) // verify the item in the hand is matchable
				return null;

			ItemPackage attachedItemPackage = packageReference.itemPackage; // return the ItemPackage reference we find.

			return attachedItemPackage;
		}

		public override void HandHoverUpdate(Hand hand) {
			if (takeBackItem && requireTriggerPressToReturn) {
				if (hand.controller != null && hand.controller.GetHairTriggerDown()) {
					ItemPackage currentAttachedItemPackage = GetAttachedItemPackage(hand);
					if (currentAttachedItemPackage == ItemPackage) {
						TakeBackItem(hand);
						return; // So that we don't pick up an ItemPackage the same frame that we return it
					}
				}
			}

			if (requireTriggerPressToTake)
				if (hand.controller != null && hand.controller.GetHairTriggerDown())
					SpawnAndAttachObject(hand);
		}

		public override void OnHandHoverEnd(Hand hand) {
			if (!justPickedUpItem && requireTriggerPressToTake && showTriggerHint)
				ControllerButtonHints.HideTextHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);

			justPickedUpItem = false;
		}

		private void RemoveMatchingItemsFromHandStack(ItemPackage package, Hand hand) {
			for (int i = 0; i < hand.AttachedObjects.Count; i++) {
				ItemPackageReference packageReference = hand.AttachedObjects[i].attachedObject.GetComponent<ItemPackageReference>();
				if (packageReference != null) {
					ItemPackage attachedObjectItemPackage = packageReference.itemPackage;
					if ((attachedObjectItemPackage != null) && (attachedObjectItemPackage == package)) {
						GameObject detachedItem = hand.AttachedObjects[i].attachedObject;
						hand.DetachObject(detachedItem);
					}
				}
			}
		}

		private void RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType packageType, Hand hand) {
			for (int i = 0; i < hand.AttachedObjects.Count; i++) {
				ItemPackageReference packageReference = hand.AttachedObjects[i].attachedObject.GetComponent<ItemPackageReference>();
				if (packageReference != null) {
					if (packageReference.itemPackage.PackageType == packageType) {
						GameObject detachedItem = hand.AttachedObjects[i].attachedObject;
						hand.DetachObject(detachedItem);
					}
				}
			}
		}

		private void SpawnAndAttachObject(Hand hand) {
			if (hand.otherHand != null) {
				//If the other hand has this item package, take it back from the other hand
				ItemPackage otherHandItemPackage = GetAttachedItemPackage(hand.otherHand);
				if (otherHandItemPackage == ItemPackage)
					TakeBackItem(hand.otherHand);
			}

			if (showTriggerHint)
				ControllerButtonHints.HideTextHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);

			if (ItemPackage.OtherHandItemPrefab != null)
				if (hand.otherHand.hoverLocked)
					//Debug.Log( "Not attaching objects because other hand is hoverlocked and we can't deliver both items." );
					return;

			// if we're trying to spawn a one-handed item, remove one and two-handed items from this hand and two-handed items from both hands
			if (ItemPackage.PackageType == ItemPackage.ItemPackageType.OneHanded) {
				RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.OneHanded, hand);
				RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand);
				RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand.otherHand);
			}

			// if we're trying to spawn a two-handed item, remove one and two-handed items from both hands
			if (ItemPackage.PackageType == ItemPackage.ItemPackageType.TwoHanded) {
				RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.OneHanded, hand);
				RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.OneHanded, hand.otherHand);
				RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand);
				RemoveMatchingItemTypesFromHand(ItemPackage.ItemPackageType.TwoHanded, hand.otherHand);
			}

			spawnedItem = Instantiate(ItemPackage.ItemPrefab);
			spawnedItem.SetActive(true);
			hand.AttachObject(spawnedItem, attachmentFlags, attachmentPoint);

			if ((ItemPackage.OtherHandItemPrefab != null) && (hand.otherHand.controller != null)) {
				GameObject otherHandObjectToAttach = Instantiate(ItemPackage.OtherHandItemPrefab);
				otherHandObjectToAttach.SetActive(true);
				hand.otherHand.AttachObject(otherHandObjectToAttach, attachmentFlags);
			}

			itemIsSpawned = true;

			justPickedUpItem = true;

			if (takeBackItem) {
				useFadedPreview = true;
				pickupEvent.Invoke();
				CreatePreviewObject();
			}
		}
	}
}

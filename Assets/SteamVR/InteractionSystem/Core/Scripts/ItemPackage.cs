using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class ItemPackage : MonoBehaviour
    {
        public enum ItemPackageType { Unrestricted, OneHanded, TwoHanded }

        [SerializeField] private new string name;
        [SerializeField] private ItemPackageType packageType = ItemPackageType.Unrestricted;
        [SerializeField] private GameObject itemPrefab; // object to be spawned on tracked controller
        [SerializeField] private GameObject otherHandItemPrefab; // object to be spawned in Other Hand
        [SerializeField] private GameObject previewPrefab; // used to preview inputObject
        [SerializeField] private GameObject fadedPreviewPrefab; // used to preview insubstantial inputObject

        public ItemPackageType PackageType { get { return packageType; } }
        public GameObject ItemPrefab {
            get {
                OnItemPickup();
                return itemPrefab;
            }
        }
        public GameObject OtherHandItemPrefab { get { return otherHandItemPrefab; } }
        public GameObject PreviewPrefab { get { return previewPrefab; } }
        public GameObject FadedPreviewPrefab { get { return fadedPreviewPrefab; } }

        protected virtual void OnItemPickup() { }
    }
}

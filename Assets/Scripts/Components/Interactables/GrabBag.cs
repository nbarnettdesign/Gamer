using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(InteractableHoverEvents))]
    [RequireComponent(typeof(BoxCollider))]
    public class GrabBag : Hoverable
    {
        [EnumFlags]
        [Tooltip("The flags used to attach this object to the hand.")]
        public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand;

        [Tooltip("Name of the attachment transform under in the hand's hierarchy which the object should should snap to.")]
        public string attachmentPoint;

        [SerializeField] private GameObject bagContents;
        [SerializeField] private float coolDownTime;
        [SerializeField] private int contentsQuantity;

        public int CurrentQuantity { get { return currentQuantity; } }

        private bool infinite;
        private int currentQuantity;
        private bool canGrab;

        private void Awake()
        {
            canGrab = true;

            if (contentsQuantity <= 0)
            {
                infinite = true;
            }
            else
            {
                currentQuantity = contentsQuantity;
            }
        }

        public override void OnHandHoverBegin(Hand hand)
        {
            base.OnHandHoverBegin(hand);

            if (hand.GetStandardInteractionButtonDown())
            {
                AttachObject(hand);
            }
        }

        public override void HandHoverUpdate(Hand hand)
        {
            if (hand.GetStandardInteractionButtonDown())
            {
                AttachObject(hand);
            }
        }

        private void AttachObject(Hand hand)
        {
            if (canGrab == false || infinite == false && currentQuantity <= 0) return;

            GameObject obj = SimplePool.Spawn(bagContents, hand.transform.position, Quaternion.identity);
            hand.AttachObject(obj, attachmentFlags, attachmentPoint);

            if (coolDownTime <= 0) return;

            if (infinite == false)
            {
                currentQuantity--;
            }

            canGrab = false;
            this.Invoke(ResetGrab, coolDownTime);
        }

        private void ResetGrab()
        {
            canGrab = true;
        }
    }
}

using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(InteractableHoverEvents))]
public class GrabbableButton : Hoverable
{
    [EnumFlags]
    [Tooltip("The flags used to attach this object to the hand.")]
    public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand;

    [Tooltip("Name of the attachment transform under in the hand's hierarchy which the object should should snap to.")]
    public string attachmentPoint;

    [SerializeField] private UnityEvent onGrabbed;
    [SerializeField] private float grabTime;

    private float grabTimeCount;
    private bool hasGrabbed;

    public override void OnHandHoverBegin(Hand hand)
    {
        base.OnHandHoverBegin(hand);
        if (hand.GetStandardInteractionButton())
        {
            UpdateGrabbed();
        }
    }

    public override void HandHoverUpdate(Hand hand)
    {
        if (hand.GetStandardInteractionButton())
        {
            UpdateGrabbed();
        }
    }

    public override void OnHandHoverEnd(Hand hand)
    {
        base.OnHandHoverEnd(hand);

        hasGrabbed = false;
        grabTimeCount = 0f;
    }

    private void UpdateGrabbed()
    {
        if (hasGrabbed) return;

        grabTimeCount += Time.deltaTime;

        if (grabTimeCount >= grabTime)
        {
            hasGrabbed = true;
            onGrabbed.Invoke();
        }
    }
}

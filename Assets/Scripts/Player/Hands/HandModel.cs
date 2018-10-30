using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Hand))]
public class HandModel : MonoBehaviour
{
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

    public HandState CurrentHandState { get { return handState != null ? handState.CurrentState : HandState.Idle; } }

    private Hand hand;
    private HandModelState handState;
    private HandModel otherHandModel;
    private GameObject modelObject;

    private void Start()
    {
        hand = GetComponent<Hand>();
        otherHandModel = hand.otherHand.GetComponent<HandModel>();

        //Spawn the hand model
        modelObject = SimplePool.Spawn(hand.startingHandType == Hand.HandType.Left ? leftHand : rightHand,
            transform.position, transform.rotation);
        modelObject.transform.SetParent(transform);

        handState = modelObject.GetComponent<HandModelState>();
    }

    private void LateUpdate()
    {
        CheckInput();
    }

    public void BowAttached()
    {
        modelObject.SetActive(false);
        otherHandModel.EnsureActive();
    }

    public void BowDetached()
    {
        EnsureActive();
        otherHandModel.EnsureActive();
    }

    public void EnsureActive()
    {
        if (modelObject == null || modelObject.activeInHierarchy)
            return;

        modelObject.SetActive(true);
    }

    public void ArrowAttached()
    {
        if (handState == null)
            return;

        handState.UpdateState(HandState.ArrowHeld);
    }

    public void HandIdle()
    {
        if (handState == null)
            return;

        handState.UpdateState(HandState.Idle);
    }

    public void HandClosed()
    {
        if (handState == null)
            return;

        handState.UpdateState(HandState.Closed);
    }

    public void HandHover()
    {
        if (handState == null)
            return;

        handState.UpdateState(HandState.Open);
    }

    public void AttachedToBowstring()
    {
        if (handState == null)
            return;

        handState.UpdateState(HandState.DrawBack);
    }

    public void HandGrab()
    {
        if (handState == null)
            return;

        handState.UpdateState(HandState.Grab);
    }

    private void CheckInput()
    {
        if (hand == null || hand.controller == null)
            return;

        if (hand.currentAttachedObject && hand.currentAttachedObject.GetComponent<ArrowHand>()
                && hand.currentAttachedObject.GetComponent<ArrowHand>().HasArrow)
        {
            if (hand.currentAttachedObject.GetComponent<ArrowHand>().Nocked)
                AttachedToBowstring();

            return;
        }

        if (hand.GetStandardInteractionButtonDown())
            HandClosed();
        else if (hand.GetStandardInteractionButtonUp())
            HandIdle();
    }
}

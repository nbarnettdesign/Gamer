using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class BrokenObjectChild : Hoverable {
    private BrokenObject brokenObject;

    private void Start()
    {
        brokenObject = GetComponentInParent<BrokenObject>();
    }

    public override void OnAttachedToHand(Hand hand)
    {
        brokenObject.ChildAttachedToHand();
    }

    public override void OnDetachedFromHand(Hand hand)
    {
        brokenObject.ChildDetachedFromHand();
    }
}

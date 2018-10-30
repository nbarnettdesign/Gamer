using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

[RequireComponent(typeof(Interactable))]
public class Pressable : Hoverable
{
    [SerializeField] private bool onOff;
    [SerializeField] private UnityEvent onPressUp;
    [SerializeField] private UnityEvent onPressDown;

    [Header("Feedback")]
    [SerializeField] private ushort hapticPulseIntensity;

    private bool hasPressed;

    public override void OnHandHoverBegin(Hand hand)
    {
        base.OnHandHoverBegin(hand);

        TogglePressed();
        hand.controller.TriggerHapticPulse(hapticPulseIntensity);
    }

    private void TogglePressed()
    {
        if (hasPressed && onOff == false) return;

        hasPressed = !hasPressed;

        if (hasPressed)
        {
            onPressDown.Invoke();
        }
        else
        {
            onPressUp.Invoke();
        }
    }
}

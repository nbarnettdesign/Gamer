using UnityEngine;
using Valve.VR.InteractionSystem;

public abstract class Hoverable : MonoBehaviour
{
    public virtual void OnHandHoverBegin(Hand hand) { }
    public virtual void OnHandHoverEnd(Hand hand) { }
    public virtual void HandHoverUpdate(Hand hand) { }
    public virtual void OnAttachedToHand(Hand hand) { }
    public virtual void OnDetachedFromHand(Hand hand) { }
    public virtual void HandAttachedUpdate(Hand hand) { }
    public virtual void OnHandFocusAcquired(Hand hand) { }
    public virtual void OnHandFocusLost(Hand hand) { }
    public virtual void OnHandInitialized(int index) { }
    public virtual void OnParentHandInputFocusAcquired() { }
    public virtual void OnParentHandInputFocusLost() { }

    public virtual void OnBowHover() { }

    public virtual void OnBowHoverExit() { }

    protected bool disabled;

    public void Disable()
    {
        disabled = true;
    }

    public void Enable()
    {
        disabled = false;
    }
}

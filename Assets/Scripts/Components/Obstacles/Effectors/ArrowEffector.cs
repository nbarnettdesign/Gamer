using UnityEngine;
using Valve.VR.InteractionSystem;

public class ArrowEffector : Effector
{
    [Tooltip("Force Kinematic on arrow entry. May be overridden by certain effectors")]
    [SerializeField] private bool forceKinematic;

    private void OnTriggerEnter(Collider other)
    {
        Arrow arrow = other.GetComponent<Arrow>();

        //Make sure we have an arrow and aren't going through walls
        if (arrow == null || Physics.Linecast(transform.position, arrow.transform.position, environmentLayer)) return;

        EffectArrow(arrow);
    }

    protected virtual void EffectArrow(Arrow arrow)
    {
        arrow.ArrowHeadRB.velocity = Vector3.zero;
        arrow.ShaftRB.velocity = Vector3.zero;

        if (forceKinematic)
        {
            arrow.ShaftRB.isKinematic = true;
            arrow.ArrowHeadRB.isKinematic = true;
        }
    }
}

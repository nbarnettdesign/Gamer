using UnityEngine;
using Valve.VR.InteractionSystem;


[RequireComponent(typeof(Collider))]
public class DeflectArrow : MonoBehaviour
{
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private float deflectForce = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        Arrow arrow = other.GetComponent<Arrow>();

        //Make sure we have an arrow and aren't going through walls
        if (arrow == null || Physics.Linecast(transform.position, arrow.transform.position, environmentLayer)) return;

        Vector3 dir = other.transform.position;

        EffectArrow(arrow, dir);
    }

    protected virtual void EffectArrow(Arrow arrow, Vector3 dir)
    {
        //Zero velocity
        arrow.ArrowHeadRB.velocity = Vector3.zero;
        arrow.ShaftRB.velocity = Vector3.zero;

        //Deflect
        //arrow.ArrowHeadRB.AddForce(-dir * deflectForce);
        //arrow.ShaftRB.AddForce(-dir * deflectForce);

        arrow.ArrowHeadRB.velocity = -dir * deflectForce;
        arrow.ShaftRB.velocity = -dir * deflectForce;
    }
}

using UnityEngine;
using Valve.VR.InteractionSystem;

public class BowImpact : MonoBehaviour
{
    [SerializeField] private float velocityModifier;
    [SerializeField] private LayerMask impactLayers;
    [SerializeField] private float teleportCooldown;

    private VelocityEstimator velocityEstimator;
    private Longbow bow;

    private bool recentlyTeleported;
    private float tpCount;

    private Collider myCollider;

    private void Start()
    {
        velocityEstimator = GetComponent<VelocityEstimator>();
        bow = GetComponentInParent<Longbow>();
        myCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        CheckTeleported();
    }

    private void CheckTeleported()
    {
        if (bow == null || bow.Hand == null || recentlyTeleported) return;

        if (bow.Hand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) || 
            bow.Hand.otherHand.controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Teleported();
            this.Invoke(ResetTeleported, teleportCooldown);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (recentlyTeleported) return;

        if (impactLayers == (impactLayers | (1 << collision.gameObject.layer)))
        {
            Rigidbody other = collision.gameObject.GetComponentInChildren<Rigidbody>();

            if (other == null) return;

            other.AddForce(velocityEstimator.GetVelocityEstimate() * velocityModifier);
        }
    }

    private void Teleported()
    {
        recentlyTeleported = true;

        if (velocityEstimator)
            velocityEstimator.FinishEstimatingVelocity();

        if (myCollider)
            myCollider.isTrigger = true;
    }

    private void ResetTeleported()
    {
        recentlyTeleported = false;

        if (velocityEstimator)
            velocityEstimator.BeginEstimatingVelocity();

        if (myCollider)
            myCollider.isTrigger = false;
    }
}

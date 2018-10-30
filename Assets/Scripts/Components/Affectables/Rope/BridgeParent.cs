using UnityEngine;
using Valve.VR.InteractionSystem;

public class BridgeParent : MonoBehaviour {

    [SerializeField] private Rigidbody bridgeRb;
    [SerializeField] private float physicsDelay = 1.0f;

    private SpringJoint attachmentPoint;
    [SerializeField] private TeleportArea tp;

    private void Start () {
        this.Invoke(ResetBridgePhysics, physicsDelay);
        attachmentPoint = GetComponentInChildren<SpringJoint>();
	}
	
    public void Detached() {
        tp.SetLocked(false);
        Destroy(attachmentPoint.gameObject);
    }

	private void ResetBridgePhysics () {
        bridgeRb.isKinematic = false;
	}
}

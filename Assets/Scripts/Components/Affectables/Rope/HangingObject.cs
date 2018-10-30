using UnityEngine;
using System.Linq;

[RequireComponent(typeof(HingeJoint))]
public class HangingObject : MonoBehaviour
{
    [SerializeField] private float distanceFromRopeEnd = 0.6f;
    [SerializeField] private bool usingBridge = false;
    [SerializeField] public float physicsDelay = 1.0f;

    [SerializeField] private string attachTag = "ChainAttachPoint";

    //In Child of this object if usingBridge is true
    private HingeJoint chainJoint;

    //Connect rope end will attach to the previous "ChainLink" in a Chain, 
    //if usingBridge is true, we access the secondary attachmenet point (of this object) and attach it to the "bridgeAttachPoint02)

    private void Start()
    {
        if (usingBridge == false) return;

        chainJoint = transform.GetChild(0).GetComponent<HingeJoint>();
        
    }

    public void ConnectRopeEnd(Rigidbody _endRigidbody)
    {
        HingeJoint joint = GetComponent<HingeJoint>();

        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = _endRigidbody;

        joint.anchor = Vector3.zero;
        joint.connectedAnchor = new Vector3(0f, -distanceFromRopeEnd, 0f);
        
        chainJoint = transform.GetChild(0).GetComponent<HingeJoint>();
        if (chainJoint == null) return;
        
        if (usingBridge) this.Invoke(ConnectBridge, physicsDelay);
    }

    public void ConnectBridge()
    {
        Rigidbody bridge = GetBridgeJoint();

        if (bridge == null ) return;

        chainJoint.autoConfigureConnectedAnchor = false;
        chainJoint.connectedBody = bridge;

        chainJoint.anchor = Vector3.zero;

        chainJoint.connectedAnchor = new Vector3(0f, -distanceFromRopeEnd, 0f);
    }

    public Rigidbody GetBridgeJoint()
    {
        Rigidbody rb = transform.root.GetComponentsInChildren<Rigidbody>().Where(r => r.tag == attachTag).ToArray()[0];
        return rb;
    }

    public float GetPhysicsDelayTime()
    {
        return physicsDelay;
    }

    public void Detach() {
        if (usingBridge == false) return;

        if (chainJoint.connectedBody != null)
            chainJoint.connectedBody = null;

        Destroy(gameObject);
    }
}

using UnityEngine;

public class AttachToHinge : MonoBehaviour
{
    [SerializeField] private Rigidbody rbToAttach;
    [SerializeField] private float attachDelay = 1.0f;

    private Rigidbody[] rb;

    // Use this for initialization
    private void Start()
    {
        rb = transform.parent.parent.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rb.Length; i++)
        {
            if (rb[i].CompareTag("ChainAttachPoint"))
            {
                print(i);
                rbToAttach = rb[i];
            }
        }

        this.Invoke(Attach, attachDelay);
    }

    public void Attach()
    {
        if (rbToAttach != null)
        {
            HingeJoint joint = GetComponent<HingeJoint>();
            if (joint != null) joint.connectedBody = rbToAttach.GetComponent<Rigidbody>();
        }
    }
}

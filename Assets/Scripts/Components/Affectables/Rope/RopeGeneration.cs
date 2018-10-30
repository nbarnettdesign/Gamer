using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RopeGeneration : RopeParent
{
    [SerializeField] private Rigidbody hook;
    [SerializeField] private GameObject chainLinkPrefab;
    [SerializeField] private GameObject hangingObjectPrefab;
    [SerializeField] private int numOfChainLinks = 3;
    [SerializeField] private UnityEvent OnChainBroken;

    [Header("Debugging")]
    [SerializeField] private bool drawDebugLocation;
    [SerializeField] private float debugSphereRadius;
    [SerializeField] private Color debugSphereColour;

    private GameObject newObject;

    protected override void Awake()
    {
        GenerateRope();
    }

    protected void GenerateRope()
    {
        chains = new List<ChainHover> { GetComponent<ChainHover>() };

        Rigidbody previousRigidbody = hook;

        float rot = 0;

        for (int i = 0; i < numOfChainLinks; i++)
        {
            GameObject newLink = SimplePool.Spawn(chainLinkPrefab, previousRigidbody.gameObject.transform.position, transform.rotation);
            newLink.transform.GetChild(0).transform.rotation *= Quaternion.Euler(0, rot, 0);
            newLink.transform.SetParent(transform);

            chains.Add(newLink.GetComponent<ChainHover>());

            HingeJoint joint = newLink.GetComponent<HingeJoint>();
            joint.connectedBody = previousRigidbody;

            //if i is second last save as previous
            if (i < numOfChainLinks - 1)
                previousRigidbody = newLink.GetComponent<Rigidbody>();
            else
            {
                newObject = Instantiate(hangingObjectPrefab, previousRigidbody.transform);
                newObject.GetComponent<HangingObject>().ConnectRopeEnd(newLink.GetComponent<Rigidbody>());
                newObject.transform.SetParent(transform);
            }
            rot += 90f;
        }
    }

    public override void RopeBroken()
    {
        base.RopeBroken();

        if (newObject == null)
            return;

        chains = new List<ChainHover> { GetComponent<ChainHover>() };

        newObject.GetComponent<HangingObject>().Detach();

        OnChainBroken.Invoke();

        if (transform.parent)
        {
            BridgeParent bridge = transform.parent.GetComponent<BridgeParent>();

            if (bridge != null)
                bridge.Detached();
        }

        newObject = null;
    }

    private void OnDrawGizmos()
    {
        if (drawDebugLocation == false)
            return;

        if (Gizmos.color != debugSphereColour)
            Gizmos.color = debugSphereColour;

        Gizmos.DrawSphere(transform.position, debugSphereRadius);
    }
}

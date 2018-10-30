using UnityEngine;
using Valve.VR.InteractionSystem;

public class Stabbable : MonoBehaviour {
    [SerializeField] private LayerMask arrowLayer;
    [SerializeField] public bool hasBeenStabbed = false;
    [SerializeField] private float stabDist = -0.25f;
    private Rigidbody rb;
    private Collider coll;
 
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    public void Stabbed(Arrow arrow)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        transform.SetParent(arrow.ArrowHeadRB.transform);
        //Set the obj slightly up the arrow
        transform.localPosition = new Vector3(0, 0, stabDist);

        rb.isKinematic = true;

        //Trying to get the speed of the arrow (already going when it stabs)
        //rb.velocity = arrow.ArrowHeadRB.velocity;
        //rb.angularVelocity = arrow.ArrowHeadRB.angularVelocity;
        //rb.useGravity = false;

        hasBeenStabbed = true;
        coll.isTrigger = true;
    }

    public void UnparentObject()
    {
        print("Deparented!");
        if(rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        transform.parent = null;

        rb.velocity = Vector3.zero;

        rb.useGravity = true;
        rb.isKinematic = true;

        hasBeenStabbed = false;

        coll.isTrigger = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        //if already impaled, do not re-child obj to arrow
        if (hasBeenStabbed) return;
        if (arrowLayer == (arrowLayer | (1 << other.gameObject.layer)))
        {
            Arrow arrow = other.collider.GetComponent<Arrow>();
            if (arrow == null)
            {
                arrow = other.transform.parent.GetComponentInParent<Arrow>();
            }
            if (arrow)
                Stabbed(arrow);
        }
    }

    
}

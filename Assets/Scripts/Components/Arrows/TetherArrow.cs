using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(LineRenderer))]
public class TetherArrow : Arrow
{
    [SerializeField] private float reelSpeed;
    [SerializeField] private float minDistance;
    [SerializeField] private string arrowTag;

    public bool ReelingIn { get { return reelIn; } }
    public GameObject ReelingObject { get { return joint.connectedBody != null ? joint.connectedBody.gameObject : null; } }
    public Transform CollidedObject { get { return collidedWithObject; } }
    public TetherArrow PairedArrow { get { return pairedArrow != null ? pairedArrow : null; } }

    private bool reelIn;

    private Transform collidedWithObject;
    private LineRenderer tetherRenderer;
    private ConfigurableJoint joint;
    private SoftJointLimit jointLimit;
    private TetherArrow pairedArrow;

    protected override void Start()
    {
        base.Start();

        tetherRenderer = GetComponent<LineRenderer>();
        joint = GetComponentInChildren<ConfigurableJoint>();
        joint.gameObject.SetActive(false);
        if (string.IsNullOrEmpty(arrowTag))
        {
            Debug.LogError(string.Format("Tether arrow needs a tag to check against!"));
        }
        else if (transform.tag != arrowTag)
        {
            transform.tag = arrowTag;
        }
    }

    private void Update()
    {
        if (reelIn && joint != null && joint.connectedBody != null && joint.linearLimit.limit > minDistance)
        {
            jointLimit.limit -= reelSpeed * Time.deltaTime;
            joint.linearLimit = jointLimit;
            joint.connectedBody.AddForce(joint.connectedBody.transform.position - transform.position);

            UpdateTetherRenderer();
        }
    }

    public void SetReeling()
    {
        reelIn = true;
    }

    public void Disconnect()
    {
        joint.connectedBody = null;
    }

    public void GivePairedArrow(TetherArrow pair)
    {
        pairedArrow = pair;
    }

    public void GiveTetherPoint(Rigidbody impactedObject)
    {
        if (tetherRenderer != null)
        {
            tetherRenderer.enabled = true;
        }

        joint.connectedBody = impactedObject;

        if (joint.connectedBody == null) return;

        reelIn = true;
        jointLimit = new SoftJointLimit
        {
            limit = Vector3.Distance(impactedObject.transform.position, transform.position)
        };

        joint.linearLimit = jointLimit;
        UpdateTetherRenderer();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (stuckInTarget == false)
        {
            StickInTarget(collision, true);
        }

        joint.gameObject.SetActive(true);

        collidedWithObject = collision.transform;

        foreach (GameObject tetherArrow in GameObject.FindGameObjectsWithTag(arrowTag))
        {
            TetherArrow t = tetherArrow.GetComponent<TetherArrow>();

            if (tetherArrow == gameObject || t == null || t.CollidedObject == collidedWithObject) continue;

            // Is the tether arrow is reeling in the object we hit?
            // if so stop it
            if (t.reelIn && t.ReelingObject != null && t.ReelingObject == collision.gameObject)
            {
                t.Disconnect();
            }

            if (t.ReelingIn) continue;

            // Pair the arrows
            pairedArrow = t;
            // We found an arrow so we are the one that needs to move towards it
            // Did we collide with something that has a rigidbody?
            if (collision.gameObject.GetComponent<Rigidbody>())
            {
                // If we did we should start reeling the object to the 1st point
                t.GiveTetherPoint(collision.gameObject.GetComponent<Rigidbody>());
                t.GivePairedArrow(this);
            }
            else
            {
                // If we didn't flag the previous arrow as isReeling so the new point is the marked location
                t.SetReeling();
            }

            break;
        }
    }

    private void UpdateTetherRenderer()
    {
        if (tetherRenderer != null)
        {
            if (tetherRenderer.GetPosition(0) != transform.position)
            {
                tetherRenderer.SetPosition(0, transform.position);
            }
            if (collidedWithObject && tetherRenderer.GetPosition(1) != collidedWithObject.transform.position)
            {
                tetherRenderer.SetPosition(1, collidedWithObject.transform.position);
            }
        }
    }
}

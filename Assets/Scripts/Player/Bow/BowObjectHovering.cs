using UnityEngine;
using Valve.VR.InteractionSystem;

public class BowObjectHovering : MonoBehaviour {
    [SerializeField] private LayerMask hoveringMask;
    [Tooltip("Length of the spherecast from the bow, if set at 0 will be set to infinity")]
    [SerializeField] private float sphereCastLength;
    [SerializeField] private float sphereCastRadius;
    [SerializeField] private float sphereCastDelay;

    public float SphereCastLength { get { return sphereCastLength; } }
    public Hoverable HoveringAnchor { get { return highlightedObject; } }

    private Longbow bow;
    private float maxChainDistanceSqr;
    private Hoverable highlightedObject;

    private float rayDelayCount;
    private bool hoveredSomething;

    private void Start()
    {
        bow = GetComponentInParent<Longbow>();
        maxChainDistanceSqr = GetComponent<BowAnchor>().MaxChainDistanceSqr;

        rayDelayCount = sphereCastDelay;
    }

    private void Update()
    {
        CheckForHoverables();
    }

    private void CheckForHoverables()
    {
        if (bow == null)
            return;

        if (bow.Nocked == false)
        {
            if (hoveredSomething && highlightedObject)
            {
                highlightedObject.OnBowHoverExit();
                highlightedObject = null;
            }    

            return;
        }

        if (rayDelayCount <= sphereCastDelay)
        {
            rayDelayCount += Time.deltaTime;
            return;
        }
        rayDelayCount = 0f;

        RaycastHit hit = new RaycastHit();

        if (Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, out hit,
            sphereCastLength > 0 ? sphereCastLength : Mathf.Infinity, hoveringMask))
        {
            Hoverable h = hit.transform.GetComponent<Hoverable>();

            if (h)
            {
                if(highlightedObject != null)
                    highlightedObject.OnBowHoverExit();

                if ((hit.transform.position - transform.position).sqrMagnitude >= maxChainDistanceSqr)
                    return;

                highlightedObject = h;
                highlightedObject.OnBowHover();
                hoveredSomething = true;
            }
            else if (highlightedObject)
            {
                highlightedObject.OnBowHoverExit();
                highlightedObject = null;
            }
        }
        else if (highlightedObject)
        {
            highlightedObject.OnBowHoverExit();
            highlightedObject = null;
            hoveredSomething = false;
        }
    }
}
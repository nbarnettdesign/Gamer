using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(VelocityEstimator))]
[RequireComponent(typeof(BowObjectHovering))]
public class BowAnchor : MonoBehaviour
{
    [Header("Velocity Settings")]
    [Tooltip("Minimum velocity that the bow needs to be over to pass velocity info to the anchor")]
    [SerializeField] private float minSendMagnitudeSqr;

    [Header("Chain Settings")]
    [SerializeField] private float maxChainDistanceSqr;
    [SerializeField] private GameObject chain;
    [SerializeField] private GameObject chainLinkPrefab;
    [SerializeField] private int segments;
    [SerializeField] private float followSpeed;
    [Tooltip("Transform to anchor the chain to, if null will go to this transform")]
    [SerializeField] private Transform anchorTransform;

    [Header("Effects")]
    [SerializeField] private AudioClip anchoredSound;
    [SerializeField] private GameObject anchoredParticle;
    [SerializeField] private AudioClip anchorBrokenSound;
    [SerializeField] private GameObject brokenParticle;

    [Header("Haptic Feedback")]
    [SerializeField] private float pulseDuration;
    [SerializeField] private float pulseInterval;
    [SerializeField] private ushort anchoredFeedbackStrength;
    [SerializeField] private ushort movedFeedbackStrength;
    [SerializeField] private ushort breakFeedbackStrength;

    public bool IsAnchored { get { return anchoredPoint; } }
    public float MaxChainDistanceSqr { get { return maxChainDistanceSqr; } }

    private bool renderChain;

    private BowObjectHovering objectHovering;
    private GameObject followingObject;
    private AnchorPoint anchoredPoint;
    private LineRenderer chainRenderer;
    private List<GameObject> chainLinks;
    private Longbow bow;
    private VelocityEstimator velocityEstimator;

    private void Start()
    {
        chainRenderer = SimplePool.Spawn(chain, transform.position, Quaternion.identity).GetComponent<LineRenderer>();
        chainRenderer.transform.SetParent(transform, false);

        chainRenderer.positionCount = segments + 2;
        objectHovering = GetComponent<BowObjectHovering>();

#if UNITY_EDITOR
        if (chainRenderer == null)
            Debug.Log(string.Format("Bow anchor is missing a line renderer!"));
#endif

        if (chainRenderer != null)
            chainRenderer.gameObject.SetActive(false);

        if (anchorTransform == null)
            anchorTransform = transform;

        bow = GetComponentInParent<Longbow>();
        velocityEstimator = GetComponent<VelocityEstimator>();

        if(Teleport.instance)
            Teleport.instance.onTeleport += BreakAnchor;

        chainLinks = new List<GameObject>();

        for (int i = 0; i < chainRenderer.positionCount; i++)
        {
            GameObject link = SimplePool.Spawn(chainLinkPrefab, transform.position, Quaternion.identity);

            if(i % 2 != 0)
            {
                Vector3 rotation = new Vector3(90, 0, 0);
                link.transform.rotation = Quaternion.Euler(rotation);
            }

            link.transform.SetParent(transform);
            link.SetActive(false);
            chainLinks.Add(link);
        }
    }

    private void Update()
    {
        UpdateChain();
        UpdateVelocity();
    }

    public void ArrowReleased(GameObject arrow)
    {
        //if for some reason the line renderer is on turn it off
        chainRenderer.gameObject.SetActive(false);
        renderChain = false;

        if (objectHovering == null || objectHovering.HoveringAnchor == null)
            return;

        if (objectHovering.HoveringAnchor is AnchorPoint)
        {
            AnchorPoint p = objectHovering.HoveringAnchor as AnchorPoint;

            if (p.IsLocked)
                return;

            renderChain = true;
            followingObject = arrow;

            SetChain();
        }
    }

    public void AnchorObject(AnchorPoint anchoredPoint)
    {
        if ((anchoredPoint.transform.position - transform.position).sqrMagnitude > maxChainDistanceSqr || 
            followingObject && followingObject.GetComponent<Arrow>() && followingObject.GetComponent<Arrow>().NonAnchorPointCollision)
        {
            chainRenderer.gameObject.SetActive(false);
            renderChain = false;

            anchoredPoint.OnAnchorExit();
            return;
        }

        this.anchoredPoint = anchoredPoint;

        velocityEstimator.BeginEstimatingVelocity();
        if (anchoredSound != null)
            AudioSource.PlayClipAtPoint(anchoredSound, transform.position);

        if (chainRenderer)
        {
            renderChain = true;
            SetChain();
        }

        if (anchoredParticle)
        {
            SimplePool.Spawn(anchoredParticle, anchorTransform.position, anchoredParticle.transform.rotation);

            //Do stuff here to make sure emitter is along the length of the chain
        }

        StartCoroutine(TriggerPulse(bow.Hand, anchoredFeedbackStrength, pulseDuration, pulseInterval));
    }

    public void BreakAnchor()
    {
        if (chainRenderer.gameObject.activeInHierarchy)
        {
            chainRenderer.gameObject.SetActive(false);
            renderChain = false;

            for (int i = 0; i < chainLinks.Count; i++)
            {
                chainLinks[i].SetActive(false);
            }
        }
        

        if (anchoredPoint == null)
            return;

        if (anchorBrokenSound)
            AudioSource.PlayClipAtPoint(anchorBrokenSound, transform.position);

        followingObject = null;
        

        if (brokenParticle)
        {
            SimplePool.Spawn(brokenParticle, anchorTransform.position, anchoredParticle.transform.rotation);

            //Do stuff here to make sure emitter is along the length of the chain
        }

        anchoredPoint.OnAnchorExit();
        anchoredPoint = null;

        StartCoroutine(TriggerPulse(bow.Hand, breakFeedbackStrength, pulseDuration, pulseInterval));
    }

    private void SetChain()
    {
        if (followingObject == null)
            return;

        chainRenderer.gameObject.SetActive(true);

        chainRenderer.SetPosition(0, anchorTransform.position);
        chainLinks[0].transform.position = anchorTransform.position;
        chainLinks[0].SetActive(true);

        chainRenderer.SetPosition(chainRenderer.positionCount - 1, followingObject.transform.position);
        chainLinks[chainRenderer.positionCount - 1].transform.position = followingObject.transform.position;
        chainLinks[0].SetActive(true);

        Vector3 directionNormalized = (chainRenderer.GetPosition(chainRenderer.positionCount - 1) - chainRenderer.GetPosition(0)).normalized;
        float totalDistance = Vector3.Distance(chainRenderer.GetPosition(0), chainRenderer.GetPosition(chainRenderer.positionCount - 1));
        float distance = totalDistance / (segments + 1);

        Vector3 currentPos = chainRenderer.GetPosition(0);

        for (int i = 1; i < chainRenderer.positionCount - 1; i++)
        {
            currentPos += directionNormalized * distance;

            chainRenderer.SetPosition(i, currentPos);
            chainLinks[i].transform.position = currentPos;
            chainLinks[i].SetActive(true);
        }
    }

    private void UpdateChain()
    {
        if (renderChain == false || followingObject == null)
            return;

        // If another arrow has been nocked break the chain
        if (bow != null && bow.Nocked || followingObject.GetComponent<Arrow>().NonAnchorPointCollision)
        {
            BreakAnchor();
            return;
        }

        if (chainRenderer.GetPosition(0) != anchorTransform.position)
        {
            chainRenderer.SetPosition(0, anchorTransform.position);
            chainLinks[0].transform.position = anchorTransform.position;
        }

        for (int i = 1; i < chainRenderer.positionCount - 1; i++)
        {
            Vector3 targetPos = Vector3.Lerp(chainRenderer.GetPosition(i - 1), chainRenderer.GetPosition(i + 1), 0.5f);
            chainRenderer.SetPosition(i, Vector3.MoveTowards(chainRenderer.GetPosition(i), targetPos, followSpeed * Time.deltaTime));
            chainLinks[i].transform.position = chainRenderer.GetPosition(i);
        }

        if (chainRenderer.GetPosition(chainRenderer.positionCount - 1) != followingObject.transform.position)
        {
            chainRenderer.SetPosition(chainRenderer.positionCount - 1, followingObject.transform.position);
            chainLinks[chainRenderer.positionCount - 1].transform.position = followingObject.transform.position;
        }
    }

    private void UpdateVelocity()
    {
        if (anchoredPoint == null)
            return;

        if (velocityEstimator.GetVelocityEstimate().sqrMagnitude > minSendMagnitudeSqr * minSendMagnitudeSqr)
        {
            StartCoroutine(TriggerPulse(bow.Hand, movedFeedbackStrength, pulseDuration, pulseInterval));
            anchoredPoint.MoveObject(velocityEstimator.GetVelocityEstimate());
        }
    }

    private IEnumerator TriggerPulse(Hand hand, ushort pulseStrength, float pulseTime, float pulseInterval)
    {
        float time = 0f;
        float interval = 0f;

        while (time <= pulseTime)
        {
            if (hand == null)
                yield break;

            time += Time.deltaTime;
            interval += Time.deltaTime;

            if (interval >= pulseInterval)
            {
                if (hand.controller != null)
                    hand.controller.TriggerHapticPulse(pulseStrength);
                interval = 0f;
            }

            yield return null;
        }
    }
}

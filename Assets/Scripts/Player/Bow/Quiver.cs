using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Quiver : MonoBehaviour
{
    [Header("Arrow Hand In Quiver")]
    [SerializeField] private ushort handInPulseStrength;
    [SerializeField] private float handInPulseTime;
    [SerializeField] private float handInPulseInterval;

    [Header("Arrow Grabbed")]
    [SerializeField] private ushort arrowGrabbedPulseStrength;
    [SerializeField] private float arrowGrabbedPulseTime;
    [SerializeField] private float arrowGrabbedPulseInterval;

    [Header("Inventory Side Switching")]
    [SerializeField] private InventorySlot playerSatchel;

    public bool HandIn { get { return bowHandIn || arrowHandIn; } }

    private float arrowPulseCount;
    private bool hasPulsed;

    private bool arrowHandIn;
    private ArrowHand arrowHand;

    private bool bowHandIn;
    private Longbow bow;
    private Hand bowHand;

    private Hand emptyHand;

    private void Update()
    {
        UpdateHandInput();
    }

    public void ResetArrowPulse()
    {
        hasPulsed = false;
    }

    private void UpdateHandInput()
    {
        if (arrowHandIn && arrowHand && arrowHand.Hand.GetStandardInteractionButtonDown() && arrowHand.HasArrow == false)
        {
            StartCoroutine(TriggerPulse(arrowHand.Hand, arrowGrabbedPulseStrength, arrowGrabbedPulseTime, arrowGrabbedPulseInterval));
            arrowHand.AttachArrow();
        }
        else if (bowHandIn && bowHand && bowHand.GetStandardInteractionButtonDown())
        {
            bowHand.DetachObject(bow.gameObject);
            ResetQuiver();
        }
        else if (emptyHand && emptyHand.GetStandardInteractionButtonDown())
            ReattachBow();
    }

    private void SetupHands(GameObject obj)
    {
        ArrowHand a = obj.GetComponentInChildren<ArrowHand>();
        Longbow b = obj.GetComponentInChildren<Longbow>();

        if (a)
        {
            arrowHand = a;
            bowHand = a.Hand.otherHand;
            bow = bowHand.GetComponentInChildren<Longbow>();
        } else if (b)
        {
            bow = b;
            bowHand = b.GetComponentInParent<Hand>();
            arrowHand = bowHand.otherHand.GetComponentInChildren<ArrowHand>();
        }
    }

    private void ResetQuiver()
    {
        bowHand = null;
        bowHandIn = arrowHandIn = false;
    }

    private void ReattachBow()
    {
        if (bow == null || arrowHand == null)
            return;

        bowHand = emptyHand;

        bow.gameObject.SetActive(true);
        bowHand.AttachObject(bow.gameObject, Hand.startingItemAttachmentFlags);

        arrowHand.Hand.DetachObject(arrowHand.gameObject);
        bowHand.otherHand.AttachObject(arrowHand.gameObject, Hand.startingItemAttachmentFlags);

        emptyHand = null;

        if (playerSatchel)
            playerSatchel.transform.localPosition = new Vector3(-playerSatchel.transform.localPosition.x, 
                playerSatchel.transform.localPosition.y, playerSatchel.transform.localPosition.z);
    }

    private void OnTriggerStay(Collider other)
    {
        if (arrowHand == null || bow == null)
        {
            SetupHands(other.gameObject);
        }

        if(bowHand == null)
        {
            Hand h = other.GetComponent<Hand>();

            if (h)
            {
                emptyHand = h;

                if (hasPulsed == false)
                {
                    hasPulsed = true;
                    StartCoroutine(TriggerPulse(h, handInPulseStrength, handInPulseTime, handInPulseInterval));
                }
            }
        }
        else if (arrowHand && arrowHand.HasArrow == false && (other.gameObject == arrowHand.gameObject || other.transform == arrowHand.gameObject.transform.parent))
        {
            if (hasPulsed == false && arrowHand.HasArrow == false)
            {
                hasPulsed = true;
                StartCoroutine(TriggerPulse(arrowHand.Hand, handInPulseStrength, handInPulseTime, handInPulseInterval));
            }
            arrowHandIn = true;
        }
        else if (bow && bowHand && (other.gameObject == bow.gameObject || other.gameObject == bowHand.gameObject))
        {
            bow = other.GetComponentInChildren<Longbow>();

            if (bow != null)
            {
                if (bowHand == null)
                    bowHand = bow.GetComponentInParent<Hand>();

                if (hasPulsed == false)
                {
                    hasPulsed = true;

                    if (bowHand)
                        StartCoroutine(TriggerPulse(bowHand, handInPulseStrength, handInPulseTime, handInPulseInterval));
                }
                bowHandIn = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null)
            return;

        if (arrowHand && (other.gameObject == arrowHand.gameObject || other.transform == arrowHand.gameObject.transform.parent))
        {
            arrowHandIn = false;
            hasPulsed = false;
        }
        else if (bow && (other.gameObject == bow.gameObject || other.transform == bow.gameObject.transform.parent))
        {
            bowHandIn = false;
            hasPulsed = false;
        }

        if (emptyHand)
        {
            emptyHand = null;
            hasPulsed = false;
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
                if(hand.controller != null)
                    hand.controller.TriggerHapticPulse(pulseStrength);
                interval = 0f;
            }

            yield return null;
        }
    }
}

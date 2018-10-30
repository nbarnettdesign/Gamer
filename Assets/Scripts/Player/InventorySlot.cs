using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private GameObject startingObject;
    [SerializeField] private InventoryType slotType;
    [SerializeField] private bool hideOnAttach;
    [SerializeField] private bool autoAttach;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask propLayer;
    [Tooltip("The scale to set an attached object to, If this is set to 0 or less objects will not shrink")]
    [SerializeField] private float shrinkSize;
    [SerializeField] private float shrinkTime;
    [SerializeField] private AnimationCurve shrinkCurve;

    public bool Empty { get { return attachedObject == null; } }

    private GameObject attachedObject;
    private Vector3 originalScale;
    private Hand handIn;

    private Quiver quiver;

    private void Start()
    {
        if (startingObject)
        {
            attachedObject = SimplePool.Spawn(startingObject, transform.position, Quaternion.identity);

            if (hideOnAttach)
                attachedObject.SetActive(false);

            attachedObject.GetComponent<Rigidbody>().isKinematic = true;
            originalScale = attachedObject.transform.lossyScale;
        }

        quiver = FindObjectOfType<Quiver>();
    }

    private void Update()
    {
        CheckInput();
    }

    public void AttachObject(GameObject obj)
    {
        // Attachable requires throwable so even if this is an attachable slot
        // we can get a throwable component off of it
        Throwable throwable = obj.GetComponent<Throwable>();

        if (throwable.AttachedToHand)
            throwable.AttachedToHand.DetachObject(obj);

        attachedObject = obj;

        attachedObject.GetComponent<Rigidbody>().isKinematic = true;


        attachedObject.transform.SetParent(transform);

        if (hideOnAttach)
            attachedObject.SetActive(false);

        else if (shrinkSize > 0)
        {
            originalScale = attachedObject.transform.lossyScale;
            StartCoroutine(LerpController.Instance.Scale(attachedObject.transform, originalScale,
                Vector3.one * shrinkSize, shrinkTime, shrinkCurve));
        }
    }

    public void DetachHeldObject()
    {
        if (hideOnAttach)
            attachedObject.SetActive(true);

        attachedObject.transform.position = handIn.transform.position;
        handIn.AttachObject(attachedObject, attachedObject.GetComponent<Throwable>().attachmentFlags);

        if (shrinkSize > 0)
        {
            StartCoroutine(LerpController.Instance.Scale(attachedObject.transform,
                attachedObject.transform.localScale, originalScale, shrinkTime, shrinkCurve));
        }

        attachedObject = null;
    }

    private void CheckInput()
    {
        if (handIn == null || quiver.HandIn)
            return;

        if (handIn.GetStandardInteractionButtonDown())
        {
            if (attachedObject && handIn.currentAttachedObject != null)
                DetachHeldObject();
        }
        else if (handIn.GetStandardInteractionButtonUp() && attachedObject == null)
        {
            if (handIn.currentAttachedObject && CheckCorrectObject(handIn.currentAttachedObject))
                AttachObject(handIn.currentAttachedObject);
        }
    }

    private bool CheckCorrectObject(GameObject obj)
    {
        switch (slotType)
        {
            case InventoryType.Attachable:
                if (obj.GetComponent<Attachable>())
                    return true;
                break;
            case InventoryType.Throwable:
                if (obj.GetComponent<Throwable>())
                    return true;
                break;
            default:
                break;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)) && other.gameObject.GetComponent<Hand>())
            handIn = other.gameObject.GetComponent<Hand>();
        else if (propLayer == (propLayer | (1 << other.gameObject.layer)))
        {
            if (autoAttach)
            {
                if (CheckCorrectObject(other.gameObject))
                    AttachObject(other.gameObject);
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (handIn == null)
            return;

        if (other.gameObject == handIn.gameObject)
            handIn = null;
    }
}

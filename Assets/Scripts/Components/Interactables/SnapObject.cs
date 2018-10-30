using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class SnapObject : MonoBehaviour
{
    [SerializeField] private string objectName;
    [SerializeField] private SoundPlayOneshot connectedSound;
    [SerializeField] private float lerpTime;
    [SerializeField] private AnimationCurve snapToCurve;
    [SerializeField] private bool useChild = false;

    protected GameObject heldObject;
    protected MeshRenderer meshRenderer;
    protected Collider col;

    protected virtual void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();

        if (useChild)
        {
            foreach (Transform child in transform)
            {
                if (child == transform)
                    continue;

                if (child.GetComponent<Throwable>())
                {
                    heldObject = child.gameObject;
                    break;
                }
            }
        }
    }

    public virtual void Detach(GameObject obj)
    {
        if (heldObject == obj)
            heldObject = null;
    }

    protected virtual void SnapTo(Throwable t)
    {
        if (t == null || t.AttachedToHand == null || heldObject != null) return;

        t.transform.SetParent(transform);
        t.ForceDetach();

        StartCoroutine(LerpSnap(t.transform));

        t.transform.SetParent(transform);

        SnapEvents events = t.gameObject.GetComponent<SnapEvents>();

        if (events != null)
        {
            events.SnapEnter();

            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
        }
    }

    private IEnumerator LerpSnap(Transform lerpObject)
    {
        float t = 0f;

        Vector3 startPos = lerpObject.transform.position;
        Vector3 endPos = transform.position;

        Quaternion startRotation = lerpObject.transform.rotation;
        Quaternion endRotation = transform.rotation;

        Rigidbody r = lerpObject.GetComponent<Rigidbody>();

        while (t <= lerpTime)
        {
            t += Time.deltaTime;

            lerpObject.position = Vector3.LerpUnclamped(startPos, endPos, snapToCurve.Evaluate(t / lerpTime));
            lerpObject.rotation = Quaternion.LerpUnclamped(startRotation, endRotation, snapToCurve.Evaluate(t / lerpTime));

            if (r && r.isKinematic == false)
                r.isKinematic = true;

            yield return null;
        }

        lerpObject.position = endPos;
        lerpObject.rotation = endRotation;

        if (connectedSound != null)
        {
            connectedSound.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useChild && other.gameObject == heldObject)
        {
            SnapTo(heldObject.GetComponent<Throwable>());
        }

        else if (other.name == objectName)
        {
            SnapTo(other.GetComponent<Throwable>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (useChild && other.gameObject == heldObject)
        {

            SnapTo(heldObject.GetComponent<Throwable>());
        }

        else if (other.tag == objectName)
        {
            SnapTo(other.GetComponent<Throwable>());
        }
    }
}

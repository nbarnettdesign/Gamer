using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandState { DrawBack, ArrowHeld, Closed, Idle, Grab, Open }

public class HandShapes : MonoBehaviour
{
    [Range(0f, 1f), SerializeField] private float drawbackWeight;
    [Range(0f, 1f), SerializeField] private float arrowHeldWeight;
    [Range(0f, 1f), SerializeField] private float closedWeight;
    [Range(0f, 1f), SerializeField] private float idleWeight;
    [Range(0f, 1f), SerializeField] private float grabWeight;

    private SkinnedMeshRenderer skinRenderer;

    private IEnumerator drawBackEnumerator;
    private IEnumerator arrowHeldEnumerator;
    private IEnumerator closedEnumerator;
    private IEnumerator idleEnumerator;
    private IEnumerator grabEnumerator;

    private void Start()
    {
        skinRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    public void AdjustWeight(HandState shape, float time, float weight = 1f)
    {
        if (gameObject.activeInHierarchy == false || skinRenderer == null)
            return;

        switch (shape)
        {
            case HandState.DrawBack:
                if (drawBackEnumerator != null)
                    StopCoroutine(drawBackEnumerator);

                drawBackEnumerator = AdjustShape(0, weight, time);
                StartCoroutine(drawBackEnumerator);

                break;
            case HandState.ArrowHeld:
                if (arrowHeldEnumerator != null)
                    StopCoroutine(arrowHeldEnumerator);

                arrowHeldEnumerator = AdjustShape(1, weight, time);
                StartCoroutine(arrowHeldEnumerator);
                break;
            case HandState.Closed:
                if (closedEnumerator != null)
                    StopCoroutine(closedEnumerator);

                closedEnumerator = AdjustShape(2, weight, time);
                StartCoroutine(closedEnumerator);
                break;
            case HandState.Idle:
                if (idleEnumerator != null)
                    StopCoroutine(idleEnumerator);

                idleEnumerator = AdjustShape(3, weight, time);
                StartCoroutine(idleEnumerator);
                break;
            case HandState.Grab:
                if (grabEnumerator != null)
                    StopCoroutine(grabEnumerator);

                grabEnumerator = AdjustShape(4, weight, time);
                StartCoroutine(grabEnumerator);
                break;
            default:
                break;
        }
    }

    private IEnumerator AdjustShape(int shapeIndex, float finalWeight, float time)
    {
        float t = 0f;
        float startWeight = skinRenderer.GetBlendShapeWeight(shapeIndex);
        finalWeight *= 100f;

        while(t <= time)
        {
            t += Time.deltaTime;
            skinRenderer.SetBlendShapeWeight(shapeIndex, Mathf.Lerp(startWeight, finalWeight, t / time));
            yield return null;
        }

        skinRenderer.SetBlendShapeWeight(shapeIndex, finalWeight);
    }

    private void OnDisabled()
    {
        StopAllCoroutines();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        skinRenderer = GetComponent<SkinnedMeshRenderer>();

        if (skinRenderer == null)
            return;

        if (skinRenderer.GetBlendShapeWeight(0) != drawbackWeight * 100)
            skinRenderer.SetBlendShapeWeight(0, drawbackWeight * 100);

        if (skinRenderer.GetBlendShapeWeight(1) != arrowHeldWeight * 100)
            skinRenderer.SetBlendShapeWeight(1, arrowHeldWeight * 100);

        if (skinRenderer.GetBlendShapeWeight(2) != closedWeight * 100)
            skinRenderer.SetBlendShapeWeight(2, closedWeight * 100);

        if (skinRenderer.GetBlendShapeWeight(3) != idleWeight * 100)
            skinRenderer.SetBlendShapeWeight(3, idleWeight * 100);

        if (skinRenderer.GetBlendShapeWeight(4) != grabWeight * 100)
            skinRenderer.SetBlendShapeWeight(4, grabWeight * 100);
    }
#endif
}

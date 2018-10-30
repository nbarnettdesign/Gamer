using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ArrowMagnetEffector : ArrowEffector
{
    [Header("Magnet Settings")]
    [SerializeField] private float pullTime;
    [SerializeField] private AnimationCurve pullCurve;

    protected override void EffectArrow(Arrow arrow)
    {
        base.EffectArrow(arrow);

        arrow.ShaftRB.isKinematic = true;
        arrow.ArrowHeadRB.isKinematic = true;

        StartCoroutine(PullToPoint(arrow.transform));
        StartCoroutine(LookAtPoint(arrow.transform));
    }

    private IEnumerator PullToPoint(Transform arrow)
    {
        float t = 0f;

        Vector3 startPos = arrow.position;

        while (t <= pullTime)
        {
            t += Time.deltaTime;
            arrow.position = Vector3.Lerp(startPos, transform.position, pullCurve.Evaluate(t / pullTime));
            yield return null;
        }

        arrow.transform.position = transform.position;
    }

    private IEnumerator LookAtPoint(Transform arrow)
    {
        float t = 0f;

        Vector3 startPos = arrow.position;
        Quaternion startRotation = arrow.rotation;

        while (t <= pullTime)
        {
            t += Time.deltaTime;
            arrow.rotation = Quaternion.Slerp(arrow.rotation, Quaternion.LookRotation(transform.position - arrow.transform.position), Time.deltaTime);
            yield return null;
        }
    }
}

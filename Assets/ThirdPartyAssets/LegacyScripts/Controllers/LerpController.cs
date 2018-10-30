using System.Collections;
using UnityEngine;

public class LerpController : Singleton<LerpController>
{
    public void LerpPositionByAmount(Transform obj, Vector3 amount, float time, AnimationCurve curve)
    {
        StartCoroutine(ByAmount(obj, amount, time, curve));
    }

    public void LerpToPosition(Transform obj, Vector3 endPosition, float time, AnimationCurve curve)
    {
        StartCoroutine(ToPosition(obj, endPosition, time, curve));
    }

    public void LerpToRotation(Transform obj, Vector3 finalRotation, float time, AnimationCurve curve)
    {
        StartCoroutine(RotateTo(obj, finalRotation, time, curve));
    }

    public void RotateByAmount(Transform obj, Vector3 rotationAddition, float time, AnimationCurve curve)
    {
        StartCoroutine(AddRotation(obj, rotationAddition, time, curve));
    }

    public IEnumerator MoveLocalSpace(Transform obj, Vector3 endPosLocal, float time, AnimationCurve curve)
    {
        float t = 0f;
        Vector3 startPos = obj.localPosition;

        while(t <= time)
        {
            t += Time.deltaTime;
            obj.localPosition = Vector3.Lerp(startPos, endPosLocal, curve.Evaluate(t / time));

            yield return null;
        }

        obj.localPosition = endPosLocal;
    }

    public IEnumerator LerpFloat(float value, float endValue, float time, AnimationCurve curve)
    {
        float t = 0f;
        float startValue = value;

        while(t <= time)
        {
            t += Time.deltaTime;

            value = Mathf.Lerp(startValue, endValue, curve.Evaluate(t / time));
            yield return null;
        }

        value = endValue;
    }

    public IEnumerator Scale(Transform obj, Vector3 startScale, Vector3 endScale, float time, AnimationCurve curve) {
        float t = 0f;

        while(t <= time)
        {
            t += Time.deltaTime;

            obj.localScale = Vector3.LerpUnclamped(startScale, endScale, curve.Evaluate(t / time));
            yield return null;
        }

        obj.localScale = endScale;
    }


    private IEnumerator ByAmount(Transform obj, Vector3 amount, float time, AnimationCurve curve)
    {
        float t = 0f;

        Vector3 startPosition = obj.position;
        Vector3 endPosition = startPosition + amount;

        while (t <= time)
        {
            t += Time.deltaTime;

            obj.position = Vector3.LerpUnclamped(startPosition, endPosition, curve.Evaluate(t / time));

            yield return null;
        }

        obj.position = endPosition;
    }

    private IEnumerator ToPosition(Transform obj, Vector3 endPosition, float time, AnimationCurve curve)
    {
        float t = 0f;

        Vector3 startPosition = obj.position;

        while (t <= time)
        {
            t += Time.deltaTime;

            obj.position = Vector3.LerpUnclamped(startPosition, endPosition, curve.Evaluate(t / time));

            yield return null;
        }

        obj.position = endPosition;
    }

    private IEnumerator RotateTo(Transform obj, Vector3 finalRotation, float time, AnimationCurve curve)
    {
        float t = 0f;
        Quaternion startRotation = obj.rotation;
        Quaternion endRotation = Quaternion.Euler(finalRotation);

        while (t <= time)
        {
            t += Time.deltaTime;

            obj.rotation = Quaternion.LerpUnclamped(startRotation, endRotation, curve.Evaluate(t / time));
            yield return null;
        }

        obj.rotation = endRotation;
    }

    private IEnumerator AddRotation(Transform obj, Vector3 rotationAddition, float time, AnimationCurve curve)
    {
        float t = 0f;
        Quaternion startRotation = obj.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(rotationAddition);

        while (t <= time)
        {
            t += Time.deltaTime;

            obj.rotation = Quaternion.LerpUnclamped(startRotation, endRotation, curve.Evaluate(t / time));
            yield return null;
        }

        obj.rotation = endRotation;
    }
}

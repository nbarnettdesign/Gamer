using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldKey : MonoBehaviour
{
    [SerializeField] private GameObject orbObject;

    [Header("ShieldOrb")]
    [SerializeField] private float orbRadius;
    [SerializeField] private float shieldActivateTime;
    [SerializeField] private AnimationCurve activationCurve;
    [SerializeField] private float shieldDeactivateTime;
    [SerializeField] private AnimationCurve deactivateCurve;

    private void Start()
    {
        orbObject.transform.localScale *= orbRadius;
    }

    public void TurnOnOrb()
    {
        StartCoroutine(ToggleShield(orbRadius, shieldActivateTime, activationCurve, true));
    }

    public void TurnOffOrb()
    {
        StartCoroutine(ToggleShield(0f, shieldDeactivateTime, deactivateCurve, false));
    }

    private IEnumerator ToggleShield(float endingScale, float toggleTime, AnimationCurve scaleCurve, bool activate)
    {
        if (activate)
        {
            orbObject.SetActive(true);
        }

        Vector3 originalSize = Vector3.one * orbObject.transform.localScale.x;
        Vector3 endSize = Vector3.one * endingScale;
        float t = 0f;

        while (t <= toggleTime)
        {
            t += Time.deltaTime;

            orbObject.transform.localScale = Vector3.LerpUnclamped(originalSize, endSize, scaleCurve.Evaluate(t / toggleTime));

            yield return null;
        }

        if (activate == false)
            orbObject.SetActive(false);
    }
}

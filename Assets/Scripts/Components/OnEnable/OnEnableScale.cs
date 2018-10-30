using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableScale : MonoBehaviour {
    [SerializeField] private float scaleDelay;
    [SerializeField] private float finalScale;
    [SerializeField] private float scaleTime;
    [SerializeField] private AnimationCurve scaleCurve;

    private void OnEnable()
    {
        this.Invoke(StartScale, scaleDelay);
    }

    private void StartScale()
    {
        StartCoroutine(LerpController.Instance.Scale(transform, transform.localScale,
            Vector3.one * finalScale, scaleTime, scaleCurve));
    }
}

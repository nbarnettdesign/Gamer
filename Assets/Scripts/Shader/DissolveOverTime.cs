using System.Collections;
using UnityEngine;

public class DissolveOverTime : MonoBehaviour
{
    [SerializeField] private float dissolveDelay;
    [SerializeField] private float dissolveTime;
    [SerializeField] private AnimationCurve dissolveCurve;
    [SerializeField] private bool destroyAfterDissolve;
    [SerializeField] private float startValue;
    [SerializeField] private float endValue;

    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        StartCoroutine(Dissolve());
    }

    private IEnumerator Dissolve()
    {
        float t = 0f;

        while (t < dissolveDelay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;

        while (t <= dissolveTime)
        {
            t += Time.deltaTime;

            rend.material.SetFloat("_amount",
                Mathf.Lerp(startValue, endValue, dissolveCurve.Evaluate(t / dissolveTime)));

            yield return null;
        }

        if (destroyAfterDissolve)
        {
            Destroy(gameObject);
        }
    }
}

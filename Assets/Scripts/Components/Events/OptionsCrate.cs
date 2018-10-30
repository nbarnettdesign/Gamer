using System.Collections;
using UnityEngine;

public class OptionsCrate : MonoBehaviour {
    [SerializeField] private float fadeInTime;

    [Header("Colour")]
    [SerializeField] private Color startColour;

    [Header("Scale")]
    [SerializeField] private float startingScale;
    [SerializeField] private AnimationCurve scaleCurve;

    private Material material;
    private Color endColour;
    private float endScale;

    private void Start()
    {
        material = GetComponent<Renderer>().material;

        if (material == null) {
            Debug.LogError("Options crate needs a material you done goofed");
            return;
        }

        endColour = material.color;
        endScale = transform.localScale.x;
        transform.localScale = Vector3.one * startingScale;
    }

    public void StartFade()
    {
        if (material == null)
            return;

        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float t = 0f;

        Vector3 start = Vector3.one * startingScale;
        Vector3 end = Vector3.one * endScale;

        while (t <= fadeInTime) {
            t += Time.deltaTime;

            material.color = Color.Lerp(startColour, endColour, t / fadeInTime);
            transform.localScale = Vector3.LerpUnclamped(start, end, scaleCurve.Evaluate(t/fadeInTime));

            yield return null;
        }
    }
}

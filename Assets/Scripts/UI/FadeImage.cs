using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour
{
    [SerializeField] private float fadeInTime;
    [Range(0, 1), SerializeField] private float fadeInAlpha;
    [SerializeField] private AnimationCurve fadeInCurve;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private AnimationCurve fadeOutCurve;

    private bool isFading;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();

        if (image == null)
        {
            image = GetComponentInChildren<Image>();
        }

        if (image == null)
        {
            Debug.LogError(string.Format("Image fader on {0} has no image to fade!", name));
            return;
        }

        if (image.color.a != 0)
        {
            Color c = image.color;
            c.a = 0;
            image.color = c;
        }
    }

    public void StartFade()
    {
        if (image == null || isFading) return;
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float t = 0f;

        isFading = true;

        Color startingColour = image.color;
        Color endingColour = startingColour;
        endingColour.a = fadeInAlpha;

        while (t <= fadeInTime)
        {
            t += Time.deltaTime;
            image.color = Color.LerpUnclamped(startingColour, endingColour,
                fadeInCurve.Evaluate(t / fadeInTime));
            yield return null;
        }
        image.color = endingColour;

        t = 0f;
        startingColour = image.color;
        endingColour = startingColour;
        endingColour.a = 0f;

        while (t <= fadeOutTime)
        {
            t += Time.deltaTime;
            image.color = Color.LerpUnclamped(startingColour, endingColour,
                fadeOutCurve.Evaluate(t / fadeOutTime));
            yield return null;
        }
        image.color = endingColour;

        isFading = false;
    }
}

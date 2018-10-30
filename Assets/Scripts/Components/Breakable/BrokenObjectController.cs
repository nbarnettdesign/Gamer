using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObjectController : Singleton<BrokenObjectController>
{
    [SerializeField] private float lifeTime;

    [Header("Dissolve Properties")]
    [SerializeField] private float dissolveTime;
    [SerializeField] private string dissolveProperty;
    [SerializeField] private float dissolveMinValue;
    [SerializeField] private float dissolveMaxValue;
    [SerializeField] private AnimationCurve dissolveCurve;

    public void BrokenObjectCreated(GameObject brokenObject)
    {
        StartCoroutine(RemoveObject(brokenObject));
    }

    public void StopDespawn(GameObject brokenObject)
    {
        StopCoroutine(RemoveObject(brokenObject));
    }

    public IEnumerator RemoveObject(GameObject brokenObject)
    {
        float t = 0f;

        List<Renderer> r = new List<Renderer>();

        brokenObject.GetComponentsInChildren(r);

        while (t <= lifeTime - dissolveTime)
        {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;

        if (brokenObject.GetComponent<BrokenObject>() == null)
        {
            Debug.LogError(string.Format("Missing Broken Object script on {0}", brokenObject.name));
            yield break;
        }

        Material dissolveMat = brokenObject.GetComponent<BrokenObject>().DissolveMaterial;

        for (int i = 0; i < r.Count; i++)
        {
            r[i].material = dissolveMat;
            r[i].material.SetFloat(dissolveProperty, dissolveMaxValue);
        }

        while (t <= dissolveTime)
        {
            t += Time.deltaTime;

            for (int i = 0; i < r.Count; i++)
            {
                r[i].material.SetFloat(dissolveProperty,
                    Mathf.Lerp(dissolveMaxValue, dissolveMinValue, dissolveCurve.Evaluate(t / dissolveTime)));
            }

            yield return null;
        }

        SimplePool.Despawn(brokenObject);
    }
}

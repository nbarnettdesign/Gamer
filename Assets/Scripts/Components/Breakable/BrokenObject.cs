using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObject : MonoBehaviour {
    [SerializeField] private Material dissolveMaterial;

    public Material DissolveMaterial { get { return dissolveMaterial; } }

    private List<Material> childMaterials;
    private IEnumerator coroutine;

    private void OnEnable()
    {
        // If child materials is null we know that this object has not been used before
        // so we can just skip this step
        if (childMaterials == null)
            return;

        for (int i = 0; i < childMaterials.Count; i++)
        {
            Renderer r = transform.GetChild(i).GetComponent<Renderer>();

            if (r && r.material != childMaterials[i])
            {
                r.material = childMaterials[i];
            }

            if (transform.GetChild(i).position != Vector3.zero)
                transform.GetChild(i).position = Vector3.zero;
        }

        BrokenObjectController.Instance.BrokenObjectCreated(gameObject);
    }

    private void Start()
    {
        List<Renderer> renderers = new List<Renderer>();
        GetComponentsInChildren(renderers);

        childMaterials = new List<Material>();
        for (int i = 0; i < renderers.Count; i++)
        {
            childMaterials.Add(renderers[i].material);
            transform.GetChild(i).gameObject.AddComponent<BrokenObjectChild>();
        }

        coroutine = BrokenObjectController.Instance.RemoveObject(gameObject);
        StartCoroutine(coroutine);
    }

    public void ChildAttachedToHand()
    {
        StopCoroutine(coroutine);
    }

    public void ChildDetachedFromHand()
    {
        coroutine = BrokenObjectController.Instance.RemoveObject(gameObject);
        StartCoroutine(coroutine);
    }
}

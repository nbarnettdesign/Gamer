using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainHover : Hoverable
{
    [SerializeField] private Material hoverMaterial;

    private Renderer myRenderer;
    private Material originalMaterial;
    private List<ChainHover> otherLinks;

    private bool ropeBroken;

    private void Start()
    {
        myRenderer = GetComponentInChildren<Renderer>();
        originalMaterial = myRenderer.material;
    }

    public override void OnBowHover()
    {
        if (ropeBroken)
            return;

        if (otherLinks == null || otherLinks.Count <= 0)
        {
            otherLinks = new List<ChainHover>();
            transform.parent.GetComponentsInChildren(otherLinks);
            otherLinks.Add(transform.parent.GetComponent<ChainHover>());
        }

        for (int i = 0; i < otherLinks.Count; i++)
        {
            if (otherLinks[i] == null)
                continue;

            otherLinks[i].Hovered();
        }
    }

    public override void OnBowHoverExit()
    {
        if (otherLinks == null || otherLinks.Count <= 0)
            return;

        for (int i = 0; i < otherLinks.Count; i++)
        {
            if (otherLinks[i] == null)
                continue;

            otherLinks[i].HoverExit();
        }
    }

    public void Hovered()
    {
        if (hoverMaterial == null || myRenderer == null)
            return;

        myRenderer.material = hoverMaterial;
    }

    public void HoverExit()
    {
        if (myRenderer == null)
            return;

        myRenderer.material = originalMaterial;
    }

    public void RopeBroke()
    {
        if (ropeBroken)
            return;

        ropeBroken = true;
        HoverExit();
    }
}

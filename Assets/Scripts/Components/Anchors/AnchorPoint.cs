using UnityEngine;
using UnityEngine.Events;

public enum Axis { X, Y, Z }

public class AnchorPoint : Hoverable
{
    [SerializeField] private Material anchoredMaterial;
    [SerializeField] private bool startsLocked;

    public bool IsLocked { get { return isLocked; } }

    protected bool isLocked;

    private BowAnchor bowAnchor;
    private Renderer myRenderer;
    private Material originalMaterial;

    protected virtual void Start()
    {
        if (anchoredMaterial)
        {
            myRenderer = GetComponent<Renderer>();

            if (myRenderer == null)
                myRenderer = GetComponentInChildren<Renderer>();

            if (myRenderer != null)
                originalMaterial = myRenderer.material;
        }

        if (startsLocked)
            isLocked = true;
    }

    public virtual void Anchored()
    {
        if (isLocked)
            return;

        if (bowAnchor == null)
            bowAnchor = FindObjectOfType<BowAnchor>();

        if (myRenderer)
            myRenderer.material = anchoredMaterial;

        bowAnchor.AnchorObject(this);
    }

    public virtual void Unlock()
    {
        if (isLocked)
            isLocked = false;
    }

    public virtual void MoveObject(Vector3 velocity) {}

    public virtual void OnAnchorExit()
    {
        if (myRenderer)
            myRenderer.material = originalMaterial;
    }

    public override void OnBowHover()
    {
        if (anchoredMaterial && isLocked == false)
            myRenderer.material = anchoredMaterial;
    }

    public override void OnBowHoverExit()
    {
        if (anchoredMaterial && isLocked == false)
            myRenderer.material = originalMaterial;
    }

    protected virtual void BreakAnchor()
    {
        bowAnchor.BreakAnchor();
        OnBowHoverExit();
        OnAnchorExit();
    }
}

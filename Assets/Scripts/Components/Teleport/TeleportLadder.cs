using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class TeleportLadder : TeleportMarkerBase
{
    [SerializeField] private Transform ladderTop;
    [SerializeField] private Transform ladderBottom;

    private Player player;

    private MeshRenderer areaMesh;
    private Color visibleTintColor = Color.clear;
    private Color highlightedTintColor = Color.clear;
    private Color lockedTintColor = Color.clear;
    private int tintColorId = 0;
    private bool highlighted = false;

    private void Awake()
    {
        areaMesh = GetComponent<MeshRenderer>();
        tintColorId = Shader.PropertyToID("_TintColor");
        player = Player.Instance;
    }

    public override void Highlight(bool highlight)
    {
        if (!locked)
        {
            highlighted = highlight;

            if (areaMesh == null)
                return;

            if (highlight)
                areaMesh.material = Teleport.instance.areaHighlightedMaterial;
            else
                areaMesh.material = Teleport.instance.areaVisibleMaterial;
        }
    }

    public override void TeleportPlayer(Vector3 pointedAtPosition)
    {
        if (player.trackingOriginTransform.position.y <= transform.position.y)
            player.trackingOriginTransform.position = ladderTop.position;
        else
            player.trackingOriginTransform.position = ladderBottom.position;

    }

    public override void SetAlpha(float tintAlpha, float alphaPercent)
    {
        Color tintedColor = GetTintColor();
        tintedColor.a *= alphaPercent;

        if (areaMesh != null)
            areaMesh.material.SetColor(tintColorId, tintedColor);
    }

    public override bool ShouldActivate(Vector3 playerPosition)
    {
        return true;
    }

    public override bool ShouldMovePlayer()
    {
        return false;
    }

    public override void UpdateVisuals()
    {
        Debug.Log("Do the visual things");
    }

    private Color GetTintColor()
    {
        if (locked)
            return lockedTintColor;
        else
        {
            if (highlighted)
                return highlightedTintColor;
            else
                return visibleTintColor;
        }
    }
}

using UnityEngine;

public class GizmoCube : MonoBehaviour {
    [SerializeField] private Color colour;
    [SerializeField] private bool selectedOnly;
    [SerializeField] private bool fromCollider;

    private void OnDrawGizmos()
    {
        if (selectedOnly || Application.isPlaying) return;

        DrawGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (selectedOnly == false || Application.isPlaying) return;

        DrawGizmo();
    }

    private void DrawGizmo()
    {
        if (Gizmos.color != colour)
        {
            Gizmos.color = colour;
        }

        Gizmos.DrawCube(fromCollider && GetComponent<BoxCollider>() ? transform.position + GetComponent<BoxCollider>().center : transform.position, fromCollider && GetComponent<BoxCollider>() ? GetComponent<BoxCollider>().size: transform.localScale);
    }
}

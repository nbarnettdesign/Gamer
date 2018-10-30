using UnityEngine;

public class GizmoWireCube : MonoBehaviour
{
    [SerializeField] private Color colour;
    [SerializeField] private bool selectedOnly;

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

        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}

using UnityEngine;

public class GizmoSphere : MonoBehaviour
{
    [SerializeField] private float radius;
    [SerializeField] private bool radiusFromCollider;
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

        Gizmos.DrawSphere(transform.position,
            radiusFromCollider && GetComponent<SphereCollider>() ? GetComponent<SphereCollider>().radius : radius);
    }
}

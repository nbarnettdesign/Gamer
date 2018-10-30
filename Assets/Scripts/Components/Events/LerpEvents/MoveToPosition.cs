using UnityEngine;

public class MoveToPosition : MonoBehaviour
{
    [SerializeField] private Vector3 finalPosition;
    [SerializeField] private float moveTime;
    [SerializeField] private AnimationCurve moveCurve;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position;
    }

    public void BeginMove()
    {
        LerpController.Instance.LerpToPosition(transform, finalPosition, moveTime, moveCurve);
    }

    public void Return()
    {
        LerpController.Instance.LerpToPosition(transform, originalPosition, moveTime, moveCurve);
    }
}

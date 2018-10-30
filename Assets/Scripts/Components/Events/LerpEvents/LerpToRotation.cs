using UnityEngine;

public class LerpToRotation : MonoBehaviour
{
    [SerializeField] private Vector3 finalRotation;
    [SerializeField] private float rotationTime;
    [SerializeField] private AnimationCurve rotationCurve;

    public void BeginRotation()
    {
        LerpController.Instance.LerpToRotation(transform, finalRotation, rotationTime, rotationCurve);
    }

    public void Return()
    {
        LerpController.Instance.LerpToRotation(transform, -finalRotation, rotationTime, rotationCurve);
    }
}

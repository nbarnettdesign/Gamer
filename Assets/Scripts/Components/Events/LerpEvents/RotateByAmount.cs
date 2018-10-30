using UnityEngine;

public class RotateByAmount : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAmount;
    [SerializeField] private float rotationTime;
    [SerializeField] private AnimationCurve rotationCurve;

    public void BeginRotation()
    {
        LerpController.Instance.RotateByAmount(transform, rotationAmount, rotationTime, rotationCurve);
    }

    public void Return()
    {
        LerpController.Instance.RotateByAmount(transform, -rotationAmount, rotationTime, rotationCurve);
    }
}

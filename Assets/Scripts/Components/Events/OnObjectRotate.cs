using UnityEngine;
using UnityEngine.Events;

public enum RotationAxis { X, Y, Z }

public class OnObjectRotate : MonoBehaviourExtended
{
    [SerializeField] private RotationAxis axis;
    [SerializeField] private UnityEvent onRotationIncrease;
    [SerializeField] private UnityEvent onRotationDescrese;

    private Vector3 previousRotation;

    protected override void Start()
    {
        base.Start();
        previousRotation = transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        if (IsRendering() == false)
            return;

        CheckRotation();
    }

    private void CheckRotation()
    {
        Vector3 currentRotation = transform.localRotation.eulerAngles;

        if (currentRotation != previousRotation)
        {
            switch (axis)
            {
                case RotationAxis.X:
                    if (currentRotation.x == previousRotation.x)
                        break;

                    if (currentRotation.x > previousRotation.x)
                        onRotationIncrease.Invoke();
                    else
                        onRotationDescrese.Invoke();

                    break;
                case RotationAxis.Y:
                    if (currentRotation.y == previousRotation.y)
                        break;

                    if (currentRotation.y > previousRotation.y)
                        onRotationIncrease.Invoke();
                    else
                        onRotationDescrese.Invoke();

                    break;
                case RotationAxis.Z:
                    if (currentRotation.z == previousRotation.z)
                        break;

                    if (currentRotation.z > previousRotation.z)
                        onRotationIncrease.Invoke();
                    else
                        onRotationDescrese.Invoke();

                    break;
            }

            previousRotation = currentRotation;
        }
    }
}

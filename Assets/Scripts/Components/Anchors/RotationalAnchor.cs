using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RotationalAnchor : AnchorPoint
{
    [SerializeField] private Axis rotationAxis;
    [Tooltip("The total amount of rotation that can be removed from this object, if 0 min rotation becomes transform.rotation")]
    [SerializeField] private float minRotation;
    [SerializeField] private bool locksAtMin;
    [Tooltip("The total amount of rotation that can be added from this object, if 0 max rotation becomes transform.rotation")]
    [SerializeField] private float maxRotation;
    [SerializeField] private bool locksAtMax;
    [SerializeField] private float moveSpeed;

    [Header("Events")]
    [SerializeField] private UnityEvent onMinPosition;
    [SerializeField] private UnityEvent onMaxPosition;

    private Quaternion minQuaternion;
    private Quaternion maxQuaternion;

    protected override void Start()
    {
        base.Start();

        minQuaternion = transform.rotation;
        maxQuaternion = transform.rotation;

        switch (rotationAxis)
        {
            case Axis.X:
                if (minRotation != 0)
                    minQuaternion = Quaternion.Euler(transform.eulerAngles + new Vector3(minRotation, 0, 0));
                if (maxRotation != 0)
                    maxQuaternion = Quaternion.Euler(transform.eulerAngles + new Vector3(maxRotation, 0, 0));
                break;
            case Axis.Y:
                if (minRotation != 0)
                    minQuaternion = Quaternion.Euler(transform.eulerAngles + new Vector3(0, minRotation, 0));
                if (maxRotation != 0)
                    maxQuaternion = Quaternion.Euler(transform.eulerAngles + new Vector3(0, maxRotation, 0));
                break;
            case Axis.Z:
                if (minRotation != 0)
                    minQuaternion = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, minRotation));
                if (maxRotation != 0)
                    maxQuaternion = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, maxRotation));
                break;
            default:
                break;
        }
    }

    public override void MoveObject(Vector3 velocity)
    {
        if (isLocked)
            return;

        velocity = velocity.normalized;
        switch (rotationAxis)
        {
            case Axis.X:
                if (velocity.z == 0)
                    return;

                transform.rotation *= Quaternion.Euler(velocity.z * moveSpeed * Time.deltaTime, 0f, 0f);

                if (transform.eulerAngles.x <= minQuaternion.eulerAngles.x)
                {
                    transform.rotation = minQuaternion;
                    onMinPosition.Invoke();

                    if (locksAtMin)
                    {
                        BreakAnchor();
                        isLocked = true;
                    }
                }
                else if (transform.eulerAngles.x >= maxQuaternion.eulerAngles.x)
                {
                    transform.rotation = maxQuaternion;
                    onMaxPosition.Invoke();

                    if (locksAtMax)
                    {
                        BreakAnchor();
                        isLocked = true;
                    }
                }

                break;
            case Axis.Y:
                if (velocity.z == 0)
                    return;

                transform.rotation *= Quaternion.Euler(0f, velocity.z * moveSpeed * Time.deltaTime, 0f);

                if (transform.eulerAngles.y <= minQuaternion.eulerAngles.y)
                {
                    transform.rotation = minQuaternion;
                    onMinPosition.Invoke();

                    if (locksAtMin)
                    {
                        BreakAnchor();
                        isLocked = true;
                    }
                }
                else if (transform.eulerAngles.y >= maxQuaternion.eulerAngles.y)
                {
                    transform.rotation = maxQuaternion;
                    onMaxPosition.Invoke();

                    if (locksAtMax)
                    {
                        BreakAnchor();
                        isLocked = true;
                    }
                }

                break;
            case Axis.Z:
                if (velocity.z == 0)
                    return;

                transform.rotation *= Quaternion.Euler(0f, 0f, velocity.z * moveSpeed * Time.deltaTime);

                if (transform.eulerAngles.z <= minQuaternion.eulerAngles.z)
                {
                    transform.rotation = minQuaternion;
                    onMinPosition.Invoke();

                    if (locksAtMin)
                    {
                        BreakAnchor();
                        isLocked = true;
                    }
                }
                else if (transform.eulerAngles.z >= maxQuaternion.eulerAngles.z)
                {
                    transform.rotation = maxQuaternion;
                    onMaxPosition.Invoke();

                    if (locksAtMax)
                    {
                        BreakAnchor();
                        isLocked = true;
                    }
                }

                break;
            default:
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowards : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float speed;

    private void Update()
    {
        Vector3 eulerAngle = Quaternion.RotateTowards(transform.localRotation, target.localRotation,
            speed * Time.deltaTime).eulerAngles;
        eulerAngle.z = 0f;

        transform.localRotation = Quaternion.Euler(eulerAngle);
    }
}

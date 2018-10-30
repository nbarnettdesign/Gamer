using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DropObject : MonoBehaviour
{
    private Rigidbody rBody;

    private void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public void Drop()
    {
        transform.SetParent(null);

        if (rBody != null && rBody.isKinematic)
        {
            rBody.isKinematic = false;
        }
    }
}

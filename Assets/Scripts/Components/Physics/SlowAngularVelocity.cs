using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowAngularVelocity : MonoBehaviourExtended
{
    [SerializeField] private float delayTime;
    [SerializeField] private float lossPerSecond;

    private Rigidbody rbody;
    private bool isSlowing;
    private float delayCount;

    protected override void Start()
    {
        base.Start();
        rbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (IsRendering() == false || rbody == null || isSlowing == false)
            return;

        Slow();
    }

    private void Slow()
    {
        if (delayCount < delayTime) {
            delayCount += Time.deltaTime;
            return;
        }

        if (rbody.angularVelocity.sqrMagnitude > (lossPerSecond * Time.deltaTime) * (lossPerSecond * Time.deltaTime))
            rbody.AddTorque(-rbody.angularVelocity.normalized * lossPerSecond, ForceMode.Force);
        else {
            rbody.angularVelocity = Vector3.zero;
            isSlowing = false;
            delayCount = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isSlowing)
            return;

        isSlowing = true;
    }
}

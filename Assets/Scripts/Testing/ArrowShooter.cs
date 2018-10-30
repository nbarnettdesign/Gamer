using UnityEngine;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;

public class ArrowShooter : MonoBehaviourExtended
{
    [SerializeField] protected List<GameObject> arrows;
    [SerializeField] protected Transform arrowSpawnTransform;
    [SerializeField] protected float maxArrowForce;
    [SerializeField] protected float forcePerSecond;

    protected GameObject currentArrow;
    protected GameObject nockedArrow;
    private bool hasFired;
    private int arrowIndex;

    private float currentForce;

    protected override void Start()
    {
        base.Start();

        if (arrows.Count > 0)
        {
            NextArrow();
        }
        else
        {
            Debug.LogError(string.Format("{0} needs arrows so it can fire!", name));
        }

        if (arrowSpawnTransform == null)
        {
            arrowSpawnTransform = transform;
        }
    }

    protected virtual void Update()
    {
        if (IsRendering() == false)
            return;

        if (nockedArrow != null && hasFired == false)
        {
            if (nockedArrow.transform.position != arrowSpawnTransform.position)
                nockedArrow.transform.position = arrowSpawnTransform.position;
        }
    }

    protected virtual void NextArrow()
    {
        currentArrow = arrows[arrowIndex];
        arrowIndex++;

        if (arrowIndex >= arrows.Count)
        {
            arrowIndex = 0;
        }
    }

    protected virtual void ArrowSpawn()
    {
        nockedArrow = Instantiate(currentArrow, arrowSpawnTransform.position, arrowSpawnTransform.rotation);

        nockedArrow.transform.SetParent(arrowSpawnTransform);
        Util.ResetTransform(nockedArrow.transform);
        hasFired = false;
    }

    protected virtual void ArrowCharge()
    {
        currentForce += forcePerSecond * Time.deltaTime;

        if (currentForce > maxArrowForce)
        {
            currentForce = maxArrowForce;
        }
    }

    protected virtual void ArrowFire()
    {
        Arrow arrow = nockedArrow.GetComponent<Arrow>();
        arrow.ShaftRB.isKinematic = false;
        arrow.ShaftRB.useGravity = true;
        arrow.ShaftRB.transform.GetComponent<BoxCollider>().enabled = true;

        arrow.ArrowHeadRB.isKinematic = false;
        arrow.ArrowHeadRB.useGravity = true;
        arrow.ArrowHeadRB.transform.GetComponent<BoxCollider>().enabled = true;

        arrow.ArrowHeadRB.AddForce(arrowSpawnTransform.transform.forward * currentForce, ForceMode.VelocityChange);
        arrow.ArrowHeadRB.AddTorque(arrowSpawnTransform.transform.forward * currentForce);

        arrow.ArrowReleased(currentForce);
        nockedArrow.transform.SetParent(null);

        currentForce = 0f;
        hasFired = true;
    }
}

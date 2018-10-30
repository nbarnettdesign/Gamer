using UnityEngine;

public class ExplodeableEffector : Effector
{
    [SerializeField] private bool disable;
    [SerializeField] private bool explodeOnEnter;

    private void OnTriggerEnter(Collider other)
    {
        Explodeable e = other.GetComponent<Explodeable>();

        if (e == null || Physics.Linecast(transform.position, e.transform.position, environmentLayer)) return;

        if (explodeOnEnter)
        {
            e.ForceExplode();
        }

        if (disable)
        {
            e.enabled = false;
        }
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (disable)
        {
            if (explodeOnEnter)
                explodeOnEnter = false;
        }
        if (explodeOnEnter)
        {
            if (disable)
                explodeOnEnter = false;
        }

    }
#endif
}

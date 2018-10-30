using UnityEngine;
using Valve.VR.InteractionSystem;

public class FireSourceEffector : Effector
{
    [Header("Fire Source Settings")]
    [SerializeField] private bool setAlight;
    [SerializeField] private bool extinguish;
    [SerializeField] private FireStrength effectorFireStrength;

    private void OnTriggerEnter(Collider other)
    {
        FireSource fireSource = other.GetComponent<FireSource>();

        if (fireSource == null)
        {
            fireSource = other.GetComponentInChildren<FireSource>();
        }

        //Make sure we have a fire source and aren't going through walls
        if (fireSource == null || Physics.Linecast(transform.position, fireSource.transform.position, environmentLayer))
            return;

        EffectFireSource(fireSource);
    }

    protected virtual void EffectFireSource(FireSource fireSource)
    {
        if (setAlight && fireSource.IsBurning == false) 
        {
            fireSource.FireExposure(effectorFireStrength);
        }
        else if (extinguish && fireSource.IsBurning == true)
            fireSource.Extinguish();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (setAlight)
        {
            if (extinguish)
            {
                extinguish = false;
            }
            else if (extinguish)
            {
                if (setAlight)
                {
                    setAlight = false;
                }
            }
        }
            
    }
#endif
}

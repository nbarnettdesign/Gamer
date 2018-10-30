using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

public class EventItemPackage : ItemPackage
{
    [Header("Events")]
    [SerializeField] private UnityEvent onItemPickup;

    protected override void OnItemPickup()
    {
        onItemPickup.Invoke();
    }
}
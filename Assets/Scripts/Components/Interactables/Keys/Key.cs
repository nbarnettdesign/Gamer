using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Throwable))]
public class Key : MonoBehaviour
{
    [SerializeField] private KeyShape keyShape;

    public KeyShape KeyShape { get { return keyShape; } }
}

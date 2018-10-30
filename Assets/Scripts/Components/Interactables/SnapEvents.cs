using UnityEngine;
using UnityEngine.Events;

public class SnapEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent onSnapEnter;
    [SerializeField] private UnityEvent onSnapExit;

    public void SnapEnter()
    {
        onSnapEnter.Invoke();
    }

    public void SnapExit()
    {
        onSnapEnter.Invoke();
    }
}

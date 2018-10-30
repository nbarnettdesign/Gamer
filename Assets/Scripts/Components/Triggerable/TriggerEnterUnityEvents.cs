using UnityEngine;
using UnityEngine.Events;

public class TriggerEnterUnityEvents : MonoBehaviour
{
    [SerializeField] private string triggerTag;
    [SerializeField] private float onEnterCoolDown;
    [SerializeField] private bool singleTriggerEnter;
    [SerializeField] private UnityEvent onTriggerEnter;

    private bool hasEntered;

    private void Start()
    {
        if (GetComponent<Collider>() != null && GetComponent<Collider>().isTrigger == false)
            GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasEntered || string.IsNullOrEmpty(triggerTag) == false && triggerTag != other.tag) return;

        hasEntered = true;

        onTriggerEnter.Invoke();

        if (singleTriggerEnter == false)
            this.Invoke(CanTriggerEnter, onEnterCoolDown);
        else
            Destroy(this);
    }

    private void CanTriggerEnter()
    {
        hasEntered = false;
    }
}

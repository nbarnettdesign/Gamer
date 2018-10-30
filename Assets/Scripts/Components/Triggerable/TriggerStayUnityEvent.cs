using UnityEngine;
using UnityEngine.Events;

public class TriggerStayUnityEvent : MonoBehaviour {
    [SerializeField] private string triggerTag;
    [SerializeField] private float onStayCoolDown;
    [SerializeField] private bool singleTriggerStay;
    [SerializeField] private UnityEvent onTriggerStay;

    private bool hasStayed;

    private void Start()
    {
        if (GetComponent<Collider>() != null && GetComponent<Collider>().isTrigger == false)
            GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (hasStayed || string.IsNullOrEmpty(triggerTag) == false && triggerTag != other.tag) return;

        hasStayed = true;
        onTriggerStay.Invoke();

        if (singleTriggerStay == false)
            Invoke("CanTriggerStay", onStayCoolDown);
        else
            Destroy(this);
    }

    private void CanTriggerStay()
    {
        hasStayed = false;
    }
}

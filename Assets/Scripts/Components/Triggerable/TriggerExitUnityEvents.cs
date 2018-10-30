using UnityEngine;
using UnityEngine.Events;

public class TriggerExitUnityEvents : MonoBehaviour {
    [SerializeField] private string triggerTag;
    [SerializeField] private float onExitCoolDown;
    [SerializeField] private bool singleTriggerExit;
    [SerializeField] private UnityEvent onTriggerExit;

    private bool hasExited;

    private void OnTriggerExit(Collider other)
    {
        if (hasExited || string.IsNullOrEmpty(triggerTag) == false && triggerTag != other.tag) return;

        hasExited = true;

        onTriggerExit.Invoke();

        if (singleTriggerExit == false)
            this.Invoke(CanTriggerExit, onExitCoolDown);
        else
            Destroy(this);
    }

    private void CanTriggerExit()
    {
        hasExited = false;
    }
}

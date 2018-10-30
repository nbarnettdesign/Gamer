using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class Lock : SnapObject
{
    [SerializeField] private KeyShape lockShape;
    [SerializeField] private GameObject key;
    [SerializeField] private UnityEvent onKeyRemoved;
    [SerializeField] private UnityEvent onKeyInserted;

    public override void Detach(GameObject obj)
    {
        base.Detach(obj);
        onKeyRemoved.Invoke();
    }

    protected override void SnapTo(Throwable t)
    {
        if (key != null)
        {
            if (key == t.gameObject)
            {
                base.SnapTo(t);
                onKeyInserted.Invoke();
            }
        }
        else
        {
            Key k = t.GetComponent<Key>();

            if (k == null)
                return;

            if (k.KeyShape == lockShape)
            {
                base.SnapTo(t);
                onKeyInserted.Invoke();
            }
        }
    }
}

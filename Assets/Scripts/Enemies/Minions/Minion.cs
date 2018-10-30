using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class Minion : MonoBehaviour
{
    [SerializeField] protected float homingSensitivity;
    [SerializeField] protected float speed;
    [SerializeField] protected float damage;
    [SerializeField] protected Vector3 offset;

    protected Transform target;
    protected Damageable damageable;

    protected virtual void Start()
    {
        damageable = GetComponent<Damageable>();
    }

    public void GiveTarget(Transform target)
    {
        this.target = target.GetComponentInChildren<DamageablePlayer>().transform;
    }

    protected void DespawnObject()
    {
        damageable.Damage(1, transform.position, Vector3.zero, false, true);
    }
}

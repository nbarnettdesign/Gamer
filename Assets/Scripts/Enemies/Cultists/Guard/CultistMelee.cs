using BehaviorDesigner.Runtime;
using UnityEngine;

public class CultistMelee : MonoBehaviourExtended
{
    [SerializeField] private float damage;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float range;

    public bool HasInitialised { get { return hasInitialised; } }

    private bool hasInitialised;
    private CultistGuard guard;
    private Affectable target;

    private BehaviorTree behaviorTree;
    private Transform targetTransform;

    protected override void Start()
    {
        base.Start();
        target = GameController.Instance.Player.trackingOriginTransform.GetComponentInChildren<Affectable>();

        behaviorTree = transform.parent.GetComponent<BehaviorTree>();
    }

    public void Attack()
    {
        if (target == null)
            return;

        if ((target.transform.position - transform.position).sqrMagnitude <= range * range)
        {
            target.Damage(damage, transform.position, transform.position);
        }
    }
}

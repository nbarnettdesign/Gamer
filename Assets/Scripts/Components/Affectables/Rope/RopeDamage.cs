using UnityEngine;

public class RopeDamage : Affectable {
    private RopeParent rope;

    protected override void Start() {
        base.Start();
        rope = transform.parent.GetComponent<RopeParent>(); 
    }

    public override void Damage(float amount, Vector3 impactPosition, Vector3 impactVelocity, bool fromExplosion = false, bool instaKill = false) {
        if (rope == false)
            return;

        rope.RopeBroken();
    }
}

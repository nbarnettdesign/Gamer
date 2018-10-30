using UnityEngine;

public class StunImpactable : Affectable {
    [SerializeField] private ImpactSettings impactSettings;
    [SerializeField] private bool alsoDamage;
    [SerializeField] private bool onlyFromAbove;

    public override void Impacted(ObjectWeight weight, Vector3 impactPoint, Vector3 impactVelocity) {
        if (onlyFromAbove && impactPoint.y <= transform.position.y)
            return;

        ImpactSetting setting = impactSettings.Find(weight);

        if (setting.weight != weight)
            return;

        // This can probably be turned more generic at some point
        // but for now this script is only being used for the charge boss so...
        if (impactSettings.Find(weight).stunTime > 0)
            GetComponent<ChargingBoss>().Stun(impactSettings.Find(weight).stunTime);

        if (alsoDamage && impactSettings.Find(weight).damage > 0)
            GetComponent<DamageableParent>().Damage(impactSettings.Find(weight).damage);
    }
}

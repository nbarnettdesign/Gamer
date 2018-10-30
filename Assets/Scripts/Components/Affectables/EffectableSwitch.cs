using UnityEngine;
using UnityEngine.Events;

public class AffectableSwitch : Affectable {
	[SerializeField] private UnityEvent onSwitchOn;
	[SerializeField] private UnityEvent onSwitchOff;

	private bool isOn;

	public override void Damage(float amount, Vector3 impactPosition, Vector3 impactVelocity, bool fromExplosion = false, bool instaKill = false) {
		if (ExplosionCheck(fromExplosion))
			return;

		Toggle();
	}

	private void Toggle() {
		isOn = !isOn;

		if (isOn)
        {
			onSwitchOn.Invoke();
        }
        else
        {
			onSwitchOff.Invoke();
        }
	}
}

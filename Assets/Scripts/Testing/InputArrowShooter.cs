using UnityEngine;

public class InputArrowShooter : ArrowShooter {
	[SerializeField] private KeyCode fire;
	[SerializeField] private KeyCode nextArrow;

	protected override void Update () {
		base.Update();

		if (Input.GetKeyDown(nextArrow))
        {
			NextArrow();
        }

		if (Input.GetKeyDown(fire))
        {
			ArrowSpawn();
        }
		else if (Input.GetKey(fire))
        {
			ArrowCharge();
        }
		else if (Input.GetKeyUp(fire))
        {
			ArrowFire();
        }
	}
}

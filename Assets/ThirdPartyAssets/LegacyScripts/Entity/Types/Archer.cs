using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Entity {
	[Header("Archer Options")]
	[SerializeField] private float attackTime;
	[SerializeField] private LayerMask checkLayers;

//	private Vector3 targetLocation;
//	private bool shouldAttack;
	private bool canAttack;
	private float currentAttackTime;

	protected override void Start() {
		base.Start();
	}

	private void Update() {
		UpdateAttackTime();

		if (CanAttackTarget())
			Attack();
	}

	public void StartAttack(Vector3 targetLocation) {
		//shouldAttack = true;
		//this.targetLocation = targetLocation;
	}

	public void StopAttack() {
		//shouldAttack = false;
	}

	private bool CanAttackTarget() {
		return false;
	}

	private void Attack() {
		canAttack = false;
	}

	private void UpdateAttackTime() {
		if (canAttack)
			return;

		currentAttackTime += Time.deltaTime;

		if(currentAttackTime >= attackTime) {
			canAttack = true;
			currentAttackTime = 0f;
		}
	}
}

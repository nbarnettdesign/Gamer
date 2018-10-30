using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EntityWatcher : MonoBehaviour {
	[SerializeField] private Entity entityToWatch;

	private Rigidbody rbody;

	void Start () {
		rbody = GetComponent<Rigidbody>();
	}

	private void Update() {
		if (entityToWatch == null && rbody.isKinematic)
			rbody.isKinematic = false;
	}
}

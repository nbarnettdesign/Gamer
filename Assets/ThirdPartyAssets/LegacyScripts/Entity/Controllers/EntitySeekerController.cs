using UnityEngine;

public class EntitySeekerController : Singleton<EntitySeekerController> {
	[SerializeField] private float targetRecheckTime;

	public float TargetRecheckTime { get { return targetRecheckTime; } }
}

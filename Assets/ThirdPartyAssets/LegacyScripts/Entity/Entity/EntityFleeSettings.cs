using UnityEngine;

[CreateAssetMenu(fileName = "Wander Settings", menuName = "Entity Settings/Flee", order = 2)]
public class EntityFleeSettings : ScriptableObject {
	[SerializeField] private float fleeDistance;
	[SerializeField] private float fleeSpeed;
	[SerializeField] private float dangerDetectionRadius;
	[SerializeField] private LayerMask dangerousLayers;

	public float FleeDistance { get { return fleeDistance; } }
	public float FleeSpeed { get { return fleeSpeed; } }
	public float DangerDetectionRadius { get { return dangerDetectionRadius; } }
	public LayerMask DangerousLayers { get { return dangerousLayers; } }
}

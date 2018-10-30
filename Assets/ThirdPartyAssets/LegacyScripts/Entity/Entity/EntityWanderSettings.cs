using UnityEngine;

[CreateAssetMenu(fileName = "Wander Settings", menuName = "Entity Settings/Wander", order = 2)]
public class EntityWanderSettings : ScriptableObject {
	[SerializeField] private float minWalkTime;
	[SerializeField] private float maxWalkTime;
	[Tooltip("The maximum radius an entity will look when seeking a new destination")]
	[SerializeField] private float newDestinationRadius;

	public float MinWalkTime { get { return minWalkTime; } }
	public float MaxWalkTime { get { return maxWalkTime; } }
	public float NewDestinationRadius { get { return newDestinationRadius; } }
}

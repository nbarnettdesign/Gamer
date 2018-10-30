using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[System.Serializable]
public struct TargetInfo {
	[SerializeField] private List<Entity> targets;
	[SerializeField] private TeleportPoint point;

	public List<Entity> Targets { get { return targets; } }
	public TeleportPoint Point { get { return point; } }
}

[RequireComponent(typeof(EntityTypeController))]
public class EntityController : Singleton<EntityController> {
    [SerializeField] private List<int> targetNumbers;
	[SerializeField] private List<TargetInfo> targetInfo; 

    [Header("Target Finding")]
    [SerializeField] private string playerTag;
    [SerializeField] private string teleportTag;

	public Transform SeekerTarget { get { return SpawnController.Player; } }
	public EntitySpawnController SpawnController { get; private set; }

	private int currentNumOfTargets;
    private int progressCount;

    private List<Entity> possibleTargets = new List<Entity>();

    private void Awake() {
        SpawnController = new EntitySpawnController(playerTag, teleportTag);
    }

	public void AddToPossibleTargetList(Entity _entityToAdd) {
        possibleTargets.Add(_entityToAdd);
    }

    public Vector3 GetSeekerTargetPosition () {
        return SpawnController.GetClosestTeleportPointToPlayer();
    }

    public void EntityKilled(Entity entity) {
		//This is probably bad but should probably work
		foreach (TargetInfo info in targetInfo) {
			//If the entity exists in this particular info
			if(info.Targets.Exists(t => t != null && t == entity)) {
				//Remove it from the list
				info.Targets.Remove(entity);

				//Check through this particular info and check if every target is null
				if (info.Targets.Exists(t => t != null) == false) {
					//If they are all null unlock the teleport point
					info.Point.SetLocked(false);
				}
				break;
			}

		}
    }
}

public class EntityStateController : Singleton<EntityStateController> {
    public IEntityState GetEntityState (EntityState state) {
        switch (state) {
            case EntityState.Idle:
                return new EntityIdle();
            case EntityState.Alert:
                return new EntityAlert();
            case EntityState.ReturnToOrigin:
                return new EntityReturnToOrigin();
            case EntityState.MoveToTarget:
                return new EntityMoveToTarget();
            case EntityState.Wander:
                return new EntityMoveRandom();
            case EntityState.Flee:
                return new EntityFlee();
			case EntityState.Patrol:
				return new EntityPatrol();
			case EntityState.SeekPlayer:
				return new EntitySeekPlayer();
            case EntityState.SeekTarget:
                return new EntitySeekTarget();
            default:
                return new EntityMoveRandom();
        }
    }
}

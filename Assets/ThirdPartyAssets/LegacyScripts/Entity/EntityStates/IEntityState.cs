public interface IEntityState {
    void OnStateEnter (MoveableEntity entity);
    void OnStateUpdate (MoveableEntity entity);
    void OnStateExit (MoveableEntity entity);
}

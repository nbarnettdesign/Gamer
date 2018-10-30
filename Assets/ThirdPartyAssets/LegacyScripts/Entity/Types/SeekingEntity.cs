using UnityEngine;

public class SeekingEntity : MoveableEntity {
    [Header("Seeking Options")]
    [SerializeField] private bool staticTarget;

    [Header("Movement Options")]
    [SerializeField] private float stopTime;
    [SerializeField] private float moveTime;
    public bool StaticTarget { get { return staticTarget; } }
    public float StopTime { get { return stopTime; } }
    public float MoveTime { get { return moveTime; } }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSpawnPoint : MonoBehaviour {
    [SerializeField] private float doorOpenTime;

    public float DoorOpenTime { get { return doorOpenTime; } }

    private MoveByAmount door;

    private void Start()
    {
        door = GetComponentInChildren<MoveByAmount>();
    }

    public void OpenDoor()
    {
        if (door == null)
            return;

        door.BeginMove(doorOpenTime);
    }
}

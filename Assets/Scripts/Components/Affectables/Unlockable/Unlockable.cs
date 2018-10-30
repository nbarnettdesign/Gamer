using UnityEngine;
using UnityEngine.Events;

public class Unlockable : MonoBehaviour {
	[SerializeField] private bool lockNumberFromChildren = true;
	[SerializeField] private int lockNumber;

    [Header("Locking")]
    [SerializeField] private bool startLocked;
    [SerializeField] private bool needsMultipleLocks;
    [SerializeField] private bool preventRelocking;

    [Header("Events")]
	[SerializeField] private UnityEvent onDoorUnlock;
	[SerializeField] private UnityEvent onDoorLock;

    private int startingLockNumber;
	private int currentLockNumber;
    private bool unlocked;

	protected virtual void Start () {
		if (lockNumberFromChildren)
			currentLockNumber = transform.GetComponentsInChildren<Affectable>().Length;
		else
			currentLockNumber = lockNumber;

        startingLockNumber = currentLockNumber;

        if (startLocked == false)
        {
            currentLockNumber = 0;
            unlocked = true;
        }
	}

    public virtual void Lock()
    {
        if (unlocked && preventRelocking)
            return;

        if (unlocked && needsMultipleLocks == false)
        {
            onDoorLock.Invoke();
            unlocked = false;
        }

        currentLockNumber++;

        if(needsMultipleLocks && currentLockNumber >= startingLockNumber)
        {
            onDoorLock.Invoke();
            unlocked = false;
        }

        if (currentLockNumber > startingLockNumber)
            currentLockNumber = startingLockNumber;
    }

	public virtual void UnlockLock() {
		currentLockNumber--;
		if (currentLockNumber <= 0)
			Unlock();
	}

	protected virtual void Unlock() {
        if (unlocked)
            return;

		onDoorUnlock.Invoke();
        unlocked = true;
	}
}

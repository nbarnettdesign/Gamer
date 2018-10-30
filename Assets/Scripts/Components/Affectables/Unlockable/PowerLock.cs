using UnityEngine;

public class PowerLock : MonoBehaviour {

    private BoxCollider interactableCollider;
    private Material wallMaterial;

    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject powerLock;

    [SerializeField] private Material dissolveMaterial;

    public void UnlockDoor()
    {
        Debug.Log("Something");
        //wall.DissolveMaterial start coroutine
        wall.SetActive(false);
        powerLock.SetActive(false);
        //powerLock.GetComponent<Rigidbody>().isKinematic = false;
    }
}

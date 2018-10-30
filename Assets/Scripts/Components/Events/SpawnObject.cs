using UnityEngine;

public class SpawnObject : MonoBehaviour {

    [SerializeField] private GameObject objectToSpawn;

    private void Awake()
    {
        if (objectToSpawn != null)
            objectToSpawn.SetActive(false);
    }

    public void Spawn()
    {
        SimplePool.Spawn(objectToSpawn, transform.position, transform.rotation);
    }

    public void ActivateObject()
    {
        objectToSpawn.SetActive(true);
    }
}

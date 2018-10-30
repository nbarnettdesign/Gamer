using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Object on Explode", menuName = "Interactables/Explode Effects/Spawn Object", order = 4)]
public class SpawnObjectExplodeEffect : ExplodeEffect
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private int numberToSpawn;
    [SerializeField] private bool usePrefabRotation;
    [SerializeField] private bool applyExplosionForce;
    [SerializeField] private float explosionForce;

    public override void Trigger(Vector3 location, float radius)
    {
        List<GameObject> spawnedObjects = new List<GameObject>();

        for (int i = 0; i < numberToSpawn; i++)
        {
            spawnedObjects.Add(SimplePool.Spawn(objectToSpawn, location + Random.insideUnitSphere * radius,
                usePrefabRotation ? objectToSpawn.transform.rotation : Quaternion.identity));
        }

        if (applyExplosionForce == false) return;

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i].GetComponent<Rigidbody>())
                spawnedObjects[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, location, radius);
        }
    }
}

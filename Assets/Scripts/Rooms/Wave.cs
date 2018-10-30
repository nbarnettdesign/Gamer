using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Enemies/Spawner/Wave", order = 5)]
public class Wave : ScriptableObject {
    [SerializeField] private List<GameObject> spawnObjects;
    [SerializeField] private bool randomisedObjects;

    private int objectNum;

    private void Awake()
    {
        objectNum = 0;

        if (randomisedObjects)
            spawnObjects.Shuffle();
    }

    public GameObject GetNextObject()
    {
        if(objectNum < spawnObjects.Count)
        {
            GameObject obj = spawnObjects[objectNum];

            objectNum++;
            return obj;
        }

        objectNum = 0;
        return null;
    }
}

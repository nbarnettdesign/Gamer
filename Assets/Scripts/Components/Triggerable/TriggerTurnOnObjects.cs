using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTurnOnObjects : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects;
    [SerializeField] private bool autoTurnOffObjects;
    [SerializeField] private string triggerTag;

    private Collider col;

    private void Start()
    {
        col = GetComponent<Collider>();

        if (col.isTrigger == false)
            col.isTrigger = true;

        if (autoTurnOffObjects == false || objects == null || objects.Count <= 0) return;

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != triggerTag) return;

        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(true);
        }
    }
}

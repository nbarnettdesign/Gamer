using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private Vector3 pivot = new Vector3(0,50,0);

    private void Start()
    {
        pivot += transform.position;
    }

    private void Update()
    {
        transform.RotateAround( pivot, Vector3.up, speed * Time.deltaTime);
    }
}

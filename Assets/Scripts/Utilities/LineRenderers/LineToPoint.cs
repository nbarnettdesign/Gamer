using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineToPoint : MonoBehaviour {
    [SerializeField] private GameObject lineRenderer;
    [SerializeField] private Transform point;
    [SerializeField] private LayerMask pointLayer;
    [SerializeField] private bool staticPoint;

    private LineRenderer line;
    private Vector3 previousLocation;
    private Vector3 previousPointLocation;

    private void Start()
    {
#if UNITY_EDITOR
        if (lineRenderer == null)
            Debug.LogError(string.Format("Line to point on {0} but no line renderer given", transform.root != null ? transform.root.name : name));

        if (point == null)
            Debug.LogError(string.Format("Line to point on {0} but no point given", transform.root != null ? transform.root.name : name));
#endif

        if (lineRenderer)
        {
            line = SimplePool.Spawn(lineRenderer, transform.position, Quaternion.identity).GetComponent<LineRenderer>();
            line.transform.SetParent(transform);
        }

        UpdatePoints();
    }

    private void Update()
    {
        if (staticPoint == false && (point.position != previousPointLocation || transform.position != previousLocation))
        {
            previousLocation = point.position;
            previousPointLocation = point.position;
            UpdatePoints();
        }
    }

    private void UpdatePoints()
    {
        if (line == null || point == null)
            return;

        RaycastHit hit = new RaycastHit();

        Vector3 direction = point.position - transform.position;
        float dist = direction.magnitude;

        direction = direction / dist;

        line.SetPosition(0, transform.position);

        if (Physics.Raycast(transform.position, direction, out hit, dist, pointLayer))
        {
            line.SetPosition(1, hit.point);
        }
        else
            line.SetPosition(1, point.position);
    }
}

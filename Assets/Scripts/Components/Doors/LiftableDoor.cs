using UnityEngine;

public class LiftableDoor : MonoBehaviourExtended
{
    [SerializeField] private float maxHeight;
    [SerializeField] private float heightPerSecond;
    [SerializeField] private float dropPerSecond;
    [SerializeField] private float timeTillDrop;

    [Header("Gizmos")]
    [SerializeField]
    private bool showGizmos;
    [SerializeField] private Color gizmoColour;
    [SerializeField] private float gizmoSize;

    private float startHeight;
    private float dropCount;

    protected override void Start()
    {
        base.Start();
        startHeight = transform.position.y;
    }

    private void Update()
    {
        if (IsRendering() == false)
            return;

        if (transform.position.y <= startHeight)
        {
            if (dropCount > 0)
                dropCount = 0f;

            return;
        }

        dropCount += Time.deltaTime;

        if (dropCount >= timeTillDrop)
            DecreaseHeight();
    }

    public void IncreaseHeight()
    {
        dropCount = 0f;

        if (transform.position.y == startHeight + maxHeight)
            return;
        else
        {
            transform.position += new Vector3(0, heightPerSecond * Time.deltaTime, 0);

            if (transform.position.y > startHeight + maxHeight)
                transform.position = new Vector3(transform.position.x, startHeight + maxHeight, transform.position.z);
        }
    }

    public void DecreaseHeight()
    {
        if (transform.position.y == startHeight)
            return;
        else if (transform.position.y < startHeight)
            transform.position = new Vector3(transform.position.x, startHeight, transform.position.z);
        else
            transform.position -= new Vector3(0, dropPerSecond * Time.deltaTime, 0);
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false)
            return;

        if (Gizmos.color != gizmoColour)
            Gizmos.color = gizmoColour;

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + maxHeight, transform.position.z), gizmoSize);
    }
}

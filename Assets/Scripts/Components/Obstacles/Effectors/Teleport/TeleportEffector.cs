using UnityEngine;
using Valve.VR.InteractionSystem;

public class TeleportEffector : Effector
{
    [Header("Teleport Options")]
    [SerializeField] private bool teleportArrows;
    [SerializeField] private bool teleportThrowables;
    [Tooltip("The time before the teleported object can be teleported again")]
    [SerializeField] private float objectTeleportCooldown;

    [Header("Teleport Gizmos")]
    [SerializeField] private Color teleportPairGizmoColour;
    [SerializeField] private float teleportPairGizmoRadius;

    private GameObject pairedTeleport;

    protected override void Start()
    {
        base.Start();

        // If we are the pair object don't just keep on creating new ones of this
        if (transform.parent && transform.parent.gameObject.GetComponent<TeleportEffector>())
            return;

        GameObject pair = null;

        foreach (Transform child in transform)
        {
            if (child.name == "Teleport Pair")
            {
                pair = child.gameObject;
                break;
            }
        }

        if (pair == null)
        {
            Debug.LogError(string.Format("Something went wrong with {0}!", name));
            return;
        }

        Vector3 pairLocation = pair.transform.position;
        Quaternion pairRotation = pair.transform.rotation;

        Destroy(pair);
        pair = Instantiate(gameObject, pairLocation, pairRotation);
        pair.transform.SetParent(transform);

        pair.GetComponent<TeleportEffector>().GivePair(gameObject);
        GivePair(pair);
    }

    public void GivePair(GameObject pair)
    {
        pairedTeleport = pair;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject teleportObject = null;

        if (teleportArrows && other.GetComponent<Arrow>())
            teleportObject = other.gameObject;
        else if (teleportThrowables && other.GetComponent<Throwable>() && other.GetComponent<Throwable>().HasThrown == false)
            teleportObject = other.gameObject;

        if (teleportObject == null || Physics.Linecast(transform.position, teleportObject.transform.position, environmentLayer) ||
            teleportObject.GetComponent<Teleported>())
            return;

        // Get direction and distance of object to us
        Vector3 heading = teleportObject.transform.position - transform.position;

        // Teleport object
        teleportObject.transform.position = pairedTeleport.transform.position + heading;
        Teleported t = teleportObject.AddComponent<Teleported>();
        t.ExistsFor(objectTeleportCooldown);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Transform pair = null;

        foreach (Transform child in transform)
        {
            if (child.name == "Teleport Pair")
            {
                pair = child;
                break;
            }
        }

        if (pair != null)
        {
            if (Gizmos.color != teleportPairGizmoColour)
                Gizmos.color = teleportPairGizmoColour;
            Gizmos.DrawSphere(pair.position, teleportPairGizmoRadius);
        }
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (transform.parent && transform.parent.gameObject.GetComponent<TeleportEffector>())
            return;

        bool foundPair = false;

        foreach (Transform child in transform)
        {
            if (child.name == "Teleport Pair")
            {
                foundPair = true;
                break;
            }
        }

        if (foundPair == false)
        {
            GameObject g = new GameObject();
            g.transform.SetParent(transform, false);
            g.name = "Teleport Pair";
        }

    }
#endif
}

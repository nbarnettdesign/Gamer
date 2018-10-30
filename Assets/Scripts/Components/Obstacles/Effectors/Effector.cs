using UnityEngine;

public class Effector : MonoBehaviour
{
    [SerializeField] protected EffectorZone zoneType;
    [SerializeField] protected float radius;
    [SerializeField] protected bool adjustCollider;
    [SerializeField] protected LayerMask environmentLayer;
    [SerializeField] protected float lifeTime;
    [Header("Debug Options")]
    [SerializeField] private bool debugRadius;
    [SerializeField] private Color debugColour;

    private BoxCollider boxCollider;
    private SphereCollider sphereCollider;

    protected virtual void Start()
    {
        if (zoneType == EffectorZone.Box)
        {
            boxCollider = GetComponent<BoxCollider>();
            if(adjustCollider)
                AdjustCollider(boxCollider);
        }
        else if (zoneType == EffectorZone.Sphere)
        {
            sphereCollider = GetComponent<SphereCollider>();
            if(adjustCollider)
                AdjustCollider(sphereCollider);
        }

        if (lifeTime > 0)
        {
            this.Invoke(Remove, lifeTime);
        }
    }

    private void AdjustCollider(Collider collider)
    {
        if (collider.isTrigger == false)
            collider.isTrigger = true;

        if (collider is BoxCollider)
        {
            BoxCollider b = collider as BoxCollider;
            b.size = Vector3.one * (radius * 2);
        }
        else if (collider is SphereCollider)
        {
            SphereCollider s = collider as SphereCollider;
            s.radius = radius;
        }
    }

    private void Remove()
    {
        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmos()
    {
        if (debugRadius == false) return;

        if (Gizmos.color != debugColour)
        {
            Gizmos.color = debugColour;
        }

        if (zoneType == EffectorZone.Box)
        {
            Gizmos.DrawCube(transform.position, Vector3.one * (radius * 2));
        }
        else if (zoneType == EffectorZone.Sphere)
        {
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (zoneType == EffectorZone.Box)
        {
            if (GetComponent<SphereCollider>())
            {
                UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(GetComponent<SphereCollider>(), true);
            }

            if (GetComponent<BoxCollider>() == null)
            {
                BoxCollider b = gameObject.AddComponent<BoxCollider>();
                AdjustCollider(b);
            }
        }
        else if (zoneType == EffectorZone.Sphere)
        {
            if (GetComponent<BoxCollider>())
            {
                UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(GetComponent<BoxCollider>(), true);
            }

            if (GetComponent<SphereCollider>() == null)
            {
                SphereCollider s = gameObject.AddComponent<SphereCollider>();
                AdjustCollider(s);
            }
        }
    }
#endif
}
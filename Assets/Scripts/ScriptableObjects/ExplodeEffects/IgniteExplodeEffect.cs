using UnityEngine;

[CreateAssetMenu(fileName = "Ignite Explode Effect", menuName = "Interactables/Explode Effects/Ignite", order = 1)]
public class IgniteDeathEffect : ExplodeEffect
{
    [SerializeField] private float burnTime;
    [SerializeField] private LayerMask objectMask;
    [SerializeField] private FireStrength fireStrength;

    public override void Trigger(Vector3 location, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(location, radius, objectMask);

        foreach (Collider c in hitColliders)
        {
            Affectable[] affectables = c.GetComponents<Affectable>();

            for (int i = 0; i < affectables.Length; i++)
            {
                affectables[i].FireExposure(fireStrength);
            }
        }
    }
}

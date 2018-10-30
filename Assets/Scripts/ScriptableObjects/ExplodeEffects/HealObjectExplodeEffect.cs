using UnityEngine;

[CreateAssetMenu(fileName = "Heal on Explode", menuName = "Interactables/Explode Effects/Heal Object", order = 2)]
public class HealObjectExplodeEffect : ExplodeEffect
{
    [SerializeField] private float healAmount;
    [SerializeField] private bool onlyPlayer;
    [SerializeField] private LayerMask effectLayers;

    public override void Trigger(Vector3 location, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(location, radius, effectLayers);

        foreach (Collider col in hitColliders)
        {
            if (onlyPlayer && col.tag != "Player") continue;

            Affectable[] affectables = col.GetComponents<Affectable>();
            for (int i = 0; i < affectables.Length; i++)
            {
                affectables[i].Heal(healAmount);
            }
        }
    }
}

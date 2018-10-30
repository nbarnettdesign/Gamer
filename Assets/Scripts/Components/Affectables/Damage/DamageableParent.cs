using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class DamageableParent : MonoBehaviour {
    [SerializeField] private float health;
    [SerializeField] private UnityEvent onDeath;

    [Header("On Death")]
    [SerializeField] private GameObject deathObject;
    [SerializeField] private GameObject deathParticle;
    [SerializeField] private SoundPlayOneshot deathSound;
    [SerializeField] private LayerMask forceLayer;
    [SerializeField] private float deathObjectForceMultiplier;

    private float currentHealth;

    private void Start() {
        currentHealth = health;
    }

    public void Damage(float amount) {
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    private void Die() {
        Debug.Log("DO THE DYING THING!");
        StartCoroutine(RemoveObject());
    }

    private IEnumerator RemoveObject() {
        if (deathParticle != null)
            SimplePool.Spawn(deathParticle, transform.position, transform.rotation);

        if (deathSound != null)
            deathSound.Play();

        if (deathObject != null) {
            SimplePool.Spawn(deathObject, transform.position, transform.rotation);

            if (deathObjectForceMultiplier > 0) {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f, forceLayer);

                foreach (Collider col in hitColliders) {
                    if (col.GetComponent<Rigidbody>())
                        col.GetComponent<Rigidbody>().AddForceAtPosition(transform.position * deathObjectForceMultiplier,
                            transform.position);
                }
            }
        }

        Entity e = GetComponent<Entity>();
        if (e != null)
            EntityController.Instance.EntityKilled(e);

        yield return new WaitForEndOfFrame();
        Destroy(gameObject);
    }
}

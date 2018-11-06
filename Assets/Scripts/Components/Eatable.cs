using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Eatable : MonoBehaviour {
    [SerializeField] private float healthAmount;
    [SerializeField] private float eatingTime;
    [SerializeField] private float maxDistanceFromFace;
    [SerializeField] private AnimationCurve shrinkCurve;
    [SerializeField] private GameObject eatenParticle;
    [SerializeField] private Color particleStartColour;
    [SerializeField] private AudioClip eatenSound;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private GameObject leftOverObject;

    private bool isEating;

    private Throwable throwable;

    private void Start()
    {
        throwable = GetComponent<Throwable>();

        if (throwable == null)
            throwable = GetComponentInParent<Throwable>();
    }

    private void Eat(Affectable affectable)
    {
        affectable.Heal(healthAmount);

        throwable.ForceDetach(true);

        if (leftOverObject)
        {
            GameObject leftOvers = SimplePool.Spawn(leftOverObject, transform.position, leftOverObject.transform.rotation);

            if (throwable.AttachedToHand)
            {
                throwable.AttachedToHand.AttachObject(leftOvers, Hand.startingItemAttachmentFlags);
            }
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isEating)
            return;

        if(playerMask == (playerMask | (1 << other.gameObject.layer)))
        {
            if(other.name == "HeadCollider" && other.transform.root.GetComponentInChildren<Affectable>())
                StartCoroutine(StartEat(other.transform, other.transform.root.GetComponentInChildren<Affectable>()));
        }
    }

    private IEnumerator StartEat(Transform eatingCollider, Affectable other)
    {
        isEating = true;

        if (eatenParticle)
        {
            GameObject p = SimplePool.Spawn(eatenParticle, transform.position, Quaternion.identity);
            var main = p.GetComponent<ParticleSystem>().main;
            main.startColor = particleStartColour;
        }

        if (eatenSound)
            AudioSource.PlayClipAtPoint(eatenSound, transform.position);

        float t = 0f;
        Vector3 startingScale = transform.localScale;

        while(t <= eatingTime)
        {
            if (throwable.AttachedToHand == false || (eatingCollider.position - transform.position).sqrMagnitude > maxDistanceFromFace * maxDistanceFromFace)
            {
                isEating = false;
                yield break;
            }

            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startingScale, Vector3.zero, shrinkCurve.Evaluate(t / eatingTime));
            yield return null;
        }

        Eat(other);
    }
}

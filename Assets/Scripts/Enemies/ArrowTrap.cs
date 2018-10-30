using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private List<GameObject> arrowSlots;
    [SerializeField] private float arrowForce;
    [SerializeField] private float resetDelay;

    [Header("Volley Options")]
    [SerializeField] private float initialVolleyDelay;
    [SerializeField] private int volleys;
    [SerializeField] private float timeBetweenVolleys;

    [Header("Player Feedback")]
    [SerializeField] private GameObject triggerParticle;
    [SerializeField] private SoundPlayOneshot triggerSound;
    [SerializeField] private SoundPlayOneshot volleySound;
    [SerializeField] private SoundPlayOneshot resetSound;

    private bool canTrigger;

    private void Start()
    {
        canTrigger = true;
    }

    public void Trigger()
    {
        if (canTrigger == false)
            return;

        canTrigger = false;
        StartCoroutine(FireVolley());
    }

    private void InvokeReset()
    {
        if(resetSound)
            resetSound.Play();

        canTrigger = true;
    }

    private IEnumerator FireVolley()
    {
        if (triggerSound)
            triggerSound.Play();

        yield return new WaitForSeconds(initialVolleyDelay);

        if (triggerParticle)
            Instantiate(triggerParticle, transform.position, transform.rotation);

        for (int i = 0; i < volleys; i++) {
            if (arrowSlots.Count <= 0)
                FireArrow(Instantiate(arrowPrefab, transform.position, transform.rotation));
            else {
                for (int j = 0; j < arrowSlots.Count; j++) {
                    FireArrow(Instantiate(arrowPrefab, arrowSlots[j].transform.position, arrowSlots[j].transform.rotation));
                }
            }

            if(volleySound)
                volleySound.Play();

            yield return new WaitForSeconds(timeBetweenVolleys);
        }

        Invoke("InvokeReset", resetDelay);
    }

    private void FireArrow(GameObject arrow)
    {
        Arrow a = arrow.GetComponent<Arrow>();

        a.ShaftRB.isKinematic = false;
        a.ShaftRB.useGravity = true;
        a.ShaftRB.transform.GetComponent<BoxCollider>().enabled = true;

        a.ArrowHeadRB.isKinematic = false;
        a.ArrowHeadRB.useGravity = true;
        a.ArrowHeadRB.transform.GetComponent<BoxCollider>().enabled = true;

        a.ArrowHeadRB.AddForce(transform.forward * arrowForce, ForceMode.VelocityChange);
        a.ArrowHeadRB.AddTorque(transform.forward * arrowForce);

        a.ArrowReleased(arrowForce);
        arrow.transform.SetParent(null);
    }

    private void OnValidate()
    {
        if (volleys < 1)
            volleys = 1;
    }
}

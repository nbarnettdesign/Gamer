using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

public class DamageablePlayer : Affectable
{
    [SerializeField] private float startingHealth;

    [Header("Health Regeneration")]
    [Tooltip("The maximum amount of health to regenerate to")]
    [SerializeField] private float healthRegenThreshold;
    [Tooltip("The delay between health being granted back to the player")]
    [SerializeField] private float healthRegenDelay;
    [Tooltip("The amount of health to grant after delay")]
    [Range(0f, 0.5f), SerializeField] private float healthRegenAmount;
    [SerializeField] private AudioSource lowHealthSound;
    [SerializeField] private SoundPlayOneshot damageSound;

    [Header("Overlay")]
    [SerializeField] private GameObject faceSphere;
    [SerializeField] private string dissolveShaderHandle;
    [SerializeField] private float dissolveInTime;
    [SerializeField] private AnimationCurve dissolveInCurve;
    [SerializeField] private float dissolveOutTime;
    [SerializeField] private AnimationCurve dissolveOutCurve;
    [Range(0f, 1f), SerializeField] private float minDissolveAmount;
    [Range(0f, 1f), SerializeField] private float maxDissolveAmount;
    [Range(0f, 1f), SerializeField] private float minDissolvePulseAmount;
    [SerializeField] private float pulseTime;

    [Header("Events")]
    [SerializeField]
    private UnityEvent onHit;
    [SerializeField] private UnityEvent onDeath;

    [Header("Debug Options")]
    [SerializeField] private bool invunerable;
    [SerializeField] private bool neverInstaKill;
    [SerializeField] private bool debugTakeDamage;

    private float currentHealth;

    private bool beenKilled;
    private float delayCount;
    private float regenCount;

    private float lowHealthMaxScale;
    private bool isScaling;
    private bool shouldPulse;
    private bool pulseGrow;

    private Renderer faceSphereRenderer;

    protected override void Start()
    {
        base.Start();
        currentHealth = startingHealth;

        if (faceSphere)
        {
            faceSphere.SetActive(false);
            faceSphereRenderer = faceSphere.GetComponent<Renderer>();
        }
    }

    private void Update()
    {
        Regen();

        if (debugTakeDamage)
        {
            debugTakeDamage = false;
            Damage(1, transform.position, Vector3.zero);
        }
    }

    public override void Damage(float amount, Vector3 impactPosition, Vector3 impactVelocity, bool fromExplosion = false, bool instaKill = false)
    {
        if (beenKilled || invunerable)
            return;

        Debug.Log("Ouch!");

        if (damageSound)
            damageSound.Play();

        currentHealth -= amount;

        onHit.Invoke();

        if (currentHealth < healthRegenThreshold && faceSphere.activeInHierarchy == false)
        {
            faceSphere.SetActive(true);

            if (lowHealthSound)
                lowHealthSound.Play();

            pulseGrow = true;
            shouldPulse = true;
            StartCoroutine(FaceSphereFade(dissolveInTime, minDissolveAmount, maxDissolveAmount, dissolveInCurve));
        }

        if (currentHealth <= 0 || neverInstaKill == false && instaKill)
        {
            beenKilled = true;
            onDeath.Invoke();
        }
    }

    public override void Heal(float amount)
    {
        if (currentHealth > startingHealth)
            return;

        currentHealth += amount;

        if (currentHealth > startingHealth)
            currentHealth = startingHealth;
    }

    private void Regen()
    {
        if (beenKilled || currentHealth > healthRegenThreshold)
            return;

        if (currentHealth == healthRegenThreshold)
        {
            if (faceSphere && faceSphere.activeInHierarchy && isScaling == false)
            {
                shouldPulse = false;

                if (lowHealthSound)
                    lowHealthSound.Stop();

                StartCoroutine(FaceSphereFade(dissolveOutTime, maxDissolveAmount, minDissolveAmount, dissolveOutCurve, true));
            }

            return;
        }

        Pulse();

        regenCount += Time.deltaTime;

        if (regenCount <= healthRegenDelay)
            return;

        regenCount = 0f;
        currentHealth++;
    }

    private void Pulse()
    {

        if (isScaling || shouldPulse == false)
            return;


        float pulseValue = faceSphereRenderer.material.GetFloat(dissolveShaderHandle);

        if (pulseGrow)
        {
            pulseValue += pulseTime * Time.deltaTime;

            if (pulseValue >= maxDissolveAmount)
                pulseGrow = false;
        }
        else
        {
            pulseValue -= pulseTime * Time.deltaTime;

            if (pulseValue <= minDissolvePulseAmount)
                pulseGrow = true;
        }

        faceSphereRenderer.material.SetFloat(dissolveShaderHandle, pulseValue);
    }

    private IEnumerator FaceSphereFade(float dissolveTime, float startValue, float endValue, AnimationCurve curve, bool turnOff = false)
    {
        if (faceSphere == null)
            yield break;

        float t = 0f;
        isScaling = true;

        while (t <= dissolveTime)
        {
            t += Time.deltaTime;

            faceSphereRenderer.material.SetFloat(dissolveShaderHandle,
                Mathf.LerpUnclamped(startValue, endValue, curve.Evaluate(t / dissolveTime)));

            yield return null;
        }

        if (turnOff)
        {
            faceSphere.SetActive(false);
        }

        isScaling = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (healthRegenThreshold > startingHealth)
            healthRegenThreshold = startingHealth;
    }
#endif
}

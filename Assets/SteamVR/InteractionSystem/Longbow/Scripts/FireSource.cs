using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
    public class FireSource : Affectable
    {
        [SerializeField] private GameObject fireParticlePrefab;
        [SerializeField] private bool startActive;
        [SerializeField] private bool isBurning;

        [Header("Burning")]
        [SerializeField] private float burnTime;
        [SerializeField] private float ignitionDelay = 0;
        [SerializeField] private FireStrength strengthOfFire;
        [SerializeField] private FireStrength litBy;

        [Header("Relighting")]
        [SerializeField] private bool canRelight;
        [Tooltip("The number of times this firesource can be relit")]
        [SerializeField] private int relightNumber;
        [SerializeField] private bool infiniteRelights;

        [Header("Propagation")]
        [SerializeField] private float propagationTime;
        [SerializeField] private float propagationRange;
        [SerializeField] private bool drawPropagationRange;
        [SerializeField] private bool canRepropagate;

        [SerializeField] private AudioSource ignitionSound;
        [SerializeField] private bool canSpreadFromThisSource = true;

        [Header("Damage")]
        [SerializeField] private float burnDamage;
        [SerializeField] private float burnDamageRate;

        [Header("Light")]
        [SerializeField] private GameObject lightPrefab;
        [SerializeField] private bool preventLight;

        [Header("Events")]
        [SerializeField] private UnityEvent onLit;
        [SerializeField] private UnityEvent onExtinguish;

        public bool IsBurning { get { return isBurning; } }
        public bool Damaging { get { return burnDamage > 0; } }
        public FireStrength FireStrength { get { return strengthOfFire; } }

        private GameObject fireObject;
        private float ignitionTime;
        private Hand hand;

        private Damageable damageable;
        private float burnTickTime;

        private float propagationCount;
        private GameObject lightObject;

        private bool beenLit;
        private int litCount;

        protected override void Start()
        {
            base.Start();
            damageable = GetComponent<Damageable>();

            if (damageable == null && transform.parent)
                damageable = transform.parent.GetComponent<Damageable>();

            if (startActive)
                StartBurning();

            if (lightPrefab)
            {
                lightObject = SimplePool.Spawn(lightPrefab, transform.position, lightPrefab.transform.rotation);
                lightObject.transform.SetParent(transform);

                if (startActive == false)
                    lightObject.SetActive(false);
            }
        }

        void Update()
        {
            if (burnTime > 0 && Time.time > (ignitionTime + burnTime) && isBurning)
                StopBurn();

            Propagate();

            if (burnDamage > 0 && isBurning && damageable != null)
            {
                burnTickTime += Time.deltaTime;

                if (burnTickTime >= burnDamageRate)
                {
                    burnTickTime = 0f;
                    damageable.Damage(burnDamage, transform.position, damageable.transform.position - transform.position);
                }
            }
        }

        private void Propagate()
        {
            if (canSpreadFromThisSource == false || propagationTime <= 0)
                return;

            if (isBurning == false)
                return;

            propagationCount += Time.deltaTime;

            if (propagationCount < propagationTime)
                return;

            propagationCount = 0f;
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, propagationRange);

            foreach (Collider hit in hitColliders)
            {
                Affectable e = hit.GetComponent<Affectable>();

                if (e == null) continue;

                e.FireExposure(strengthOfFire);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (isBurning && canSpreadFromThisSource)
            {
                Affectable[] affectables = other.GetComponents<Affectable>();

                for (int i = 0; i < affectables.Length; i++)
                {
                    affectables[i].FireExposure(strengthOfFire);
                }
            }
        }

        public override void FireExposure(FireStrength strength)
        {
            if (isBurning || litBy == FireStrength.None)
                return;

            if (litBy != FireStrength.None && strength >= litBy)
            {
                if (ignitionDelay <= 0)
                {
                    onLit.Invoke();
                    StartBurning();
                }
                else
                {
                    this.Invoke(onLit.Invoke, ignitionDelay);
                    this.Invoke(StartBurning, ignitionDelay);
                }
            }

            if (hand = GetComponentInParent<Hand>())
                hand.controller.TriggerHapticPulse(1000);
        }

        public void Extinguish()
        {
            StopBurn();
        }

        public void Extinguish(float delay)
        {
            this.Invoke(StopBurn, delay);
        }

        private void StartBurning()
        {
            if (beenLit && canRelight == false || beenLit && infiniteRelights == false && litCount >= relightNumber)
                return;

            if (lightObject)
                lightObject.SetActive(true);

            isBurning = true;

            if (beenLit)
                litCount++;

            if (beenLit == false)
                beenLit = true;

            ignitionTime = Time.time;

            if (ignitionSound != null)
                ignitionSound.Play();

            if (fireParticlePrefab != null)
            {
                fireObject = SimplePool.Spawn(fireParticlePrefab, transform.position, transform.rotation);

                ParticleSystem particle = fireObject.GetComponent<ParticleSystem>();

                if (particle == null)
                    return;

                particle.Clear();
                particle.Play();

                fireObject.transform.SetParent(transform);
            }
        }

        private void StopBurn()
        {
            isBurning = false;

            if (lightObject)
                lightObject.SetActive(false);

            if (fireObject)
            {
                SimplePool.Despawn(fireObject);
                fireObject = null;
            }

            onExtinguish.Invoke();
        }

        private IEnumerator Remove()
        {
            yield return new WaitForEndOfFrame();
            SimplePool.Despawn(transform.parent.gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            if (drawPropagationRange)
                Gizmos.DrawWireSphere(transform.position, propagationRange);
        }
    }
}

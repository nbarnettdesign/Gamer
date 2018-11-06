using UnityEngine;
using System.Collections.Generic;

namespace Valve.VR.InteractionSystem
{
    public class Arrow : Hoverable
    {
        [Header("Damage")]
        [SerializeField] protected float damage;
        [SerializeField] private float minDamageVelocity;
        [SerializeField] private LayerMask tooCloseLayers;

        [Header("Lifetime")]
        [SerializeField] protected float arrowLifeTime = 30f;

        [Header("Glint")]
        [SerializeField] private ParticleSystem glintParticle;
        [SerializeField] private float glintStartVelocity;
        [SerializeField] private float glintStartDisance;
        [SerializeField] private float glintCheckTime;

        [Header("Rigidbodies")]
        [SerializeField] private Rigidbody arrowHeadRB;
        [SerializeField] private Rigidbody shaftRB;

        [Header("Sticking")]
        [SerializeField] private List<PhysicMaterial> targetPhysMaterial;
        [SerializeField] private float minStickVelocity;

        [Header("Impale")]
        [SerializeField] private PhysicMaterial impalePhysMaterial;
        [SerializeField] private float minImpaleVelocity;

        [Header("Deflection")]
        [SerializeField] private PhysicMaterial reflectPhysMaterial;
        [SerializeField] private float reflectScalar;

        [Header("Sound Effects")]
        [SerializeField] private SoundPlayOneshot fireReleaseSound;
        [SerializeField] private SoundPlayOneshot airReleaseSound;
        [SerializeField] private SoundPlayOneshot hitTargetWoodSound;
        [SerializeField] private SoundPlayOneshot hitTargetStoneSound;
        [SerializeField] private SoundPlayOneshot hitTargetFleshSound;
        [SerializeField] private SoundPlayOneshot hitTargetMetalSound;
        [SerializeField] private PlaySound hitGroundSound;

        [Header("Sound Physics Materials")]
        [SerializeField] private PhysicMaterial woodMaterial;
        [SerializeField] private PhysicMaterial stoneMaterial;
        [SerializeField] private PhysicMaterial fleshMaterial;
        [SerializeField] private PhysicMaterial metalMaterial;

        [Header("Parenting")]
        [SerializeField] protected string arrowParentName = "Arrow Scale Parent";

        public Rigidbody ShaftRB { get { return shaftRB; } }
        public Rigidbody ArrowHeadRB { get { return arrowHeadRB; } }
        public bool StuckInTarget { get { return stuckInTarget; } }
        public bool NonAnchorPointCollision { get { return nonAnchorPointCollision; } }

        private Vector3 prevPosition;
        private Quaternion prevRotation;
        private Vector3 prevVelocity;
        private Vector3 prevHeadPosition;

        protected bool inFlight;
        protected bool released;
        protected bool stuckInTarget;
        private bool hasSpreadFire = false;

        private int travelledFrames = 0;

        private GameObject scaleParentObject = null;
        private TrailRenderer trail;

        private bool playedGlint;
        private Vector3 startPosition;
        private float glintCheckCount;

        private bool nonAnchorPointCollision;

        protected virtual void OnEnable()
        {
            // If inFlight is false we know that this arrow has not been fired yet
            // so just return
            if (released == false)
                return;

            inFlight = false;
            released = false;
            stuckInTarget = false;
            scaleParentObject = null;
            playedGlint = false;

            if (trail)
            {
                trail.Clear();
                trail.gameObject.SetActive(false);
            }
        }

        protected virtual void Start()
        {
            if (shaftRB == null)
                return;

            Physics.IgnoreCollision(shaftRB.GetComponent<Collider>(), Player.Instance.headCollider);
            trail = GetComponentInChildren<TrailRenderer>();

            if (trail != null)
                trail.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (released && inFlight)
            {
                prevPosition = transform.position;
                prevRotation = transform.rotation;
                prevVelocity = GetComponent<Rigidbody>().velocity;
                prevHeadPosition = arrowHeadRB.transform.position;
                travelledFrames++;

                if (glintParticle != null && playedGlint == false && arrowHeadRB.velocity.magnitude >= glintStartVelocity)
                {

                    glintCheckCount += Time.deltaTime;

                    if (glintCheckCount >= glintCheckTime)
                    {
                        glintCheckCount = 0f;

                        if ((transform.position - startPosition).sqrMagnitude >= glintStartDisance * glintStartDisance)
                        {
                            playedGlint = true;
                            glintParticle.Play();
                        }
                    }
                }
            }
        }

        public void ResetReleased()
        {
            released = false;
            prevPosition = transform.position;
            prevRotation = transform.rotation;
            prevVelocity = GetComponent<Rigidbody>().velocity;
            prevHeadPosition = arrowHeadRB.transform.position;
            travelledFrames = 0;
        }

        public override void HandHoverUpdate(Hand hand)
        {
            if (disabled)
                return;

            //Trigger got pressed
            if (hand.GetStandardInteractionButtonDown())
            {
                if (arrowHeadRB)
                {
                    arrowHeadRB.isKinematic = true;
                    arrowHeadRB.GetComponent<Collider>().enabled = true;
                }

                if (shaftRB)
                {
                    shaftRB.isKinematic = true;
                    shaftRB.GetComponent<Collider>().enabled = true;
                }

                hand.AttachObject(gameObject, Hand.defaultAttachmentFlags);
                ControllerButtonHints.HideButtonHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);
            }
        }

        public virtual void ArrowReleased(float inputVelocity)
        {
            Destroy(GetComponentInChildren<Damager>());

            inFlight = true;
            released = true;

            airReleaseSound.Play();
            startPosition = transform.position;

            if (trail != null)
                trail.gameObject.SetActive(true);

            if (gameObject.GetComponentInChildren<FireSource>().IsBurning)
                fireReleaseSound.Play();

            // Check if arrow is shot inside or too close to an object
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.01f, transform.forward, 0.80f, tooCloseLayers, QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject != gameObject && hit.collider.gameObject != arrowHeadRB.gameObject && hit.collider != Player.Instance.headCollider)
                {
                    RemoveObject();
                    return;
                }
            }

            travelledFrames = 0;
            prevPosition = transform.position;
            prevRotation = transform.rotation;
            prevHeadPosition = arrowHeadRB.transform.position;
            prevVelocity = GetComponent<Rigidbody>().velocity;

            this.Invoke(RemoveObject, arrowLifeTime);
        }

        protected virtual void RemoveObject()
        {
            if (shaftRB)
            {
                shaftRB.velocity = Vector3.zero;
                shaftRB.angularVelocity = Vector3.zero;
                shaftRB.isKinematic = true;
                shaftRB.useGravity = false;
            }

            if (arrowHeadRB)
            {
                arrowHeadRB.velocity = Vector3.zero;
                arrowHeadRB.angularVelocity = Vector3.zero;
                arrowHeadRB.isKinematic = true;
                arrowHeadRB.useGravity = false;
            }

            arrowHeadRB.transform.GetComponent<BoxCollider>().enabled = false;

            if (shaftRB)
                shaftRB.transform.GetComponent<BoxCollider>().enabled = false;

            transform.SetParent(null);

            playedGlint = false;
            SimplePool.Despawn(gameObject);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (inFlight)
            {
                if (collision.transform.GetComponent<AnchorPoint>() == null)
                    nonAnchorPointCollision = true;

                Rigidbody rb = GetComponent<Rigidbody>();
                float rbSpeed = rb.velocity.sqrMagnitude;
                bool canStick = collision.gameObject.GetComponent<AnchorPoint>();

                if (collision.collider.sharedMaterial)
                    Debug.Log(collision.collider.sharedMaterial.name);

                if (canStick == false)
                {
                    for (int i = 0; i < targetPhysMaterial.Count; i++)
                    {
                        if(collision.collider.sharedMaterial && collision.collider.sharedMaterial.name.Contains(targetPhysMaterial[i].name))
                        {
                            canStick = true;
                            break;
                        }
                    }
                }

                Debug.Log(canStick);

                bool canImpale = (impalePhysMaterial != null && collision.collider.sharedMaterial == impalePhysMaterial && rbSpeed > minImpaleVelocity);               

                //Arrow deflecting non-stick or impale material
                if (travelledFrames < 2 && !canStick && !canImpale)
                {
                    // Reset transform but halve your velocity
                    transform.position = prevPosition - prevVelocity * Time.deltaTime;
                    transform.rotation = prevRotation;

                    Vector3 reflfectDir = Vector3.Reflect(arrowHeadRB.velocity, collision.contacts[0].normal);

                    if(reflectPhysMaterial != null && collision.collider.sharedMaterial == reflectPhysMaterial)
                    {
                        arrowHeadRB.velocity = reflfectDir * reflectScalar;
                        shaftRB.velocity = reflfectDir * reflectScalar;
                    }
                    else
                    {
                        arrowHeadRB.velocity = reflfectDir * 0.25f;
                        shaftRB.velocity = reflfectDir * 0.25f;
                    }

                    travelledFrames = 0;
                    return;
                }
                //older j.i.c
                //if (travelledFrames < 2 && !canStick)
                //{
                //    // Reset transform but halve your velocity
                //    transform.position = prevPosition - prevVelocity * Time.deltaTime;
                //    transform.rotation = prevRotation;
                //
                //    Vector3 reflfectDir = Vector3.Reflect(arrowHeadRB.velocity, collision.contacts[0].normal);
                //    arrowHeadRB.velocity = reflfectDir * 0.25f;
                //    shaftRB.velocity = reflfectDir * 0.25f;
                //
                //    travelledFrames = 0;
                //    return;
                //}

                if (glintParticle != null)
                    glintParticle.Stop(true);

                if (trail != null)
                    trail.gameObject.SetActive(false);

                if (rbSpeed > 0.1f)
                    hitGroundSound.Play();

                //cause damage
                if (arrowHeadRB.velocity.sqrMagnitude >= minDamageVelocity)
                {
                    Affectable[] affectables = collision.gameObject.GetComponents<Affectable>();

                    FireSource arrowFire = gameObject.GetComponentInChildren<FireSource>();
                    FireSource fireSourceOnTarget = collision.collider.GetComponentInParent<FireSource>();

                    for (int i = 0; i < affectables.Length; i++)
                    {
                        if (arrowFire != null && arrowFire.IsBurning && (fireSourceOnTarget != null))
                        {
                            if (!hasSpreadFire)
                            {
                                hasSpreadFire = true;
                                affectables[i].FireExposure(arrowFire.FireStrength);
                            }
                        }
                        else
                        {
                            if (rbSpeed > 0.1f && affectables[i].ArrowImmune == false)
                            {
                                affectables[i].Damage(damage, arrowHeadRB.transform.position, arrowHeadRB.transform.forward);
                                affectables[i].Explode(arrowHeadRB.velocity.magnitude, transform.position, arrowHeadRB.velocity);
                            }
                            else if (rbSpeed < 0.1f)
                                GetComponent<Rigidbody>().isKinematic = true;
                        }
                    }
                }

                if (canImpale)
                {
                    Impale(collision.gameObject, collision, travelledFrames < 2);
                }

                if (canStick)
                {
                    StickInTarget(collision, travelledFrames < 2 || collision.gameObject.GetComponent<AnchorPoint>());
                    Anchor(collision);
                }


                // Player Collision Check (self hit)
                if (Player.Instance && collision.collider == Player.Instance.headCollider)
                    Player.Instance.PlayerShotSelf();
            }
        }

        protected void Impale(GameObject impaledObj, Collision collision, bool bSkipRayCast)
        {
            Vector3 prevForward = prevRotation * Vector3.forward;

            // Only stick in target if the collider is front of the arrow head
            if (!bSkipRayCast)
            {
                RaycastHit[] hitInfo;
                hitInfo = Physics.RaycastAll(prevHeadPosition - prevVelocity * Time.deltaTime, prevForward, prevVelocity.magnitude * Time.deltaTime * 2.0f);
                bool properHit = collision.gameObject.GetComponent<AnchorPoint>();

                if(properHit)


                for (int i = 0; i < hitInfo.Length; ++i)
                {
                    RaycastHit hit = hitInfo[i];

                    if (hit.collider == collision.collider)
                    {
                        properHit = true;
                        break;
                    }
                }

                if (!properHit)
                {
                    return;
                }
            }

            if (glintParticle != null)
            {
                SimplePool.Despawn(glintParticle.gameObject);
            }

            //Child the object you're impaling
            //parent must be arrow
            Transform childTransform = collision.collider.transform;
            childTransform.position = collision.contacts[0].point;
            childTransform.transform.parent = transform;
      
            Rigidbody childRb = childTransform.GetComponent<Rigidbody>();
            //childRb.velocity = Vector3.zero;
            //childRb.angularVelocity = Vector3.zero;
            childRb.isKinematic = true;
            childRb.useGravity = false;
            childTransform.GetComponent<Collider>().enabled = false;

            // Move the arrow to the place on the target collider we were expecting to hit prior to the impact itself knocking it around
            // transform.rotation = prevRotation;
            //transform.position = prevPosition;
            //childTransform.position = collision.contacts[0].point - transform.forward * (0.75f - (Util.RemapNumberClamped(prevVelocity.magnitude, 0f, 10f, 0.0f, 0.1f) + Random.Range(0.0f, 0.05f))) + new Vector3(0, 0, 0.5f);

            ///TODO
            //Need to set the impaled object to local transform of (0,0,0.5f) to make it stuck on the arrow (above the tip)

            childTransform.Translate(Vector3.forward * 0.5f);

            //Play impale sound fx
            hitTargetWoodSound.Play();
        }

        protected void StickInTarget(Collision collision, bool bSkipRayCast)
        {
            Vector3 prevForward = prevRotation * Vector3.forward;

            // Only stick in target if the collider is front of the arrow head
            //if (!bSkipRayCast)
            //{
            //    RaycastHit[] hitInfo;
            //    hitInfo = Physics.RaycastAll(prevHeadPosition - prevVelocity * Time.deltaTime, prevForward, prevVelocity.magnitude * Time.deltaTime * 2.0f);
            //    bool properHit = false;
            //    for (int i = 0; i < hitInfo.Length; ++i)
            //    {
            //        RaycastHit hit = hitInfo[i];

            //        if (hit.collider == collision.collider)
            //        {
            //            properHit = true;
            //            break;
            //        }
            //    }

            //    if (!properHit)
            //    {
            //        return;
            //    }
            //}

            if (glintParticle != null)
            {
                SimplePool.Despawn(glintParticle.gameObject);
            }


            inFlight = false;

            //shaftRB.velocity = Vector3.zero;
            //shaftRB.angularVelocity = Vector3.zero;
            //shaftRB.isKinematic = true;
            //shaftRB.useGravity = false;
            //shaftRB.transform.GetComponent<BoxCollider>().enabled = false;

            Destroy(shaftRB);

            arrowHeadRB.transform.GetComponent<BoxCollider>().enabled = false;

            PlayImpactSound(collision.collider.material);

            // If the hit item has a parent, dock an empty object to that
            // this fixes an issue with scaling hierarchy. I suspect this is not sustainable for a large object / scaling hierarchy.
            scaleParentObject = new GameObject(arrowParentName);
            Transform parentTransform = collision.collider.transform;
            scaleParentObject.transform.position = collision.contacts[0].point;

            scaleParentObject.transform.parent = parentTransform;

            //Add stabbable re-child here

            // Move the arrow to the place on the target collider we were expecting to hit prior to the impact itself knocking it around
            transform.parent = scaleParentObject.transform;
            transform.rotation = prevRotation;
            transform.position = prevPosition;
            transform.position = collision.contacts[0].point - transform.forward * (0.75f - (Util.RemapNumberClamped(prevVelocity.magnitude, 0f, 10f, 0.0f, 0.1f) + Random.Range(0.0f, 0.05f)));
        }

        protected virtual void Anchor(Collision collision)
        {
            if (collision.gameObject.GetComponent<AnchorPoint>() == null || nonAnchorPointCollision)
            {
                GameObject.FindWithTag("BowAnchor").GetComponent<BowAnchor>().BreakAnchor();
                return;
            }

            collision.gameObject.GetComponent<AnchorPoint>().Anchored();
        }

        protected virtual void OnDestroy()
        {
            if (scaleParentObject != null)
                Destroy(scaleParentObject);
        }

        private void PlayImpactSound(PhysicMaterial impactedMaterial)
        {
            if (impactedMaterial == woodMaterial && hitTargetWoodSound)
                hitTargetWoodSound.Play();
            else if (impactedMaterial == metalMaterial && hitTargetMetalSound)
                hitTargetMetalSound.Play();
            else if (impactedMaterial == stoneMaterial && hitTargetStoneSound)
                hitTargetStoneSound.Play();
            else if (impactedMaterial == fleshMaterial && hitTargetFleshSound)
                hitTargetFleshSound.Play();
        }
    }
}

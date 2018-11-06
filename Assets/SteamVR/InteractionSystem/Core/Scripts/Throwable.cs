using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Valve.VR.InteractionSystem {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(VelocityEstimator))]
    public class Throwable : Interactable
    {
        [Header("Attachment Settings")]
        [EnumFlags]
        [Tooltip("The flags used to attach this object to the hand.")]
        public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.DetachFromOtherHand;
     
        [Tooltip("Name of the attachment transform under in the hand's hierarchy which the object should should snap to.")]
        public string attachmentPoint;

        [Tooltip("How fast must this object be moving to attach due to a trigger hold instead of a trigger press?")]
        public float catchSpeedThreshold = 0.0f;

        [Tooltip("When detaching the object, should it return to its original parent?")]
        public bool restoreOriginalParent = false;

        public bool attachEaseIn = true;
        public AnimationCurve snapAttachEaseInCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
        public float snapAttachEaseInTime = .45f;
        public string[] attachEaseInAttachmentNames;

        [SerializeField] private float thrownVelocityModifier = 2.0f;
        [SerializeField, Range(0f, 1f)] private float thrownVelocityMinimum = 0.5f;

        [Header("Impact")]
        [SerializeField] private bool damageSelfOnImpact;
        [Range(0f, 10f), Tooltip("If velocity magnitude on impact is longer than this apply damage")]
        [SerializeField] private float impactDamageMagnitude;
        [SerializeField] private ObjectWeight impactForce;

        public Hand AttachedToHand { get { return attachedToHand; } }
        public bool HasThrown { get { return hasThrown; } }

        private VelocityEstimator velocityEstimator;
        private bool attached = false;
        private float attachTime;
        private Vector3 attachPosition;
        private Quaternion attachRotation;
        private Transform attachEaseInTransform;

        public UnityEvent onPickUp;
        public UnityEvent onDetachFromHand;

        public bool snapAttachEaseInCompleted = false;
        [SerializeField] private float awakeImpactVelocity;
        [SerializeField] private bool moveToGround;
        [SerializeField] private LayerMask sittingLayer;
        [Tooltip("Used to prevent physics objects rolling on start, enabled by default"), SerializeField] private bool kinematicOnStart = true;
        [SerializeField] private bool preventTeleportInHand;

        protected bool hasThrown;
        protected Hand attachedToHand;
        protected Rigidbody rBody;

        protected Damageable damageable;

        protected bool preventHover;

        void Awake () {
            velocityEstimator = GetComponent<VelocityEstimator>();

            if (attachEaseIn)
                attachmentFlags &= ~Hand.AttachmentFlags.SnapOnAttach;

            if (moveToGround) {
                // Are we sitting on something or just hovering?
                // if we are hovering move down
                RaycastHit hit;

                Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, sittingLayer);

                if (hit.transform != null) {
                    Vector3 halfSize = Vector3.zero;
                    halfSize.y = GetComponent<Collider>().bounds.size.y / 4f;

                    transform.position = hit.point + halfSize;
                }
            }

            rBody = GetComponent<Rigidbody>();

            if (kinematicOnStart) {
                rBody.isKinematic = true;
                rBody.maxAngularVelocity = 50.0f;
            }

            if (damageSelfOnImpact) {
                damageable = GetComponent<Damageable>();

                if (damageable == null) {
                    Debug.LogWarning(string.Format("{0} is marked as an impact throwable, but has no damageable script!", name));
                    damageSelfOnImpact = false;
                }
            }

            if (preventTeleportInHand == false)
                gameObject.AddComponent<AllowTeleportWhileAttachedToHand>();
        }

        public void ForceDetach (bool preventHover = false) {
            this.preventHover = preventHover;

            if(attachedToHand)
                attachedToHand.DetachObject(gameObject, restoreOriginalParent);
        }

        public override void OnHandHoverBegin (Hand hand) {
            if (disabled || preventHover)
                return;

            base.OnHandHoverBegin(hand);

            bool showHint = false;

            // "Catch" the throwable by holding down the interaction button instead of pressing it.
            // Only do this if the throwable is moving faster than the prescribed threshold speed,
            // and if it isn't attached to another hand
            if (!attached) {
                if (hand.GetStandardInteractionButton()) {
                    Rigidbody rb = GetComponent<Rigidbody>();
                    if (rb.velocity.magnitude >= catchSpeedThreshold) {
                        hand.AttachObject(gameObject, attachmentFlags, attachmentPoint);
                        showHint = false;
                    }
                }
            }

            if (showHint)
                ControllerButtonHints.ShowButtonHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);
        }

        public override void OnHandHoverEnd (Hand hand) {
            base.OnHandHoverEnd(hand);
            ControllerButtonHints.HideButtonHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);
        }

        public override void HandHoverUpdate (Hand hand) {
            if (disabled)
                return;

            //Trigger got pressed
            if (hand.GetStandardInteractionButtonDown()) {
                hand.AttachObject(gameObject, attachmentFlags, attachmentPoint);
                ControllerButtonHints.HideButtonHint(hand, EVRButtonId.k_EButton_SteamVR_Trigger);
            }
        }

        public override void OnAttachedToHand (Hand hand) {
            attachedToHand = hand;
            attached = true;

            onPickUp.Invoke();

            hand.HoverLock(null);

            if (rBody) {
                rBody.isKinematic = true;
                rBody.interpolation = RigidbodyInterpolation.None;
            }

            if (hand.controller == null)
                velocityEstimator.BeginEstimatingVelocity();

            attachTime = Time.time;
            attachPosition = transform.position;
            attachRotation = transform.rotation;

            if (attachEaseIn) {
                attachEaseInTransform = hand.transform;
                if (!Util.IsNullOrEmpty(attachEaseInAttachmentNames)) {
                    float smallestAngle = float.MaxValue;
                    for (int i = 0; i < attachEaseInAttachmentNames.Length; i++) {
                        Transform t = hand.GetAttachmentTransform(attachEaseInAttachmentNames[i]);
                        float angle = Quaternion.Angle(t.rotation, attachRotation);
                        if (angle < smallestAngle) {
                            attachEaseInTransform = t;
                            smallestAngle = angle;
                        }
                    }
                }
            }

            snapAttachEaseInCompleted = false;
        }

        public override void OnDetachedFromHand (Hand hand) {
            attachedToHand = null;
            attached = false;

            hand.HoverUnlock(null);


            if (rBody) {
                rBody.isKinematic = false;
                rBody.interpolation = RigidbodyInterpolation.Interpolate;
            }

            onDetachFromHand.Invoke();

            Vector3 position = Vector3.zero;
            Vector3 velocity = Vector3.zero;
            Vector3 angularVelocity = Vector3.zero;
            if (hand.controller == null) {
                velocityEstimator.FinishEstimatingVelocity();
                velocity = velocityEstimator.GetVelocityEstimate();
                angularVelocity = velocityEstimator.GetAngularVelocityEstimate();
                position = velocityEstimator.transform.position;
            } else {
                velocity = Player.Instance.trackingOriginTransform.TransformVector(hand.controller.velocity);
                angularVelocity = Player.Instance.trackingOriginTransform.TransformVector(hand.controller.angularVelocity);
                position = hand.transform.position;
            }

            Vector3 r = transform.TransformPoint(rBody.centerOfMass) - position;
            rBody.velocity = velocity + Vector3.Cross(angularVelocity, r);

            if (rBody.velocity.magnitude > thrownVelocityMinimum) {
                hasThrown = true;
                rBody.velocity *= thrownVelocityModifier;
            }


            rBody.angularVelocity = angularVelocity;

            if (rBody.angularVelocity.magnitude > thrownVelocityMinimum)
                rBody.angularVelocity *= thrownVelocityModifier;

            // Make the object travel at the release velocity for the amount
            // of time it will take until the next fixed update, at which
            // point Unity physics will take over
            float timeUntilFixedUpdate = (Time.fixedDeltaTime + Time.fixedTime) - Time.time;
            transform.position += timeUntilFixedUpdate * velocity;
            float angle = Mathf.Rad2Deg * angularVelocity.magnitude;
            Vector3 axis = angularVelocity.normalized;
            transform.rotation *= Quaternion.AngleAxis(angle * timeUntilFixedUpdate, axis);
        }

        public override void HandAttachedUpdate (Hand hand) {
            //Trigger got released
            if (!hand.GetStandardInteractionButton()) {
                // Detach ourselves late in the frame.
                // This is so that any vehicles the player is attached to
                // have a chance to finish updating themselves.
                // If we detach now, our position could be behind what it
                // will be at the end of the frame, and the object may appear
                // to teleport behind the hand when the player releases it.
                StartCoroutine(LateDetach(hand));
            }

            if (attachEaseIn) {
                float t = Util.RemapNumberClamped(Time.time, attachTime, attachTime + snapAttachEaseInTime, 0.0f, 1.0f);
                if (t < 1.0f) {
                    t = snapAttachEaseInCurve.Evaluate(t);
                    transform.position = Vector3.Lerp(attachPosition, attachEaseInTransform.position, t);
                    transform.rotation = Quaternion.Lerp(attachRotation, attachEaseInTransform.rotation, t);
                } else if (!snapAttachEaseInCompleted) {
                    gameObject.SendMessage("OnThrowableAttachEaseInCompleted", hand, SendMessageOptions.DontRequireReceiver);
                    snapAttachEaseInCompleted = true;
                }
            }
        }

        private void OnCollisionEnter (Collision collision) {
            if (rBody == null || (transform.parent != null && transform.parent.GetComponent<Hand>()))
                return;

            Rigidbody other = collision.gameObject.GetComponent<Rigidbody>();

            if (other != null && other.velocity.magnitude > awakeImpactVelocity) {
                rBody.isKinematic = false;
            }

            if (damageSelfOnImpact && rBody.velocity.sqrMagnitude >= impactDamageMagnitude * impactDamageMagnitude) {
                damageable.Damage(1f, transform.position, rBody.velocity);
            }
        }

        private IEnumerator LateDetach (Hand hand) {
            yield return new WaitForEndOfFrame();

            hand.DetachObject(gameObject, restoreOriginalParent);
        }

        public override void OnHandFocusAcquired (Hand hand) {
            gameObject.SetActive(true);
            velocityEstimator.BeginEstimatingVelocity();
        }

        public override void OnHandFocusLost (Hand hand) {
            gameObject.SetActive(false);
            velocityEstimator.FinishEstimatingVelocity();
        }
    }
}

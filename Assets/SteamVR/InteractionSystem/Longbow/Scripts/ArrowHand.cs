using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    public class ArrowHand : Hoverable
    {
        [SerializeField] private List<GameObject> arrowPrefabs;
        [SerializeField] private Transform arrowNockTransform;
        [SerializeField] private float nockDistance = 0.1f;
        [SerializeField] private float lerpCompleteDistance = 0.08f;
        [SerializeField] private float rotationLerpThreshold = 0.15f;
        [SerializeField] private float positionLerpThreshold = 0.15f;
        [SerializeField] private int maxArrowCount = 10;
        [SerializeField] private SoundPlayOneshot arrowSpawnSound;
        [SerializeField] private string quiverTag;
        [SerializeField] private Transform handAttachPoint;

        [Header("Knocked movement")]
        [SerializeField] private Vector3 nockedPosition;
        [SerializeField] private float nockedTransitionTime;
        [SerializeField] private AnimationCurve nockedTransitionCurve;

        public Transform ArrowNockTransform { get { return arrowNockTransform; } }
        public Hand Hand { get { return hand; } }
        public bool Nocked { get { return nocked; } }
        public bool HasArrow { get { return currentArrow; } }

        private Hand hand;
        private HandModel handModel;
        private Longbow bow;

        private GameObject currentArrow;

        private bool allowArrowSpawn = true;
        private bool nocked;

        private bool inNockRange = false;
        private bool arrowLerpComplete = false;

        private AllowTeleportWhileAttachedToHand allowTeleport = null;

        private List<GameObject> arrowList;
        private GameObject currentArrowPrefab;
        private int currentArrowIndex;

        private GameObject handObject;
        private Quiver quiver;
        private BowAnchor bowAnchor;

        private bool movingArrowPoint;
        private Vector3 originalArrowPoint;

        private void Awake()
        {
            allowTeleport = GetComponent<AllowTeleportWhileAttachedToHand>();
            allowTeleport.teleportAllowed = true;
            allowTeleport.overrideHoverLock = false;

            arrowList = new List<GameObject>();
            currentArrowPrefab = arrowPrefabs[currentArrowIndex];

            quiver = GameObject.FindWithTag(quiverTag).GetComponent<Quiver>();
            bowAnchor = FindObjectOfType<BowAnchor>();

            originalArrowPoint = arrowNockTransform.parent.localPosition;
        }

        private void Update()
        {
            if (currentArrow == null)
                return;

            if (bow.Nocked == false && hand.GetStandardInteractionButtonUp())
            {
                FireArrow(hand.ArrowThrowModifier, (-hand.transform.up + hand.transform.forward) * 0.75f);

                currentArrow = null;
                hand.HoverUnlock(GetComponent<Interactable>());
                hand.otherHand.HoverUnlock(bow.GetComponent<Interactable>());
            }
        }

        private void NextArrow()
        {
            currentArrowIndex++;
            if (currentArrowIndex > arrowPrefabs.Count)
                currentArrowIndex = 0;

            currentArrowPrefab = arrowPrefabs[currentArrowIndex];

            if (currentArrow != null)
            {
                SimplePool.Despawn(currentArrow);
                currentArrow = InstantiateArrow();
            }
        }

        public void AttachArrow()
        {
            if (currentArrow != null) return;

            currentArrow = InstantiateArrow();

            if (handModel)
                handModel.ArrowAttached();
        }

        public void AttachArrow(GameObject arrow)
        {
            if (currentArrow != null) return;

            // Is the arrow on fire?
            FireSource fireSource = arrow.GetComponentInChildren<FireSource>();
            bool onFire = fireSource != null && fireSource.IsBurning;

            Destroy(arrow);
            AttachArrow();

            if (currentArrow && onFire)
            {
                fireSource = currentArrow.GetComponentInChildren<FireSource>();
                fireSource.FireExposure(fireSource.FireStrength);
            }

            //currentArrow = arrow;
            //arrow.transform.position = arrowNockTransform.position;
            //arrow.transform.rotation = arrowNockTransform.rotation;
            //arrow.transform.parent = arrowNockTransform;

            //Arrow a = arrow.GetComponent<Arrow>();

            //a.ResetReleased();
            ////Util.ResetTransform(arrow.transform);
            //a.ShaftRB.GetComponent<BoxCollider>().enabled = false;
            //a.ArrowHeadRB.GetComponent<BoxCollider>().enabled = false;
        }

        public override void OnAttachedToHand(Hand attachedHand)
        {
            hand = attachedHand;

            handModel = hand.GetComponent<HandModel>();
            FindBow();
        }

        private GameObject InstantiateArrow()
        {
            GameObject arrow = Instantiate(currentArrowPrefab, arrowNockTransform.position, arrowNockTransform.rotation) as GameObject;
            arrow.name = currentArrowPrefab.name;
            arrow.transform.parent = arrowNockTransform;
            Util.ResetTransform(arrow.transform);
            arrowList.Add(arrow);

            while (arrowList.Count > maxArrowCount)
            {
                GameObject oldArrow = arrowList[0];
                arrowList.RemoveAt(0);
                if (oldArrow)
                    SimplePool.Despawn(oldArrow);
            }
            return arrow;
        }

        public override void HandAttachedUpdate(Hand hand)
        {
            if (bow == null)
                FindBow();

            if (bow == null) return;

            if (currentArrow == null) return;


            float sqrDistanceToNockPosition = (bow.NockTransform.position - transform.parent.position).sqrMagnitude;

            // If there's an arrow spawned in the hand and it's not nocked yet
            if (!nocked && allowArrowSpawn)
            {
                // If we're close enough to nock position that we want to start arrow rotation lerp, do so
                if (sqrDistanceToNockPosition < rotationLerpThreshold * rotationLerpThreshold)
                {
                    float lerp = Util.RemapNumber(sqrDistanceToNockPosition, rotationLerpThreshold, lerpCompleteDistance, 0, 1);

                    arrowNockTransform.rotation = Quaternion.Lerp(arrowNockTransform.parent.rotation, bow.NockRestTransform.rotation, lerp);
                }
                else // Not close enough for rotation lerp, reset rotation
                {
                    arrowNockTransform.localRotation = Quaternion.identity;
                }

                // If we're close enough to the nock position that we want to start arrow position lerp, do so
                if (sqrDistanceToNockPosition < positionLerpThreshold * positionLerpThreshold)
                {
                    float posLerp = Util.RemapNumber(sqrDistanceToNockPosition, positionLerpThreshold, lerpCompleteDistance, 0, 1);

                    posLerp = Mathf.Clamp(posLerp, 0f, 1f);

                    arrowNockTransform.position = Vector3.Lerp(arrowNockTransform.parent.position, bow.NockRestTransform.position, posLerp);
                }
                else // Not close enough for position lerp, reset position
                {
                    arrowNockTransform.position = arrowNockTransform.parent.position;
                }

                // Give a haptic tick when lerp is visually complete
                if (sqrDistanceToNockPosition < lerpCompleteDistance * lerpCompleteDistance)
                {
                    if (!arrowLerpComplete)
                    {
                        arrowLerpComplete = true;

                        if (currentArrow)
                            hand.controller.TriggerHapticPulse(1000);
                    }
                }
                else
                {
                    if (arrowLerpComplete)
                    {
                        arrowLerpComplete = false;
                    }
                }

                // Allow nocking the arrow when controller is close enough
                if (sqrDistanceToNockPosition < nockDistance * nockDistance)
                {
                    if (!inNockRange)
                    {
                        inNockRange = true;
                        bow.ArrowInPosition();
                    }
                }
                else
                {
                    if (inNockRange)
                    {
                        inNockRange = false;
                    }
                }

                // If arrow is close enough to the nock position and we're pressing the trigger, and we're not nocked yet, Nock
                if ((sqrDistanceToNockPosition < nockDistance * nockDistance) && hand.GetStandardInteractionButton() && !nocked)
                {
                    nocked = true;
                    bow.StartNock(this);
                    hand.HoverLock(GetComponent<Interactable>());
                    allowTeleport.teleportAllowed = false;
                    currentArrow.transform.parent = bow.NockTransform;
                    Util.ResetTransform(currentArrow.transform);
                    Util.ResetTransform(arrowNockTransform);
                }
            }
            else if (nocked && movingArrowPoint == false)
            {
                StartCoroutine(LerpController.Instance.MoveLocalSpace(arrowNockTransform.parent, nockedPosition, nockedTransitionTime, nockedTransitionCurve));
                movingArrowPoint = true;
            }

            // If arrow is nocked, and we release the trigger
            if (currentArrow != null && nocked && (!hand.GetStandardInteractionButton() || hand.GetStandardInteractionButtonUp()))
            {
                if (bow.Pulled) // If bow is pulled back far enough, fire arrow, otherwise reset arrow in arrowhand
                    FireArrow(bow.GetArrowVelocity(), currentArrow.transform.forward);
                else
                {
                    arrowNockTransform.rotation = currentArrow.transform.rotation;
                    currentArrow.transform.parent = arrowNockTransform;
                    Util.ResetTransform(currentArrow.transform);
                    nocked = false;
                    bow.ReleaseNock();
                    hand.HoverUnlock(GetComponent<Interactable>());
                    allowTeleport.teleportAllowed = true;
                }
                bow.StartRotationLerp(); // Arrow is releasing from the bow, tell the bow to lerp back to controller rotation
            }
        }

        private void FireArrow(float arrowVelocity, Vector3 arrowDirection)
        {
            if (currentArrow == null) return;

            if (currentArrow)
                currentArrow.transform.parent = null;

            handModel.HandIdle();

            Arrow arrow = currentArrow.GetComponent<Arrow>();
            arrow.ShaftRB.isKinematic = false;
            arrow.ShaftRB.useGravity = true;
            arrow.ShaftRB.transform.GetComponent<BoxCollider>().enabled = true;

            arrow.ArrowHeadRB.isKinematic = false;
            arrow.ArrowHeadRB.useGravity = true;
            arrow.ArrowHeadRB.transform.GetComponent<BoxCollider>().enabled = true;

            arrow.ArrowHeadRB.AddForce(arrowDirection * arrowVelocity, ForceMode.VelocityChange);
            arrow.ArrowHeadRB.AddTorque(arrowDirection * 10);

            nocked = false;

            currentArrow.GetComponent<Arrow>().ArrowReleased(arrowVelocity);

            if(bow.gameObject.activeInHierarchy)
                bow.ArrowReleased();

            if(bowAnchor)
                bowAnchor.ArrowReleased(currentArrow);

            allowArrowSpawn = false;

            //Disabling Invoke will stop new arrows spawning without permission
            this.Invoke(EnableArrowSpawn, 0.5f);
            StartCoroutine(ArrowReleaseHaptics());

            currentArrow = null;
            allowTeleport.teleportAllowed = true;

            quiver.ResetArrowPulse();

            if (movingArrowPoint)
            {
                StartCoroutine(LerpController.Instance.MoveLocalSpace(arrowNockTransform.parent, originalArrowPoint, nockedTransitionTime, nockedTransitionCurve));
                movingArrowPoint = false;
            }
        }

        private void EnableArrowSpawn()
        {
            allowArrowSpawn = true;
        }

        private IEnumerator ArrowReleaseHaptics()
        {
            yield return new WaitForSeconds(0.05f);

            hand.otherHand.controller.TriggerHapticPulse(1500);
            yield return new WaitForSeconds(0.05f);

            hand.otherHand.controller.TriggerHapticPulse(800);
            yield return new WaitForSeconds(0.05f);

            hand.otherHand.controller.TriggerHapticPulse(500);
            yield return new WaitForSeconds(0.05f);

            hand.otherHand.controller.TriggerHapticPulse(300);
        }

        public override void OnHandFocusLost(Hand hand)
        {
            gameObject.SetActive(false);
        }

        public override void OnHandFocusAcquired(Hand hand)
        {
            gameObject.SetActive(true);
        }

        private void FindBow()
        {
            bow = hand.otherHand.GetComponentInChildren<Longbow>();
        }
    }
}

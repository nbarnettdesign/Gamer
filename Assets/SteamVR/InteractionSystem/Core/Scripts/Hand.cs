using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR;

namespace Valve.VR.InteractionSystem
{
    public class Hand : MonoBehaviour
    {
        public enum HandType
        {
            Left,
            Right,
            Any
        };

        // The flags used to determine how an object is attached to the hand.
        [Flags]
        public enum AttachmentFlags
        {
            SnapOnAttach = 1 << 0, // The object should snap to the position of the specified attachment point on the hand.
            DetachOthers = 1 << 1, // Other objects attached to this hand will be detached.
            DetachFromOtherHand = 1 << 2, // This object will be detached from the other hand.
            ParentToHand = 1 << 3, // The object will be parented to the hand.
        };

        public const AttachmentFlags defaultAttachmentFlags = AttachmentFlags.ParentToHand |
                                                              AttachmentFlags.DetachOthers |
                                                              AttachmentFlags.DetachFromOtherHand |
                                                              AttachmentFlags.SnapOnAttach;

        public const AttachmentFlags startingItemAttachmentFlags = AttachmentFlags.ParentToHand |
                                                              AttachmentFlags.DetachFromOtherHand |
                                                              AttachmentFlags.SnapOnAttach;


        public Hand otherHand;
        public HandType startingHandType;
        [SerializeField] private LayerMask environmentMask;

        public Transform hoverSphereTransform;
        public float hoverSphereRadius = 0.05f;
        public LayerMask hoverLayerMask = -1;
        public float hoverUpdateInterval = 0.1f;
        public float sphereCastRadius;
        public float maxThrowableDistance;
        public float grabbableVacuumSpeed;
        public float grabbableRotationSpeed;

        public Camera noSteamVRFallbackCamera;
        public float noSteamVRFallbackMaxDistanceNoItem = 10.0f;
        public float noSteamVRFallbackMaxDistanceWithItem = 0.5f;
        private float noSteamVRFallbackInteractorDistance = -1.0f;

        public SteamVR_Controller.Device controller;

        public GameObject controllerPrefab;
        private GameObject controllerObject = null;

        public bool showDebugText = false;
        public bool spewDebugText = false;

        [Header("Starting Objects")]
        [SerializeField] private ItemPackage startingHandObject;

        [Header("Arrow Throwing")]
        [SerializeField] private float arrowThrowModifier;

        public struct AttachedObject
        {
            public GameObject attachedObject;
            public GameObject originalParent;
            public bool isParentedToHand;
        }

        private List<AttachedObject> attachedObjects = new List<AttachedObject>();

        public List<AttachedObject> AttachedObjects
        {
            get { return attachedObjects; }
        }

        public bool hoverLocked { get; private set; }
        public bool IsEnabled { get { return isEnabled; } }
        public float ArrowThrowModifier { get { return velocityEstimator.GetVelocityEstimate().sqrMagnitude * arrowThrowModifier; } }

        private Interactable _hoveringInteractable;

        private TextMesh debugText;
        private int prevOverlappingColliders = 0;

        private const int ColliderArraySize = 16;
        private Collider[] overlappingColliders;

        private Player playerInstance;

        private GameObject applicationLostFocusObject;

        SteamVR_Events.Action inputFocusAction;

        private Camera rotationCamera;
        private bool isEnabled;
        private bool inEnvironment;

        private VelocityEstimator velocityEstimator;

        private VRDevice vRDevice;
        private HandModel handModel;
        private Quiver quiver;

        //-------------------------------------------------
        // The Interactable object this Hand is currently hovering over
        //-------------------------------------------------
        public Interactable hoveringInteractable
        {
            get { return _hoveringInteractable; }
            set
            {
                if (_hoveringInteractable != value)
                {
                    if (_hoveringInteractable != null)
                    {
                        HandDebugLog("HoverEnd " + _hoveringInteractable.gameObject);
                        Hoverable[] hoverables = _hoveringInteractable.gameObject.GetComponents<Hoverable>();

                        for (int i = 0; i < hoverables.Length; i++)
                        {
                            hoverables[i].OnHandHoverEnd(this);
                        }

                        //Note: The _hoveringInteractable can change after sending the OnHandHoverEnd message so we need to check it again before broadcasting this message
                        if (_hoveringInteractable != null)
                        {
                            List<ControllerHoverHighlight> highlights = new List<ControllerHoverHighlight>();

                            GetComponentsInChildren(highlights);

                            if (GetComponent<ControllerHoverHighlight>() != null)
                                highlights.Add(GetComponent<ControllerHoverHighlight>());

                            for (int i = 0; i < highlights.Count; i++)
                            {
                                highlights[i].OnParentHandHoverEnd(_hoveringInteractable);
                            }
                        }
                    }

                    _hoveringInteractable = value;

                    if (_hoveringInteractable != null)
                    {
                        HandDebugLog("HoverBegin " + _hoveringInteractable.gameObject);
                        Hoverable[] hoverables = _hoveringInteractable.gameObject.GetComponents<Hoverable>();

                        for (int i = 0; i < hoverables.Length; i++)
                        {
                            hoverables[i].OnHandHoverBegin(this);
                        }

                        //Note: The _hoveringInteractable can change after sending the OnHandHoverBegin message so we need to check it again before broadcasting this message
                        if (_hoveringInteractable != null)
                        {
                            List<ControllerHoverHighlight> highlights = new List<ControllerHoverHighlight>();

                            GetComponentsInChildren(highlights);

                            if (GetComponent<ControllerHoverHighlight>() != null)
                                highlights.Add(GetComponent<ControllerHoverHighlight>());

                            for (int i = 0; i < highlights.Count; i++)
                            {
                                highlights[i].OnParentHandHoverBegin(_hoveringInteractable);
                            }
                        }
                    }
                }
            }
        }

        //-------------------------------------------------
        // Active GameObject attached to this Hand
        //-------------------------------------------------
        public GameObject currentAttachedObject
        {
            get
            {
                CleanUpAttachedObjectStack();

                if (attachedObjects.Count > 0)
                    return attachedObjects[attachedObjects.Count - 1].attachedObject;

                return null;
            }
        }

        public Transform GetAttachmentTransform(string attachmentPoint = "")
        {
            Transform attachmentTransform = null;

            if (!string.IsNullOrEmpty(attachmentPoint))
                attachmentTransform = transform.Find(attachmentPoint);

            if (!attachmentTransform)
                attachmentTransform = transform;

            return attachmentTransform;
        }

        //-------------------------------------------------
        // Guess the type of this Hand
        //
        // If startingHandType is Hand.Left or Hand.Right, returns startingHandType.
        // If otherHand is non-null and both Hands are linked to controllers, returns
        // Hand.Left if this Hand is leftmost relative to the HMD, otherwise Hand.Right.
        // Otherwise, returns Hand.Any
        //-------------------------------------------------
        public HandType GuessCurrentHandType()
        {
            if (startingHandType == HandType.Left || startingHandType == HandType.Right)
                return startingHandType;

            if (startingHandType == HandType.Any && otherHand != null && otherHand.controller == null)
                return HandType.Right;

            if (controller == null || otherHand == null || otherHand.controller == null)
                return startingHandType;

            if (controller.index == SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost))
                return HandType.Left;

            return HandType.Right;
        }

        //-------------------------------------------------
        // Attach a GameObject to this GameObject
        //
        // objectToAttach - The GameObject to attach
        // flags - The flags to use for attaching the object
        // attachmentPoint - Name of the GameObject in the hierarchy of this Hand which should act as the attachment point for this GameObject
        //-------------------------------------------------
        public void AttachObject(GameObject objectToAttach, AttachmentFlags flags = defaultAttachmentFlags, string attachmentPoint = "")
        {
            if (isEnabled == false)
                return;

            if (objectToAttach.GetComponent<Arrow>())
            {
                if (currentAttachedObject && currentAttachedObject.GetComponent<ArrowHand>() && currentAttachedObject.GetComponent<ArrowHand>().HasArrow == false)
                {
                    currentAttachedObject.GetComponent<ArrowHand>().AttachArrow(objectToAttach);
                }

                return;
            }

            if (flags == 0)
                flags = defaultAttachmentFlags;

            //Make sure top object on stack is non-null
            CleanUpAttachedObjectStack();

            //Detach the object if it is already attached so that it can get re-attached at the top of the stack
            DetachObject(objectToAttach);

            //Detach from the other hand if requested
            if (((flags & AttachmentFlags.DetachFromOtherHand) == AttachmentFlags.DetachFromOtherHand) && otherHand)
                otherHand.DetachObject(objectToAttach);

            if ((flags & AttachmentFlags.DetachOthers) == AttachmentFlags.DetachOthers)
            {
                //Detach all the objects from the stack
                while (attachedObjects.Count > 0)
                {
                    DetachObject(attachedObjects[0].attachedObject);
                }
            }

            if (currentAttachedObject)
            {
                Hoverable[] hoverables = currentAttachedObject.GetComponents<Hoverable>();

                for (int i = 0; i < hoverables.Length; i++)
                {
                    hoverables[i].OnHandFocusLost(this);
                }
            }

            AttachedObject attachedObject = new AttachedObject();
            attachedObject.attachedObject = objectToAttach;
            attachedObject.originalParent = objectToAttach.transform.parent != null ? objectToAttach.transform.parent.gameObject : null;
            if ((flags & AttachmentFlags.ParentToHand) == AttachmentFlags.ParentToHand)
            {
                //Parent the object to the hand
                objectToAttach.transform.parent = GetAttachmentTransform(attachmentPoint);
                attachedObject.isParentedToHand = true;
            }
            else
                attachedObject.isParentedToHand = false;

            attachedObjects.Add(attachedObject);

            if ((flags & AttachmentFlags.SnapOnAttach) == AttachmentFlags.SnapOnAttach)
            {
                objectToAttach.transform.localPosition = Vector3.zero;
                objectToAttach.transform.localRotation = Quaternion.identity;
            }

            HandDebugLog("AttachObject " + objectToAttach);
            Hoverable[] h = objectToAttach.GetComponents<Hoverable>();
            for (int i = 0; i < h.Length; i++)
            {
                h[i].OnAttachedToHand(this);
            }

            UpdateHovering();
        }

        //-------------------------------------------------
        // Detach this GameObject from the attached object stack of this Hand
        //
        // objectToDetach - The GameObject to detach from this Hand
        //-------------------------------------------------
        public void DetachObject(GameObject objectToDetach, bool restoreOriginalParent = true)
        {
            if (isEnabled == false)
                return;

            int index = attachedObjects.FindIndex(l => l.attachedObject == objectToDetach);
            if (index != -1)
            {
                HandDebugLog("DetachObject " + objectToDetach);

                GameObject prevTopObject = currentAttachedObject;

                Transform parentTransform = null;
                if (attachedObjects[index].isParentedToHand)
                {
                    if (restoreOriginalParent && (attachedObjects[index].originalParent != null))
                    {
                        parentTransform = attachedObjects[index].originalParent.transform;
                    }
                    attachedObjects[index].attachedObject.transform.parent = parentTransform;
                }

                attachedObjects[index].attachedObject.SetActive(true);

                Hoverable[] hoverables = attachedObjects[index].attachedObject.GetComponents<Hoverable>();
                for (int i = 0; i < hoverables.Length; i++)
                {
                    hoverables[i].OnDetachedFromHand(this);
                }

                attachedObjects.RemoveAt(index);

                GameObject newTopObject = currentAttachedObject;

                //Give focus to the top most object on the stack if it changed
                if (newTopObject != null && newTopObject != prevTopObject)
                {
                    newTopObject.SetActive(true);

                    Hoverable[] h = newTopObject.GetComponents<Hoverable>();
                    for (int i = 0; i < h.Length; i++)
                    {
                        h[i].OnHandFocusAcquired(this);
                    }
                }
            }

            CleanUpAttachedObjectStack();
        }

        //-------------------------------------------------
        // Get the world velocity of the VR Hand.
        // Note: controller velocity value only updates on controller events (Button but and down) so good for throwing
        //-------------------------------------------------
        public Vector3 GetTrackedObjectVelocity()
        {
            if (controller != null)
                return transform.parent.TransformVector(controller.velocity);
            return Vector3.zero;
        }

        //-------------------------------------------------
        // Get the world angular velocity of the VR Hand.
        // Note: controller velocity value only updates on controller events (Button but and down) so good for throwing
        //-------------------------------------------------
        public Vector3 GetTrackedObjectAngularVelocity()
        {
            if (controller != null)
                return transform.parent.TransformVector(controller.angularVelocity);
            return Vector3.zero;
        }

        private void CleanUpAttachedObjectStack()
        {
            attachedObjects.RemoveAll(l => l.attachedObject == null);
        }

        private void Awake()
        {
            inputFocusAction = SteamVR_Events.InputFocusAction(OnInputFocus);

            if (hoverSphereTransform == null)
                hoverSphereTransform = transform;

            applicationLostFocusObject = new GameObject("_application_lost_focus");
            applicationLostFocusObject.transform.parent = transform;
            applicationLostFocusObject.SetActive(false);

            velocityEstimator = GetComponent<VelocityEstimator>();
        }

        IEnumerator Start()
        {
            isEnabled = true;

            // save off player instance
            playerInstance = Player.Instance;
            if (!playerInstance)
                Debug.LogError("No player instance found in Hand Start()");

            // allocate array for colliders
            overlappingColliders = new Collider[ColliderArraySize];

            // We are a "no SteamVR fallback hand" if we have this camera set
            // we'll use the right mouse to look around and left mouse to interact
            // - don't need to find the device
            if (noSteamVRFallbackCamera)
            {
                rotationCamera = noSteamVRFallbackCamera;
                yield break;
            }

            rotationCamera = Camera.main;

            // Acquire the correct device index for the hand we want to be
            // Also for the other hand if we get there first
            while (true)
            {
                // Don't need to run this every frame
                yield return new WaitForSeconds(1.0f);

                // We have a controller now, break out of the loop!
                if (controller != null)
                    break;

                // Initialize both hands simultaneously
                if (startingHandType == HandType.Left || startingHandType == HandType.Right)
                {
                    // Left/right relationship.
                    // Wait until we have a clear unique left-right relationship to initialize.
                    int leftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
                    int rightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                    if (leftIndex == -1 || rightIndex == -1 || leftIndex == rightIndex)
                    {
                        //Debug.Log( string.Format( "...Left/right hand relationship not yet established: leftIndex={0}, rightIndex={1}", leftIndex, rightIndex ) );
                        continue;
                    }

                    int myIndex = (startingHandType == HandType.Right) ? rightIndex : leftIndex;
                    int otherIndex = (startingHandType == HandType.Right) ? leftIndex : rightIndex;

                    InitController(myIndex);
                    if (otherHand)
                        otherHand.InitController(otherIndex);
                }
                else
                {
                    // No left/right relationship. Just wait for a connection
                    var vr = SteamVR.instance;
                    for (int i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
                    {
                        if (vr.hmd.GetTrackedDeviceClass((uint)i) != ETrackedDeviceClass.Controller)
                        {
                            //Debug.Log( string.Format( "Hand - device {0} is not a controller", i ) );
                            continue;
                        }

                        var device = SteamVR_Controller.Input(i);
                        if (!device.valid)
                            //Debug.Log( string.Format( "Hand - device {0} is not valid", i ) );
                            continue;

                        if ((otherHand != null) && (otherHand.controller != null))
                        {
                            // Other hand is using this index, so we cannot use it.
                            if (i == (int)otherHand.controller.index)
                                //Debug.Log( string.Format( "Hand - device {0} is owned by the other hand", i ) );
                                continue;
                        }

                        InitController(i);
                    }
                }
            }

            if (startingHandObject)
            {
                AttachObject(Instantiate(startingHandObject.ItemPrefab), startingItemAttachmentFlags, "");

                if ((startingHandObject.OtherHandItemPrefab != null) && (otherHand.controller != null))
                {
                    otherHand.AttachObject(Instantiate(startingHandObject.OtherHandItemPrefab), startingItemAttachmentFlags, "");
                }
            }

            vRDevice = GetVRDevice.GetDevice(XRDevice.model);
            quiver = FindObjectOfType<Quiver>();
            handModel = GetComponent<HandModel>();
        }

        private void UpdateHovering()
        {
            if (currentAttachedObject == null)
                return;

            if ((noSteamVRFallbackCamera == null) && (controller == null)) return;

            if (hoverLocked || isEnabled == false) return;

            if (applicationLostFocusObject.activeSelf) return;

            if ((Teleport.instance != null && Teleport.instance.Disabled == false && Teleport.instance.TeleportButtonHeld) || quiver && quiver.HandIn) return;

            if (currentAttachedObject != controllerObject && currentAttachedObject.GetComponent<ArrowHand>() &&
                currentAttachedObject.GetComponent<ArrowHand>().HasArrow) return;

            if (currentAttachedObject != controllerObject && currentAttachedObject.GetComponent<Longbow>()) return;

            float sqrClosestDistance = float.MaxValue;
            Interactable closestInteractable = null;

            // Pick the closest hovering
            float flHoverRadiusScale = playerInstance.transform.lossyScale.x;
            float flScaledSphereRadius = hoverSphereRadius * flHoverRadiusScale;

            // if we're close to the floor, increase the radius to make things easier to pick up
            float handDiff = Mathf.Abs(transform.position.y - playerInstance.trackingOriginTransform.position.y);
            float boxMult = Util.RemapNumberClamped(handDiff, 0.0f, 0.5f * flHoverRadiusScale, 5.0f, 1.0f) * flHoverRadiusScale;

            // null out old vals
            for (int i = 0; i < overlappingColliders.Length; ++i)
            {
                overlappingColliders[i] = null;
            }

            Physics.OverlapBoxNonAlloc(
                hoverSphereTransform.position - new Vector3(0, flScaledSphereRadius * boxMult - flScaledSphereRadius, 0),
                new Vector3(flScaledSphereRadius, flScaledSphereRadius * boxMult * 2.0f, flScaledSphereRadius),
                overlappingColliders,
                Quaternion.identity,
                hoverLayerMask.value
            );

            RaycastHit[] hitColliders = Physics.SphereCastAll(hoverSphereTransform.position, sphereCastRadius,
                rotationCamera.transform.forward, maxThrowableDistance, hoverLayerMask);

            int arrayPosition = 0;
            for (int i = 0; i < overlappingColliders.Length; i++)
            {
                if (overlappingColliders[i] == null)
                {
                    arrayPosition = i;
                    break;
                }
            }

            foreach (RaycastHit hit in hitColliders)
            {
                if (hit.collider.GetComponent<Interactable>() != null ||
                    currentAttachedObject && currentAttachedObject.GetComponent<ArrowHand>() && hit.collider.GetComponent<Arrow>())
                {
                    if (hit.collider.GetComponent<Throwable>())
                    {
                        FireSource f = hit.collider.GetComponentInChildren<FireSource>();

                        if (f != null && f.IsBurning && f.Damaging)
                            continue;
                    }

                    if (arrayPosition < overlappingColliders.Length)
                    {
                        if (otherHand != null && otherHand.attachedObjects.Exists(t => t.attachedObject == hit.transform.gameObject) == false)
                        {
                            overlappingColliders[arrayPosition] = hit.collider;
                            arrayPosition++;
                        }
                        else if (transform.name == "FallbackHand")
                        {
                            overlappingColliders[arrayPosition] = hit.collider;
                            arrayPosition++;
                        }
                    }
                    else
                        break;
                }
            }

            // DebugVar
            int iActualColliderCount = 0;

            foreach (Collider collider in overlappingColliders)
            {
                if (collider == null)
                    continue;

                Interactable contacting = collider.GetComponentInParent<Interactable>();

                if (contacting == null)
                    continue;

                // Ignore this collider for hovering
                IgnoreHovering ignore = collider.GetComponent<IgnoreHovering>();
                if (ignore != null)
                {
                    if (ignore.onlyIgnoreHand == null || ignore.onlyIgnoreHand == this)
                        continue;
                }

                // Can't hover over the object if it's attached
                if (attachedObjects.FindIndex(l => l.attachedObject == contacting.gameObject) != -1)
                    continue;

                // Occupied by another hand, so we can't touch it
                if (otherHand && otherHand.hoveringInteractable == contacting)
                    continue;

                // Best candidate so far...
                float sqrDistance = (hoverSphereTransform.position - contacting.transform.position).sqrMagnitude;

                if (sqrDistance < sqrClosestDistance)
                {
                    sqrClosestDistance = sqrDistance;
                    closestInteractable = contacting;
                }
                iActualColliderCount++;
            }

            // Hover on this one
            hoveringInteractable = closestInteractable;

            if (handModel)
            {
                if (hoveringInteractable)
                    handModel.HandHover();
                else if (handModel.CurrentHandState == HandState.Open)
                    handModel.HandIdle();      
            }

            if (iActualColliderCount > 0 && iActualColliderCount != prevOverlappingColliders)
            {
                prevOverlappingColliders = iActualColliderCount;
                HandDebugLog("Found " + iActualColliderCount + " overlapping colliders.");
            }
        }

        private void UpdateNoSteamVRFallback()
        {
            if (noSteamVRFallbackCamera)
            {
                Ray ray = noSteamVRFallbackCamera.ScreenPointToRay(Input.mousePosition);

                if (attachedObjects.Count > 0)
                {
                    // Holding down the mouse:
                    // move around a fixed distance from the camera
                    transform.position = ray.origin + noSteamVRFallbackInteractorDistance * ray.direction;
                }
                else
                {
                    // Not holding down the mouse:
                    // cast out a ray to see what we should mouse over

                    // Don't want to hit the hand and anything underneath it
                    // So move it back behind the camera when we do the raycast
                    Vector3 oldPosition = transform.position;
                    transform.position = noSteamVRFallbackCamera.transform.forward * (-1000.0f);

                    RaycastHit raycastHit;
                    if (Physics.Raycast(ray, out raycastHit, noSteamVRFallbackMaxDistanceNoItem))
                    {
                        transform.position = raycastHit.point;

                        // Remember this distance in case we click and drag the mouse
                        noSteamVRFallbackInteractorDistance = Mathf.Min(noSteamVRFallbackMaxDistanceNoItem, raycastHit.distance);
                    }
                    else if (noSteamVRFallbackInteractorDistance > 0.0f)
                    {
                        // Move it around at the distance we last had a hit
                        transform.position = ray.origin + Mathf.Min(noSteamVRFallbackMaxDistanceNoItem, noSteamVRFallbackInteractorDistance) * ray.direction;
                    }
                    else
                    {
                        // Didn't hit, just leave it where it was
                        transform.position = oldPosition;
                    }
                }
            }
        }

        private void UpdateDebugText()
        {
            if (showDebugText)
            {
                if (debugText == null)
                {
                    debugText = new GameObject("_debug_text").AddComponent<TextMesh>();
                    debugText.fontSize = 120;
                    debugText.characterSize = 0.001f;
                    debugText.transform.parent = transform;

                    debugText.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                }

                if (GuessCurrentHandType() == HandType.Right)
                {
                    debugText.transform.localPosition = new Vector3(-0.05f, 0.0f, 0.0f);
                    debugText.alignment = TextAlignment.Right;
                    debugText.anchor = TextAnchor.UpperRight;
                }
                else
                {
                    debugText.transform.localPosition = new Vector3(0.05f, 0.0f, 0.0f);
                    debugText.alignment = TextAlignment.Left;
                    debugText.anchor = TextAnchor.UpperLeft;
                }

                debugText.text = string.Format(
                    "Hovering: {0}\n" +
                    "Hover Lock: {1}\n" +
                    "Attached: {2}\n" +
                    "Total Attached: {3}\n" +
                    "Type: {4}\n",
                    (hoveringInteractable ? hoveringInteractable.gameObject.name : "null"),
                    hoverLocked,
                    (currentAttachedObject ? currentAttachedObject.name : "null"),
                    attachedObjects.Count,
                    GuessCurrentHandType().ToString());
            }
            else
            {
                if (debugText != null)
                    Destroy(debugText.gameObject);
            }
        }

        private void OnEnable()
        {
            inputFocusAction.enabled = true;

            // Stagger updates between hands
            float hoverUpdateBegin = ((otherHand != null) && (otherHand.GetInstanceID() < GetInstanceID())) ? (0.5f * hoverUpdateInterval) : (0.0f);
            InvokeRepeating("UpdateHovering", hoverUpdateBegin, hoverUpdateInterval);
            InvokeRepeating("UpdateDebugText", hoverUpdateBegin, hoverUpdateInterval);
        }

        private void OnDisable()
        {
            inputFocusAction.enabled = false;
            CancelInvoke();
        }

        private void Update()
        {
            if (isEnabled == false && inEnvironment == false)
                CheckDisabled();

            if (isEnabled == false)
                return;

            UpdateNoSteamVRFallback();

            if (currentAttachedObject)
            {
                if (currentAttachedObject.GetComponent<Throwable>() != null)
                {
                    float step;
                    if (currentAttachedObject.transform.position != transform.position)
                    {
                        step = grabbableVacuumSpeed * Time.deltaTime;
                        currentAttachedObject.transform.position = Vector3.MoveTowards(currentAttachedObject.transform.position, transform.position, step);

                        step = grabbableRotationSpeed * Time.deltaTime;
                        currentAttachedObject.transform.localRotation = Quaternion.RotateTowards(currentAttachedObject.transform.localRotation, Quaternion.Euler(transform.forward), step);

                        handModel.HandGrab();
                    } else
                        handModel.HandClosed();
                }

                Hoverable[] hoverables = currentAttachedObject.GetComponents<Hoverable>();
                for (int i = 0; i < hoverables.Length; i++)
                {
                    hoverables[i].HandAttachedUpdate(this);
                }
            }

            if (hoveringInteractable)
            {
                Hoverable[] hoverables = hoveringInteractable.GetComponents<Hoverable>();
                for (int i = 0; i < hoverables.Length; i++)
                {
                    hoverables[i].HandHoverUpdate(this);
                }
            }
        }

        private void LateUpdate()
        {
            //Re-attach the controller if nothing else is attached to the hand
            if (controllerObject != null && attachedObjects.Count == 0)
            {
                AttachObject(controllerObject);
            }
        }

        private void OnInputFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                DetachObject(applicationLostFocusObject, true);
                applicationLostFocusObject.SetActive(false);
                UpdateHandPoses();
                UpdateHovering();

                List<Hoverable> hoverables = new List<Hoverable>();
                GetComponentsInChildren(hoverables);
                hoverables.AddRange(GetComponents<Hoverable>().ToList());
                for (int i = 0; i < hoverables.Count; i++)
                {
                    hoverables[i].OnParentHandInputFocusAcquired();
                }
            }
            else
            {
                applicationLostFocusObject.SetActive(true);
                AttachObject(applicationLostFocusObject, AttachmentFlags.ParentToHand);

                List<Hoverable> hoverables = new List<Hoverable>();
                GetComponentsInChildren(hoverables);
                hoverables.AddRange(GetComponents<Hoverable>().ToList());
                for (int i = 0; i < hoverables.Count; i++)
                {
                    hoverables[i].OnParentHandInputFocusLost();
                }
            }
        }

        private void FixedUpdate()
        {
            UpdateHandPoses();
        }

        private void CheckDisabled()
        {
            //Get direction to hand
            Vector3 direction = transform.position - playerInstance.trackingOriginTransform.position;
            float distance = direction.magnitude;
            //direction

            RaycastHit hit;

            if (Physics.Linecast(transform.position, playerInstance.headCollider.transform.position, out hit, environmentMask) == false)
            {
                isEnabled = true;

                foreach (AttachedObject obj in attachedObjects)
                {
                    Hoverable h = obj.attachedObject.GetComponent<Hoverable>();

                    if (h == null)
                        continue;

                    h.Enable();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 10)
            {
                isEnabled = false;
                inEnvironment = true;

                foreach (AttachedObject obj in attachedObjects)
                {
                    Hoverable h = obj.attachedObject.GetComponent<Hoverable>();

                    if (h == null)
                        continue;

                    h.Disable();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == 10)
            {
                inEnvironment = false;

                CheckDisabled();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.5f, 1.0f, 0.5f, 0.9f);
            Transform sphereTransform = hoverSphereTransform ? hoverSphereTransform : this.transform;
            Gizmos.DrawWireSphere(sphereTransform.position, hoverSphereRadius);
        }

        private void HandDebugLog(string msg)
        {
            if (spewDebugText)
                Debug.Log("Hand (" + name + "): " + msg);
        }

        private void UpdateHandPoses()
        {
            if (controller != null)
            {
                SteamVR vr = SteamVR.instance;
                if (vr != null)
                {
                    var pose = new TrackedDevicePose_t();
                    var gamePose = new TrackedDevicePose_t();
                    var err = vr.compositor.GetLastPoseForTrackedDeviceIndex(controller.index, ref pose, ref gamePose);
                    if (err == EVRCompositorError.None)
                    {
                        var t = new SteamVR_Utils.RigidTransform(gamePose.mDeviceToAbsoluteTracking);
                        transform.localPosition = t.pos;
                        transform.localRotation = t.rot;
                    }
                }
            }
        }

        //-------------------------------------------------
        // Continue to hover over this object indefinitely, whether or not the Hand moves out of its interaction trigger volume.
        //
        // interactable - The Interactable to hover over indefinitely.
        //-------------------------------------------------
        public void HoverLock(Interactable interactable)
        {
            HandDebugLog("HoverLock " + interactable);
            hoverLocked = true;
            hoveringInteractable = interactable;
        }

        //-------------------------------------------------
        // Stop hovering over this object indefinitely.
        //
        // interactable - The hover-locked Interactable to stop hovering over indefinitely.
        //-------------------------------------------------
        public void HoverUnlock(Interactable interactable)
        {
            HandDebugLog("HoverUnlock " + interactable);
            if (hoveringInteractable == interactable)
            {
                hoverLocked = false;
            }
        }

        //-------------------------------------------------
        // Was the standard interaction button just pressed? In VR, this is a trigger press. In 2D fallback, this is a mouse left-click.
        //-------------------------------------------------
        public bool GetStandardInteractionButtonDown()
        {
            if (isEnabled == false)
                return false;

            if (noSteamVRFallbackCamera)
                return Input.GetMouseButtonDown(0);
            else if (controller != null)
            {
                if (vRDevice == VRDevice.Vive)
                    return controller.GetHairTriggerDown();
                else if (vRDevice == VRDevice.Occulus)
                    return controller.GetPressDown(EVRButtonId.k_EButton_Grip) || controller.GetHairTriggerDown();
            }

            return false;
        }

        //-------------------------------------------------
        // Was the standard interaction button just released? In VR, this is a trigger press. In 2D fallback, this is a mouse left-click.
        //-------------------------------------------------
        public bool GetStandardInteractionButtonUp()
        {
            if (isEnabled == false)
                return false;

            if (noSteamVRFallbackCamera)
                return Input.GetMouseButtonUp(0);
            else if (controller != null)
            {
                if (vRDevice == VRDevice.Vive)
                    return controller.GetHairTriggerUp();
                else if (vRDevice == VRDevice.Occulus)
                    return controller.GetPressUp(EVRButtonId.k_EButton_Grip) || controller.GetHairTriggerUp();
            }

            return false;
        }

        //-------------------------------------------------
        // Is the standard interaction button being pressed? In VR, this is a trigger press. In 2D fallback, this is a mouse left-click.
        //-------------------------------------------------
        public bool GetStandardInteractionButton()
        {
            if (isEnabled == false)
                return false;

            if (noSteamVRFallbackCamera)
                return Input.GetMouseButton(0);
            else if (controller != null)
            {
                if (vRDevice == VRDevice.Vive)
                    return controller.GetHairTrigger();
                else if (vRDevice == VRDevice.Occulus)
                    return controller.GetPress(EVRButtonId.k_EButton_Grip) || controller.GetHairTrigger();
            }

            return false;
        }

        //-------------------------------------------------
        private void InitController(int index)
        {
            if (controller == null)
            {
                controller = SteamVR_Controller.Input(index);

                HandDebugLog("Hand " + name + " connected with device index " + controller.index);

                controllerObject = SimplePool.Spawn(controllerPrefab, transform.position, controllerPrefab.transform.rotation);
                controllerObject.SetActive(true);
                controllerObject.name = controllerPrefab.name + "_" + name;
                controllerObject.layer = gameObject.layer;
                controllerObject.tag = gameObject.tag;

                AttachObject(controllerObject);
                controller.TriggerHapticPulse(800);

                // If the player's scale has been changed the object to attach will be the wrong size.
                // To fix this we change the object's scale back to its original, pre-attach scale.
                controllerObject.transform.localScale = controllerPrefab.transform.localScale;

                // let child objects know we've initialized
                List<Hoverable> hoverables = new List<Hoverable>();
                GetComponentsInChildren(hoverables);
                for (int i = 0; i < hoverables.Count; i++)
                {
                    hoverables[i].OnHandInitialized(index);
                }
            }
        }
    }

#if UNITY_EDITOR
    //-------------------------------------------------------------------------
    [UnityEditor.CustomEditor(typeof(Hand))]
    public class HandEditor : UnityEditor.Editor
    {
        //-------------------------------------------------
        // Custom Inspector GUI allows us to click from within the UI
        //-------------------------------------------------
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Hand hand = (Hand)target;

            if (hand.otherHand)
            {
                if (hand.otherHand.otherHand != hand)
                {
                    UnityEditor.EditorGUILayout.HelpBox("The otherHand of this Hand's otherHand is not this Hand.", UnityEditor.MessageType.Warning);
                }

                if (hand.startingHandType == Hand.HandType.Left && hand.otherHand.startingHandType != Hand.HandType.Right)
                {
                    UnityEditor.EditorGUILayout.HelpBox("This is a left Hand but otherHand is not a right Hand.", UnityEditor.MessageType.Warning);
                }

                if (hand.startingHandType == Hand.HandType.Right && hand.otherHand.startingHandType != Hand.HandType.Left)
                {
                    UnityEditor.EditorGUILayout.HelpBox("This is a right Hand but otherHand is not a left Hand.", UnityEditor.MessageType.Warning);
                }

                if (hand.startingHandType == Hand.HandType.Any && hand.otherHand.startingHandType != Hand.HandType.Any)
                {
                    UnityEditor.EditorGUILayout.HelpBox("This is an any-handed Hand but otherHand is not an any-handed Hand.", UnityEditor.MessageType.Warning);
                }
            }
        }
    }
#endif
}

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class Interactable : Hoverable
    {
        [Header("Hover Materials")]
        [SerializeField]
        private Material handHoverMaterial;
        [SerializeField] private Material arrowHoverMaterial;

        protected Renderer myRenderer;
        private Material[] originalMaterials;

        public delegate void OnAttachedToHandDelegate(Hand hand);
        public delegate void OnDetachedFromHandDelegate(Hand hand);

        [HideInInspector] public event OnAttachedToHandDelegate onAttachedToHand;
        [HideInInspector] public event OnDetachedFromHandDelegate onDetachedFromHand;

        protected virtual void Start()
        {
            myRenderer = GetComponent<Renderer>();

            if (myRenderer == null)
            {
                myRenderer = GetComponentInChildren<Renderer>();
            }

            if (myRenderer == null)
                return;

            originalMaterials = myRenderer.materials;
        }

        public override void OnHandHoverBegin(Hand hand)
        {
            if (handHoverMaterial != null && myRenderer != null)
            {
                Material[] m = myRenderer.materials;
                for (int i = 0; i < m.Length; i++)
                {
                    m[i] = handHoverMaterial;
                }

                myRenderer.materials = m;
            }
        }

        public override void OnHandHoverEnd(Hand hand)
        {
            if (myRenderer != null)
            {
                myRenderer.materials = originalMaterials;
            }
        }

        public override void OnAttachedToHand(Hand hand)
        {
            if (onAttachedToHand != null)
                onAttachedToHand.Invoke(hand);

            OnHandHoverEnd(hand);
        }

        public override void OnDetachedFromHand(Hand hand)
        {
            if (onDetachedFromHand != null)
                onDetachedFromHand.Invoke(hand);
        }

        public override void OnBowHover()
        {
            if (arrowHoverMaterial != null && myRenderer != null)
            {
                Material[] m = myRenderer.materials;
                for (int i = 0; i < m.Length; i++)
                {
                    m[i] = arrowHoverMaterial;
                }

                myRenderer.materials = m;
            }
        }

        public override void OnBowHoverExit()
        {
            if (myRenderer != null)
            {
                myRenderer.materials = originalMaterials;
            }
        }
    }
}

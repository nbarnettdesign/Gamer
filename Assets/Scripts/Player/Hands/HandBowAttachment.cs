using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class HandBowAttachment : MonoBehaviour {
    [SerializeField] private GameObject leftHandModel;
    [SerializeField] private GameObject rightHandModel;

    private GameObject leftHand;
    private GameObject rightHand;
    private Longbow bow;

	private void Awake () {
        bow = GetComponentInParent<Longbow>();

        leftHand = SimplePool.Spawn(leftHandModel, transform.position, transform.rotation);
        leftHand.transform.SetParent(transform);
        leftHand.SetActive(false);

        rightHand = SimplePool.Spawn(rightHandModel, transform.position, transform.rotation);
        rightHand.transform.SetParent(transform);
        rightHand.SetActive(false);
    }

    public void SwitchHands()
    {
        if(bow == null)
            bow = GetComponentInParent<Longbow>();

        if (bow.Hand.startingHandType == Hand.HandType.Left)
        {
            leftHand.SetActive(true);

            if (rightHand.activeInHierarchy)
                rightHand.SetActive(false);
        }
        else
        {
            rightHand.SetActive(true);

            if (leftHand.activeInHierarchy)
                leftHand.SetActive(false);
        }
    }
}

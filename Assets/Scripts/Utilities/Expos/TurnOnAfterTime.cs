using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class TurnOnAfterTime : MonoBehaviour
{
    [SerializeField] private float minsTilTurnOn;
    [SerializeField] private GameObject turnOnObject;

    private float resetTime;
    private float resetCount;

    private Hand[] hands;
    private Vector3[] previousHandPos;

    private void Start()
    {
        if (turnOnObject && turnOnObject.activeInHierarchy)
            turnOnObject.SetActive(false);

        resetTime = minsTilTurnOn * 60f;

        hands = Player.Instance.hands;

        previousHandPos = new Vector3[2];
        ResetPositions();
    }

    private void Update()
    {
        CheckHandPosition();
    }

    private void CheckHandPosition()
    {
        if (hands == null)
            return;

        if (hands[0].transform.position == previousHandPos[0] && hands[1].transform.position == previousHandPos[1])
        {
            resetCount += Time.deltaTime;

            if (resetCount >= resetTime)
            {
                turnOnObject.SetActive(true);
            }
        }
        else
        {
            resetCount = 0f;
            ResetPositions();

            if (turnOnObject.activeInHierarchy)
                turnOnObject.SetActive(false);
        }
    }

    private void ResetPositions()
    {
        previousHandPos[0] = hands[0].transform.position;
        previousHandPos[1] = hands[1].transform.position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ResetAfterTime : MonoBehaviour
{
    [SerializeField] private float minsTilReset;
    [SerializeField] private bool resetThisLevel;
    [SerializeField] private string levelName;

    private float resetTime;
    private float resetCount;

    private Hand[] hands;
    private Vector3[] previousHandPos;

    private void Start()
    {
        resetTime = minsTilReset * 60f;

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
                if (resetThisLevel)
                    GameController.Instance.ReloadLevel();
                else
                    GameController.Instance.LoadLevel(levelName);
            }
        }
        else
        {
            resetCount = 0f;
            ResetPositions();
        }
    }

    private void ResetPositions()
    {
        previousHandPos[0] = hands[0].transform.position;
        previousHandPos[1] = hands[1].transform.position;
    }
}

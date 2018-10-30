using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandModelState : MonoBehaviour
{
    [SerializeField] private HandState startingState;
    [SerializeField] private float stateTransitionTime;
    [SerializeField] private float stateReturnTime;

    public HandState CurrentState { get { return currentState; } }

    private HandShapes hand;
    private HandState currentState;
    private HandState previousState;

    private void Start()
    {
        hand = GetComponentInChildren<HandShapes>();

        currentState = startingState;

        if (hand)
            hand.AdjustWeight(currentState, 0f); 
    }

    public void UpdateState(HandState newState)
    {
        if (newState == currentState)
            return;

        previousState = currentState;
        currentState = newState;
        UpdateState();
    }

    private void UpdateState()
    { 
        hand.AdjustWeight(currentState, stateTransitionTime);

        if (previousState != currentState)
            hand.AdjustWeight(previousState, stateReturnTime, 0f);
    }
}

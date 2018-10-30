using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class CombatAudioTracker : MonoBehaviour
{
    [SerializeField] private bool waitingForExternal;
    [SerializeField] private bool singleEvent;
    [SerializeField] private float checkTime;

    private bool inCombat;
    private bool eventStarted;
    private bool eventFinished;
    private float checkCount;

    private void Update()
    {
        CheckAudio();
    }

    public void EventStarted()
    {
        if (singleEvent && eventFinished)
            return;

        eventStarted = true;
        eventFinished = false;
    }

    public void EventFinished()
    {
        eventFinished = true;
        eventStarted = false;
    }

    private void CheckAudio()
    {
        if (checkCount < checkTime)
        {
            checkCount += Time.deltaTime;
            return;
        }

        checkCount = 0f;

        if (inCombat == false)
            CheckInCombat();
        else
            CheckCombatEnd();
    }

    private void CheckInCombat()
    {
        // Check all our children and see if they are in combat
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<BehaviorTree>())
            {
                var value = (SharedBool)transform.GetChild(i).GetComponent<BehaviorTree>().GetVariable("SeenPlayer");

                if (value.Value)
                {
                    inCombat = true;
                    break;
                }
            }
        }

        // Are we waiting for an event and has it started?
        // If we are already in combat, just skip this
        if (inCombat == false && waitingForExternal && eventStarted)
            inCombat = true;

        if (inCombat == false)
            return;

        BackgroundMusicController.Instance.FadeInCombat();
    }

    private void CheckCombatEnd()
    {
        // Check if we have children
        // Check if we are waiting for an external event to finish
        if (transform.childCount <= 0 && (waitingForExternal == false || (waitingForExternal && eventFinished)))
            inCombat = false;

        // If no to both scale out music
        if (inCombat)
            return;

        BackgroundMusicController.Instance.FadeOutCombat();
    }
}

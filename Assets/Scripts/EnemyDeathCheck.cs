using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDeathCheck : MonoBehaviour {

	[SerializeField] private UnityEvent onEnemiesDead;



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

		CheckIfDead ();
	}
		

	private void CheckIfDead()
	{

		if (transform.childCount <= 0 && (waitingForExternal == false || (waitingForExternal && eventFinished)))
			onEnemiesDead.Invoke ();

	}

}

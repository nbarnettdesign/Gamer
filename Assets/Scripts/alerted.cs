using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class alerted : MonoBehaviour {

	public NavMeshAgent nma;	
	private GameObject player;
    private Animator anim;

	// Use this for initialization
	void Start () {
		nma = GetComponent<NavMeshAgent> ();
		player = GameController.Instance.Player.gameObject;
        anim = transform.GetChild(0).GetComponent<Animator>();
	}

    private void Update()
    {
        if (anim.GetBool("Patrolling"))
        {


            if (nma.remainingDistance <= 0.2f)
            {
                
                anim.SetBool("Idle", true);
                anim.SetBool("Walk", false);
            }
            else
            {
                
                anim.SetBool("Idle", false);
                anim.SetBool("Walk", true);
            }
        }
      
    }


    public void AlertedToTheFight()
	{
        nma.SetDestination(player.transform.position);
        anim.SetBool("Idle", false);
        anim.SetBool("Unsheathed", true);
        anim.SetBool("Run", true);
        NavMeshPath path = new NavMeshPath();
        nma.CalculatePath(player.transform.position, path);
        if (path.status != NavMeshPathStatus.PathPartial)
        {
            Debug.Log("cant Get there");
        }
    }

}

using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(BehaviorTree))]
public class AnimController : MonoBehaviour
{
    [SerializeField] private Animator animController;
    [SerializeField] private float minDestinationDistance;
    [SerializeField] private float minAttackingDistance;
    [SerializeField] private float attackTime;

    [Header("Sounds")]
    [SerializeField] private PlaySound idleSound;
    [SerializeField] private SoundPlayOneshot alertedSound;
    [SerializeField] private SoundPlayOneshot attackSound;

    private NavMeshAgent navAgent;
    private bool attacking;
    private bool prepareAttack;
    private BehaviorTree tree;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        tree = GetComponent<BehaviorTree>();
    }

    private void Update()
    {
        if (navAgent == null || tree == null)
            return;

        var combatState = (SharedBool)tree.GetVariable("SeenPlayer");

        //If we're moving change anim to walk, if not idle
        if (navAgent.remainingDistance > minDestinationDistance && combatState.Value == false ||
            navAgent.remainingDistance > minAttackingDistance && combatState.Value)
        {
            // If move is on we know this is the 1st time in here so turn on/off relevant bools
            if(animController.GetBool("Move") == false)
            {
                animController.SetBool("Move", true);
                animController.SetBool("Attacking", false);
            }

            animController.SetFloat("MoveSpeed", navAgent.velocity.sqrMagnitude / navAgent.desiredVelocity.sqrMagnitude);
        }
        else
        {
            if(animController.GetBool("Move"))
                animController.SetBool("Move", false);

            if (prepareAttack)
            {
                animController.SetBool("Attacking", true);
                prepareAttack = false;
            }

            if (attacking)
            {
                if (attackSound)
                    attackSound.Play();

                animController.SetTrigger("SwingSword");
                attacking = false;
            }
        }

        if (animController && idleSound)
        {
            if (animController.GetBool("Attacking") == false && animController.GetBool("Move") == false && idleSound.Playing == false)
                idleSound.Play();
            else if ((animController.GetBool("Attacking") || animController.GetBool("Move")) && idleSound.Playing)
                idleSound.Stop();
        }
    }

    public void SawPlayer()
    {
        Alerted();

        for (int i = 0; i < transform.parent.childCount; i++)
        {
            //If we got ourselves just continue
            if (transform.parent.GetChild(i) == transform)
                continue;

            AnimController t = transform.parent.GetChild(i).GetComponent<AnimController>();

            if (t == null)
                continue;

            t.Alerted();
        }
    }

    public void Alerted()
    {
        if (alertedSound)
            alertedSound.Play();

        var value = (SharedBool)GetComponent<BehaviorTree>().GetVariable("SeenPlayer");
        value.Value = true;
    }

    public void PrepareAttack()
    {
        prepareAttack = true;
    }

    public void StartAttack()
    {
        attacking = true;
    }

    private void StopAttack()
    {
        attacking = false;
    }

    //if we end up with root anims

    //private void OnAnimatorMove()
    //{
    //    if (moveState == AnimStates.Run || moveState == AnimStates.Walk)
    //    {
    //        //set velocity to the clip velocity
    //        navAgent.velocity = animController.deltaPosition / Time.deltaTime;

    //        //smoothly rotate the character in the desired direction of motion
    //        Quaternion lookRotation = Quaternion.LookRotation(navAgent.desiredVelocity);
    //        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, navAgent.angularSpeed * Time.deltaTime);
    //    }
    //}


}

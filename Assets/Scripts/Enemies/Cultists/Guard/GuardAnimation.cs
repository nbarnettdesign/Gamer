using UnityEngine;

public class GuardAnimation : MonoBehaviour
{
    private CultistGuard guard;
    private Animator animator;

    private void Start()
    {
        guard = GetComponentInParent<CultistGuard>();
        animator = GetComponentInChildren<Animator>();
        Idle();
    }

    public void Idle()
    {
        //Debug.Log("Idle");

        if (animator.GetBool("Idle") == false)
            animator.SetBool("Idle", true);

        if (animator.GetBool("Walk"))
            animator.SetBool("Walk", false);

        if (animator.GetBool("Turn"))
            animator.SetBool("Turn", false);

        if (animator.GetBool("Run"))
            animator.SetBool("Run", false);

        if (animator.GetBool("SwordIdle"))
            animator.SetBool("SwordIdle", false);
    }

    public void Unsheathe()
    {
        //Debug.Log("Unsheathe");
        animator.SetBool("Idle", false);
        animator.SetTrigger("Unsheathe");
    }

    public void SwordUnsheathed()
    {
        guard.UnsheathedSword();
    }

    public void Sheathe()
    {
       // Debug.Log("Sheathe");
        //animator.SetBool("Idle", true);
        animator.SetTrigger("Sheathe");

        if (animator.GetBool("Walk"))
            animator.SetBool("Walk", false);

        if (animator.GetBool("Turn"))
            animator.SetBool("Turn", false);

        if (animator.GetBool("Run"))
            animator.SetBool("Run", false);

        if (animator.GetBool("SwordIdle"))
            animator.SetBool("SwordIdle", false);
    }

    public void SwordSheathed()
    {
        //Debug.Log("SwordSheathed");
        guard.SeathedSword();
    }

    public void SwordDrawnIdle()
    {
        //Debug.Log("SwordDrawnIdle");
        if (animator.GetBool("Walk"))
            animator.SetBool("Walk", false);

        if (animator.GetBool("Turn"))
            animator.SetBool("Turn", false);

        if (animator.GetBool("Run"))
            animator.SetBool("Run", false);

        if (animator.GetBool("SwordIdle") == false)
            animator.SetBool("SwordIdle", true);
    }

    public void Turn()
    {
       // Debug.Log("Turn");
        if (animator.GetBool("Walk"))
            animator.SetBool("Walk", false);

        if (animator.GetBool("Turn") == false)
            animator.SetBool("Turn", true);

        if (animator.GetBool("Run"))
            animator.SetBool("Run", false);

        if (animator.GetBool("SwordIdle"))
            animator.SetBool("SwordIdle", false);
    }

    public void Walk()
    {
        //Debug.Log("Walk");

        if (animator.GetBool("Walk") == false)
            animator.SetBool("Walk", true);

        if (animator.GetBool("Turn"))
            animator.SetBool("Turn", false);

        if (animator.GetBool("Run"))
            animator.SetBool("Run", false);

        if (animator.GetBool("SwordIdle"))
            animator.SetBool("SwordIdle", false);
    }

    public void Run()
    {
        if (animator.GetBool("Walk"))
            animator.SetBool("Walk", false);

        if (animator.GetBool("Turn"))
            animator.SetBool("Turn", false);

        if (animator.GetBool("Run") == false)
            animator.SetBool("Run", true);

        if (animator.GetBool("SwordIdle"))
            animator.SetBool("SwordIdle", false);
    }

    public void Attack()
    {
        animator.SetTrigger("SwingSword");
    }
}

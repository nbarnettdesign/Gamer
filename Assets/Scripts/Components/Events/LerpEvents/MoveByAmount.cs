using UnityEngine;
using UnityEngine.Events;

public class MoveByAmount : MonoBehaviour
{
    [SerializeField] private Vector3 movementAmount;
    [SerializeField] private float moveTime;
    [SerializeField] private AnimationCurve moveCurve;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip startedMoveSound;
    [SerializeField] private AudioClip finishedMoveSound;
    [SerializeField] private AudioClip returnFinishedSound;

    public float MoveTime { get { return moveTime; } }
    public bool IsMoving { get { return isMoving; } }

    private bool isMoving;

    public void BeginMove(float returnTime = 0)
    {
        if (isMoving)
            return;

        BeginMove(moveTime, returnTime);
    }

    public void BeginMove(float time, float returnTime)
    {
        if (isMoving)
            return;

        if(startedMoveSound)
            AudioSource.PlayClipAtPoint(startedMoveSound, transform.position);

        LerpController.Instance.LerpPositionByAmount(transform, movementAmount, time, moveCurve);
        isMoving = true;
        this.Invoke(StoppedMove, moveTime);

        if (returnTime > 0)
            this.Invoke(Return, moveTime + returnTime);
    }

    public void Return()
    {
        LerpController.Instance.LerpPositionByAmount(transform, -movementAmount, moveTime, moveCurve);
        isMoving = true;
        this.Invoke(StoppedMove, moveTime);
    }

    private void StoppedMove()
    {
        isMoving = false;

        if(finishedMoveSound && transform.position == transform.position + movementAmount)
            AudioSource.PlayClipAtPoint(finishedMoveSound, transform.position);
        else if(returnFinishedSound)
            AudioSource.PlayClipAtPoint(returnFinishedSound, transform.position);
    }
}

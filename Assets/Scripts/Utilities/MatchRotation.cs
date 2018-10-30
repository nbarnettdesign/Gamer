using UnityEngine;

public class MatchRotation : MonoBehaviour
{
    [SerializeField] private Transform transformToMatch;

    [Header("Match Axis")]
    [SerializeField]
    private bool x;
    [SerializeField] private bool y;
    [SerializeField] private bool z;
    [SerializeField] private bool forceRotationOnStart;

    private void Start()
    {
#if UNITY_EDITOR
        if (x == false && y == false && z == false)
            Debug.LogError(string.Format("{0} is flagged to match a rotation but does not have an axis flagged", name));

        if (transformToMatch == null)
            Debug.LogError(string.Format("{0} is flagged to match a rotation but has not transform to match", name));
#endif

        if (forceRotationOnStart && transformToMatch)
            transform.localRotation = transformToMatch.localRotation;

    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        Vector3 otherRotation = transformToMatch.localEulerAngles;
        Vector3 rotateTo = new Vector3();

        if (x == true && otherRotation.x != transform.localEulerAngles.x)
            rotateTo.x = otherRotation.x;

        if (y == true && otherRotation.y != transform.localEulerAngles.y)
            rotateTo.y = otherRotation.y;

        if (z == true && otherRotation.z != transform.localEulerAngles.z)
            rotateTo.z = otherRotation.z;


        if (x == true && rotateTo.x != 0 || y == true && rotateTo.y != 0 || z == true && rotateTo.z != 0)
            transform.localRotation = Quaternion.Euler(rotateTo);
    }
}

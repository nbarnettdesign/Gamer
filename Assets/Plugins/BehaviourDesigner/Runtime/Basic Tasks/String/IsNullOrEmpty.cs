using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityString
{
    [TaskCategory("Basic/String")]
    [TaskDescription("Returns success if the string is null or empty")]
    public class IsNullOrEmpty : Conditional
    {
        [Tooltip("The target string")]
        public SharedGameObject targetString;
        public SharedGameObject nothingComparison;
        public SharedString targetName;
        public bool started;

        
        public override TaskStatus OnUpdate()
        {
            if (!started)
            {
                started = true;
                nothingComparison = targetString;
            }
            targetName = targetString.Name;
            Debug.Log(targetName);
            if (targetString != nothingComparison)
            {
                Debug.Log("AXJGHLJAS");
            }
            return targetString == null ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            targetString = null;
        }
    }
}
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatrollingEntity)), CanEditMultipleObjects]
public class PatrolingEntityEditor : Editor {
	protected virtual void OnSceneGUI() {
		PatrollingEntity patrollingEntity = (PatrollingEntity)target;

		if (patrollingEntity.manualPatrolPoints) {
			for (int i = 0; i < patrollingEntity.patrolPoints.Count; i++) {
				EditorGUI.BeginChangeCheck();
				Vector3 patrolPoint = Handles.PositionHandle(patrollingEntity.patrolPoints[i], Quaternion.identity);

				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(patrollingEntity, string.Format("Patrol Point {0} adjusted", patrollingEntity.patrolPoints[i]));
					patrollingEntity.patrolPoints[i] = patrolPoint;
				}
			}
		}
	}
}

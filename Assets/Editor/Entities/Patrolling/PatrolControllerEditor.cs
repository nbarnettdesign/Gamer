using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatrolController)), CanEditMultipleObjects]
public class PatrolControllerEditor : Editor {
	protected virtual void OnSceneGUI() {
		PatrolController patrolController = (PatrolController)target;

		for (int i = 0; i < patrolController.patrolPoints.Count; i++) {
			EditorGUI.BeginChangeCheck();
			Vector3 patrolPoint = Handles.PositionHandle(patrolController.patrolPoints[i], Quaternion.identity);

			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(patrolController, string.Format("Patrol Point {0} adjusted", patrolController.patrolPoints[i]));
				patrolController.patrolPoints[i] = patrolPoint;
			}
		}
	}
}

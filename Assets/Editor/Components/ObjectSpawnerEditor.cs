using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectSpawner)), CanEditMultipleObjects]
public class ObjectSpawnerEditor : Editor {
	protected virtual void OnSceneGUI() {
		ObjectSpawner objectSpawner = (ObjectSpawner)target;

		for (int i = 0; i < objectSpawner.spawnPoints.Count; i++) {
			EditorGUI.BeginChangeCheck();
			Vector3 spawnPoint = Handles.PositionHandle(objectSpawner.spawnPoints[i], Quaternion.identity);

			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(objectSpawner, string.Format("{0} spawner position adjusted", objectSpawner.name));
				objectSpawner.spawnPoints[i] = spawnPoint;
			}
		}
	}
}

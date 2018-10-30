using System.Collections.Generic;
using UnityEngine;

public class EntitySpawnController {
    private List<Vector3> teleportPoints;
    private GameObject player;

	public Transform Player { get { return player.transform; } }

    public EntitySpawnController (string playerTag, string teleportTag) {
        player = GameObject.FindWithTag(playerTag);
        teleportPoints = new List<Vector3>();

        foreach (GameObject teleportPoint in GameObject.FindGameObjectsWithTag(teleportTag)) {
            teleportPoints.Add(teleportPoint.transform.position);
        }
    }

	public Vector3 GetClosestTeleportPoint(Vector3 point) {
		Vector3 p = Vector3.zero;
		float distSqr = Mathf.Infinity;

		for (int i = 0; i < teleportPoints.Count; i++) {
			float d = (teleportPoints[i] - point).sqrMagnitude;

			if (d < distSqr) {
				p = teleportPoints[i];
				distSqr = d;
			}
		}

		return p;
	}

    public Vector3 GetClosestTeleportPointToPlayer () {
        Vector3 p = Vector3.zero;
        float distSqr = Mathf.Infinity;

        for (int i = 0; i < teleportPoints.Count; i++) {
            float d = (teleportPoints[i] - player.transform.position).sqrMagnitude;

            if(d < distSqr) {
                p = teleportPoints[i];
                distSqr = d;
            }
        }

        return p;
    }
}

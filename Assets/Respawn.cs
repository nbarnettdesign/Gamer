using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {
    [SerializeField] Transform[] spawnPoints;
    private int spawnIndex;

    private void Start()
    {
       if(PlayerPrefs.HasKey("spawnPoint") == false)
            PlayerPrefs.SetInt("spawnPoint", 0);
    }

    public void SetSpawnPoint( int spawnIndex)
    {
       this.spawnIndex = spawnIndex;
        PlayerPrefs.SetInt("spawnPoint", this.spawnIndex);
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPoints[spawnIndex].position;
    }

    public void Reset()
    {
        spawnIndex = 0;
        spawnIndex = 0;
        PlayerPrefs.SetInt("spawnPoint", 0);
    }
}

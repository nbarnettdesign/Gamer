using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadyLoader : MonoBehaviour {
    [SerializeField] private float loadDelay;
    [SerializeField] private string levelName;

    public void LoadWithDelay()
    {
        this.Invoke(LevelLoad, loadDelay);
    }

    public void Load()
    {
        LevelLoad();
    }

    private void LevelLoad()
    {
        GameController.Instance.LoadLevel(levelName);
    }
}

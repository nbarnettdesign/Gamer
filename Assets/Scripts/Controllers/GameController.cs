using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class GameController : Singleton<GameController>
{
    [SerializeField] private Player player;

    [Header("Level Loading")]
    [SerializeField] private string deadLevel;
    [SerializeField] private float levelLoadDelay;

    public string SceneName { get { return SceneManager.GetActiveScene().name; } }
    public Player Player { get { return player; } }

    private void Awake()
    {
        if (player == null)
            player = FindObjectOfType<Player>();
    }

    public void EndGame()
    {
        SteamVR_LoadLevel.Begin(deadLevel, true, 0.5f, 1, 1, 1, 1);
    }

    public void LoadLevel(string levelName)
    {
        StartCoroutine(BeginLoadLevel(levelName));
    }

    public void ReloadLevel()
    {
        SteamVR_LoadLevel.Begin(SceneManager.GetActiveScene().name, true, 0.5f, 0, 0, 0, 0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private IEnumerator BeginLoadLevel(string levelName)
    {
        yield return new WaitForSeconds(levelLoadDelay);

        SteamVR_LoadLevel.Begin(levelName, true, 0.5f, 0, 0, 0, 0);
    }
}

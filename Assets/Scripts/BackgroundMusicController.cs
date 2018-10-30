using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LevelStartMusic
{
    public string levelName;
    public AudioSource audioSourceToStart;
}

public class BackgroundMusicController : Singleton<BackgroundMusicController>
{
    private AudioSource aSource;

    [SerializeField] private List<LevelStartMusic> levelStartingMusic;
    [SerializeField] private AudioSource ambientMusic;
    [SerializeField] private AudioSource actionMusic;
    [SerializeField] private AudioSource voidMusic;

    private void Start()
    {
        if (FindObjectsOfType<BackgroundMusicController>().Length <= 1)
            DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
    }

    private void OnLevelWasLoaded()
    {
        // Will have to do this better in the future
        for (int i = 0; i < levelStartingMusic.Count; i++)
        {
            if (GameController.Instance.SceneName == levelStartingMusic[i].levelName)
            {
                FadeInAmbient();
                TurnOffCombat();
                TurnOffVoid();
            }
        }
    }

    public void FadeInAmbient()
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn(ambientMusic, 4.5f));

    }

    public void FadeOutAmbient()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut(ambientMusic, 4f));

    }

    public void FadeInCombat()
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn(actionMusic, 1f));

    }

    public void FadeOutCombat()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut(actionMusic, 2f));

    }

    public void FadeInVoid()
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn(voidMusic, 8f));

    }

    public void FadeOutVoid()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut(voidMusic, 6f));

    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = 0.2f;

        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 1.0f)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.volume = 1f;
    }

    private void TurnOffVoid()
    {
        voidMusic.Stop();
    }

    private void TurnOffCombat()
    {
        actionMusic.Stop();
    }

    private void TurnOnAmbient()
    {
        ambientMusic.Play();
    }
}

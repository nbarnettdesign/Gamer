using UnityEngine;

public class Headfade : MonoBehaviour
{
    [SerializeField] private GameObject playArea;
    private GameObject head;
    private bool arghMyHead;
    private bool imBlind;
    [SerializeField] private LayerMask environmentMask;
    [SerializeField] private float fadeInTime = 1.2f;
    [SerializeField] private float fadeOutTime = 0.5f;

    [SerializeField] private Color fadeColour = Color.cyan;

    private void Start()
    {
        head = gameObject;
        arghMyHead = false;
        imBlind = false;
    }

    private void Update()
    {
        Blindness();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 10)
        {
            FadeToWhite();
            arghMyHead = true;
            imBlind = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            arghMyHead = false;
        }
    }

    private void Blindness()
    {
        if (!arghMyHead && imBlind)
            if (!Physics.Linecast(head.transform.position, playArea.transform.position, environmentMask))
            {
                Invoke("FadeFromWhite", fadeInTime);
                imBlind = false;
            }
    }

    private void FadeToWhite()
    {
        SteamVR_Fade.Start(Color.clear, 0f);
        SteamVR_Fade.Start(fadeColour, fadeInTime);
    }

    private void FadeFromWhite()
    {
        SteamVR_Fade.Start(fadeColour, 0f);
        SteamVR_Fade.Start(Color.clear, fadeOutTime);
    }

}

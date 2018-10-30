using UnityEngine;
using Valve.VR.InteractionSystem;

public class CrushingWall : MonoBehaviourExtended
{
    private SphereCollider triggerVolume;

    [SerializeField] private bool autoStart = false;
    [SerializeField] private int touchDamage = 1;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float triggerRadius = 2.0f;
    [SerializeField] private PlaySound loopingSlideSound;
    [SerializeField] private string stopTag;
    [SerializeField] private SoundPlayOneshot stopSound;

    private bool isActive = false;

    protected override void Start()
    {
        base.Start();

        triggerVolume = GetComponentInChildren<SphereCollider>();
        triggerVolume.radius = triggerRadius;

        if (autoStart != false) isActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (string.IsNullOrEmpty(other.tag))
            return;

        if (other.CompareTag("Player") && isActive)
        {
            Affectable a = other.GetComponent<Affectable>();

            if (a == null)
                other.GetComponentInChildren<Affectable>();

            if(a != null)
                a.Damage(touchDamage, transform.position, transform.forward, false, true);
        } else if(other.CompareTag(stopTag) && isActive)
        {
            isActive = false;
        }
    }

    void Update()
    {
        if (IsRendering() == false)
            return;

        MoveTrap();
    }

    void MoveTrap()
    {
        if (isActive == false)
            return;

        transform.Translate(0, 0, -moveSpeed * Time.deltaTime);
    }

    public void TurnOnWall()
    {
        isActive = true;
        loopingSlideSound.Play();
        triggerVolume.gameObject.SetActive(false);
    }

    public void TurnOffWall()
    {
        isActive = false;
        stopSound.Play();
    }
}

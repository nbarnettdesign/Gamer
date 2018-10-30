using UnityEngine;

public class World_HUDController : MonoBehaviourExtended
{
    [Header("Cache")]
    [SerializeField] GameObject HUDGameObject;

    Animator uiAnimator;
    Camera cam;

    [Header("Time Settings")]
    [SerializeField] float activateLookTime = 0.5f;
    //[SerializeField]
    //float deactivateLookTime = .8f;

    float activateTimer;
    float deactiveTimer;

    [SerializeField]
    bool isActive = false;

    float raycastDist = 25f;

    [SerializeField]
    LayerMask headRaycastLayer;

    RaycastHit hit;

    protected override void Start()
    {
        base.Start();
        //HUDGameObject = GetComponent<GameObject>();
        cam = Camera.main;
        uiAnimator = HUDGameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (IsRendering() == false)
            return;

        activateTimer += Time.deltaTime;
        deactiveTimer += Time.deltaTime;

        Physics.Raycast(cam.transform.position, transform.forward, out hit, raycastDist, headRaycastLayer);

        if (activateTimer > activateLookTime)
        {


            if (isActive == false && hit.collider)
            {
                activateTimer = 0;
                TurnOnUI();
            }
        }

        /*if (deactiveTimer > deactivateLookTime)
        {
            if (isActive == true && !hit.collider)
            {
                deactiveTimer = 0;
                TurnOffUI();
            }
        }*/
    }

    void TurnOnUI()
    {
        isActive = true;
        HUDGameObject.SetActive(true);
        uiAnimator.SetBool("uiActive", true);
    }

    void TurnOffUI()
    {
        isActive = false;
        HUDGameObject.SetActive(false);
        uiAnimator.SetBool("uiActive", false);
    }
}

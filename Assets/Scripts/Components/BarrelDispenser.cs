using UnityEngine;

public class BarrelDispenser : MonoBehaviour
{
    private Animator trapAnim;
    [SerializeField] private Transform spawnPoint;

    private float cooldown = 0;
    [SerializeField] private GameObject barrelToSpawn;
    [SerializeField] private AudioClip dispenseSound;
    [SerializeField] private float doorInterval = 3f;


    void Start()
    {
        trapAnim = GetComponent<Animator>();
    }

    public void StartDoor()
    {
        if (cooldown > 0)
            return;

        cooldown = doorInterval;

        trapAnim.SetTrigger("start");
        this.Invoke(SpawnBarrel, 2f);
    }

    private void SpawnBarrel()
    {
        if (dispenseSound)
            AudioSource.PlayClipAtPoint(dispenseSound, transform.position);

        GameObject barrel = Instantiate(barrelToSpawn, spawnPoint);
        barrel.transform.SetParent(null);
        barrel.GetComponent<Rigidbody>().AddForce(spawnPoint.forward * 5, ForceMode.VelocityChange);
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
    }
}

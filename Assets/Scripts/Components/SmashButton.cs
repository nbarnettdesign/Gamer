using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

public class SmashButton : Hoverable {
    [SerializeField] private float handSpeed;
    [SerializeField] private UnityEvent ButtonSmashEvent;



    private void OnTriggerEnter(Collider other)
    {
        //check players hand velocity

        if (other.GetComponent<Hand>()){
            //is hand
            if (other.GetComponent<VelocityEstimator>())
            {
                if (other.GetComponent<VelocityEstimator>().GetVelocityEstimate().sqrMagnitude >= handSpeed)
                    //Play button animation

                    ButtonSmashEvent.Invoke();

                    //Start Cooldown

            }
        }
    }
}

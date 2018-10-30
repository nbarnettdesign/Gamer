using UnityEngine;

public class Teleported : MonoBehaviour
{
    public void ExistsFor(float existsTime)
    {
        this.Invoke(Remove, existsTime);
    }

    private void Remove()
    {
        Destroy(this);
    }
}

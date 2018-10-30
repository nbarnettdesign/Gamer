using UnityEngine;

public class MonoBehaviourExtended : MonoBehaviour {
    protected Renderer myRenderer;

	protected virtual void Start () {
        myRenderer = GetComponent<Renderer>();

        if(myRenderer == null && transform.parent)
        {
            // This is for situations where an object has no renderer
            // fireSource as an example but has one in the parent
            myRenderer = transform.parent.GetComponent<Renderer>();
        }
    }

    /// <summary>
    /// Checks to see if the objects renderer is running, retuns true if no renderer has been found
    /// </summary>
    /// <returns></returns>
    protected bool IsRendering()
    {
        if (myRenderer && myRenderer.isVisible == false)
            return false;

        return true;
    }
}

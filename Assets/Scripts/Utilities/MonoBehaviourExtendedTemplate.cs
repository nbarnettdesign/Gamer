
public class MonoBehaviourExtendedTemplate : MonoBehaviourExtended {

	protected override void Start () {
        base.Start();
	}
	
	void Update () {
        if (IsRendering() == false)
            return;
	}
}

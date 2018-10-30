using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class Entity : MonoBehaviour {
	[SerializeField] private CurrentEntityType currentType;
	public CurrentEntityType CurrentType { get { return currentType; } }

    protected virtual void Start () {
    }
}

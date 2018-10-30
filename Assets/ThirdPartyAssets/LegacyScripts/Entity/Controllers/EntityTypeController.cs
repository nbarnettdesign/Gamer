using System.Collections.Generic;
using UnityEngine;

public class EntityTypeController : MonoBehaviour {
    [SerializeField] private List<EntityTypeInfo> entityTypeInfo;
    
    public EntityTypeInfo GetEntityTypeInfo (CurrentEntityType type) {
        return entityTypeInfo.Find(e => e.Type == type);
    }

    private void OnValidate () {
        int entityTypeCount = System.Enum.GetValues(typeof(CurrentEntityType)).Length;

        if (entityTypeInfo == null)
            entityTypeInfo = new List<EntityTypeInfo>();

        if (entityTypeInfo.Count < entityTypeCount) {
            for (int i = 0; i < entityTypeCount; i++) {
                if(entityTypeInfo.Exists(e => e.Type == (CurrentEntityType)i) == false) {
                    EntityTypeInfo et = new EntityTypeInfo();
                    et.SetEntityType((CurrentEntityType) i);

                    entityTypeInfo.Add(et);
                }
            }
        } 
    }
}

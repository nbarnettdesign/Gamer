using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ImpactSetting {
    public ObjectWeight weight;
    public float stunTime;
    public float damage;
}

[CreateAssetMenu(fileName = "New Impact Settings", menuName = "Affectables/Impact Settings")]
public class ImpactSettings : ScriptableObject {
    [SerializeField] private List<ImpactSetting> impactSettings;

    public ImpactSetting Find(ObjectWeight weight) {
        return impactSettings.Find(i => i.weight == weight);
    }

    private void OnValidate() {
        if (impactSettings == null || impactSettings.Count != System.Enum.GetValues(typeof(ObjectWeight)).Length) {
            if (impactSettings == null)
                impactSettings = new List<ImpactSetting>();

            for (int i = 0; i < System.Enum.GetValues(typeof(ObjectWeight)).Length; i++) {
                if (impactSettings.Exists(w => w.weight == (ObjectWeight)i) == false) {
                    ImpactSetting impact = new ImpactSetting {
                        weight = (ObjectWeight)i
                    };

                    impactSettings.Add(impact);
                }
            }
        }
    }
}

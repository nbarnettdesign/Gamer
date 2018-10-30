using UnityEngine;

[System.Serializable]
public struct EntityTypeInfo {
    [SerializeField] private CurrentEntityType type;
    [SerializeField] private Sprite entityTypeSprite;
    [SerializeField] private Material typeMaterial;
    [SerializeField] private Texture typeTexture;

    public void SetEntityType (CurrentEntityType type) {
        this.type = type;
    }

    public CurrentEntityType Type { get { return type; } }
    public Sprite EntityTypeSprite { get { return entityTypeSprite; } }
    public Material TypeMaterial { get { return typeMaterial; } }
    public Texture TypeTexture { get { return typeTexture; } }
}

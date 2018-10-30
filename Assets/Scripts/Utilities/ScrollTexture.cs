using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    [SerializeField] private Vector2 scrollSpeed;

    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        Scroll();
    }

    private void Scroll()
    {
        if (rend == null)
            return;

        Vector2 currentOffset = rend.material.GetTextureOffset("_MainTex");
        rend.material.SetTextureOffset("_MainTex", currentOffset + (scrollSpeed * Time.deltaTime));
    }
}

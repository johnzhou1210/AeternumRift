using UnityEngine;
using KBCore.Refs;

public class ScrollingGradient : MonoBehaviour {
    [SerializeField, Self] private Material gradientMaterial;
    [SerializeField] private float scrollSpeed = 0.1f;

    private float offset = 0f;

    void Update() {
        // Increment the offset based on scroll speed
        offset += scrollSpeed * Time.deltaTime;
        offset = offset % 1f; // Wrap the offset to keep it within [0, 1]

        // Update the shader's texture offset
        gradientMaterial.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}

using UnityEngine;
using System.Collections;
using KBCore.Refs;
using Gilzoide.GradientRect;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class AnimateGradient : MonoBehaviour {
    [SerializeField, Self] private GradientTexture gradientTexture;
    [SerializeField] private float gradientUpdateInterval = 0.1f; // Control speed of scrolling
    [SerializeField] private float gradientStepDistance = .05f;

    private Coroutine gradientCoroutine;

    private void OnEnable() {
        gradientCoroutine = StartCoroutine(GradientCoroutine());
    }

    public void OnDisable() {
        if (gradientCoroutine != null) {
            StopCoroutine(gradientCoroutine);
        }
    }

    IEnumerator GradientCoroutine() {
        while (true) {
            // Get current color keys from the gradient
            GradientColorKey[] colorKeys = gradientTexture.Gradient.colorKeys;

            // Increment the time values for the center, left, and right keys
            IncrementColorKey(ref colorKeys[1], gradientStepDistance); // Center key
            IncrementColorKey(ref colorKeys[2], gradientStepDistance); // Left key
            IncrementColorKey(ref colorKeys[3], gradientStepDistance); // Right key

            // Update the gradient with the new color keys
            gradientTexture.Gradient.colorKeys = colorKeys;

            // Refresh the gradient texture
            gradientTexture.SetVerticesDirty();
            gradientTexture.SetMaterialDirty();

            yield return new WaitForSeconds(gradientUpdateInterval);
        }
    }

    private void IncrementColorKey(ref GradientColorKey key, float increment) {
        key.time = (key.time + increment) % 1f;
    }

    private void OnValidate() {
        this.ValidateRefs();
    }
}

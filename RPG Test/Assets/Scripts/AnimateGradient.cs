using UnityEngine;
using System.Collections;
using KBCore.Refs;
using Gilzoide.GradientRect;

public class AnimateGradient : MonoBehaviour
{
    [SerializeField, Self] private GradientTexture gradientTexture;
    [SerializeField] private float gradientSpeed = 0.1f; // Control speed of scrolling

    // Update is called once per frame
    void Update()
    {
        // Calculate progress based on time and speed
        float progress = (Time.time * gradientSpeed) % 1f;

        // Smoothly update the gradient keys
        MoveGradientKeys(Time.deltaTime * gradientSpeed);

        // Mark the gradient texture as dirty to update it
        gradientTexture.SetVerticesDirty();
        gradientTexture.SetMaterialDirty();
    }


    private void MoveGradientKeys(float offset)
    {
        // Get the existing color and alpha keys
        GradientColorKey[] colorKeys = gradientTexture.Gradient.colorKeys;
        GradientAlphaKey[] alphaKeys = gradientTexture.Gradient.alphaKeys;

        // Move all color keys by the specified offset while preserving order
        for (int i = 0; i < colorKeys.Length; i++)
        {
            colorKeys[i].time = (colorKeys[i].time + offset) % 1f;
            Debug.Log(colorKeys[i].time);
        }

        // Move all alpha keys by the specified offset while preserving order
        for (int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i].time = Mathf.Clamp01(alphaKeys[i].time + offset);
        }

        // Sort the color keys by their time to maintain the correct order
        System.Array.Sort(colorKeys, (a, b) => a.time.CompareTo(b.time));
        System.Array.Sort(alphaKeys, (a, b) => a.time.CompareTo(b.time));

        // Set the modified keys back to the gradient
        gradientTexture.Gradient.SetKeys(colorKeys, alphaKeys);
    }

    private void OnValidate()
    {
        this.ValidateRefs();
    }
}

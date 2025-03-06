using System;
using KBCore.Refs;
using TMPro;
using UnityEngine;

public class DisplayFPS : MonoBehaviour {
    [SerializeField, Self] private TextMeshProUGUI fpsText;
    private float deltaTime = 0f;

    private void Awake() {
        Application.targetFrameRate = (int)Mathf.Round((float)Screen.currentResolution.refreshRateRatio.value);
    }

    private void OnValidate() {
        this.ValidateRefs();
    }

    private void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = $"{fps:0.} FPS";
    }
}

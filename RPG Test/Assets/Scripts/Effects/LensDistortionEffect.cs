using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LensDistortionEffect : MonoBehaviour
{
    public static LensDistortionEffect Instance;
    
    public Volume volume;
    private LensDistortion lens;
    private DepthOfField depthOfField;
    private ColorAdjustments colorAdjustments;

    private Coroutine _activeDistortionEffect;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        volume.profile.TryGet(out lens);
        volume.profile.TryGet(out depthOfField);
        volume.profile.TryGet(out colorAdjustments);
    }

    public void DoEncounterEffect() {
        _activeDistortionEffect = StartCoroutine(Warp());
    }

    private void OnDestroy() {
        if (_activeDistortionEffect != null) {
            StopCoroutine(_activeDistortionEffect);
            _activeDistortionEffect = null;
        }
    }


    IEnumerator Warp()
    {
        float duration = 2f;
        float t = 0f;

        // Warp in
        while (t < duration)
        {
            float normalized = t / duration;
            lens.intensity.value = Mathf.Lerp(0f, -1f, normalized);
            lens.scale.value = Mathf.Lerp(1f, 0f, normalized);
            depthOfField.focusDistance.value = Mathf.Lerp(5f, 0f, normalized * 2f);
            colorAdjustments.postExposure.value = Mathf.Lerp(0, -10f, normalized * 2f);
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f); // Optional pause at peak

        // Warp out (reverse)
        t = 0f;
        while (t < duration)
        {
            float normalized = t / duration;
            lens.intensity.value = Mathf.Lerp(-1f, 0f, normalized);
            lens.scale.value = Mathf.Lerp(0f, 1f, normalized);
            depthOfField.focusDistance.value = Mathf.Lerp(0f, 5f, normalized * 2f);
            colorAdjustments.postExposure.value = Mathf.Lerp(-10, 0f, normalized * 2f);
            t += Time.deltaTime;
            yield return null;
        }

        // Optional: Disable the effects after reset
        lens.active = false;
        depthOfField.active = false;
    }
}
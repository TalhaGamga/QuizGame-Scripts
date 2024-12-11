using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PostProcessingEffector : MonoBehaviour
{
    public Volume volume; // Assign your post-processing Volume in the Inspector

    private Vignette vignette;
    private Tonemapping tonemapping;

    private void Start()
    {
        // Ensure the Volume is valid and fetch overrides
        if (volume.profile.TryGet(out vignette))
        {
            Debug.Log("Vignette found!");
        }
        else
        {
            Debug.LogWarning("Vignette not found in Volume Profile!");
        }

        if (volume.profile.TryGet(out tonemapping))
        {
            Debug.Log("Tonemapping found!");
        }
        else
        {
            Debug.LogWarning("Tonemapping not found in Volume Profile!");
        }
    }

    // Smoothly blend vignette intensity
    private void blendVignette(float targetIntensity, float duration)
    {
        if (vignette != null)
        {
            DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, targetIntensity, duration)
                .SetEase(Ease.InOutQuad);
        }
    }

    // Smoothly change tonemapping mode (optional example)
    private void ChangeTonemapping(TonemappingMode targetMode)
    {
        if (tonemapping != null)
        {
            tonemapping.mode.Override(targetMode);
        }
    }

    // Example: Trigger a "hit effect" with a red vignette flash
    public void TriggerHitEffect()
    {
        if (vignette != null)
        {
            // Flash intensity to 0.8 and then back to 0.3
            DOTween.Sequence()
                .Append(DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0.5f, 0.1f))
                .Append(DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0f, 1.5f))
                .SetEase(Ease.OutQuad);
        }
    }
}

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightnessController : MonoBehaviour
{
    public Volume volume;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        if (volume.profile.TryGet(out colorAdjustments))
        {
            Debug.Log("Color Adjustments override found.");
        }
        else
        {
            Debug.LogWarning("Color Adjustments override not found on Volume.");
        }
    }

    public void SetBrightness(float value)
    {
        if (colorAdjustments != null)
        {
            // value typically in range [-5, 5]
            colorAdjustments.postExposure.value = value;
        }
    }

    public void SetContrast(float value)
    {
        if (colorAdjustments != null)
        {
            // value range: [-100, 100]
            colorAdjustments.contrast.value = value;
        }
    }

    public void SetSaturation(float value)
    {
        if (colorAdjustments != null)
        {
            // value range: [-100, 100]
            colorAdjustments.saturation.value = value;
        }
    }
}

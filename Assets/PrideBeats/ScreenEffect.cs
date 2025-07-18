using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenEffect : MonoBehaviour
{
    public char keybind;

    public float duration = 0.5f;
    public float magnitude = 1f;
    public AnimationCurve animationCurve;
    public Volume volume;

    private bool isActive = false;
    private LensDistortion lensDistortion;

    void Update()
    {
        if (!isActive)
        {
            try
            {
                KeyCode code = (KeyCode)System.Enum.Parse(typeof(KeyCode), keybind.ToString().ToUpper());
                if (Input.GetKeyDown(code))
                {
                    isActive = true;
                    Debug.Log("ACTIVATING SCREEN EFFECTS");
                    ActivateEffects(true);
                }
            }
            catch
            {
                Debug.LogWarning($"Invalid keybind: {keybind}");
            }
        }
    }

    public void ActivateEffects(bool fullEffect)
    {
        StartCoroutine(RotateShake());
        if (fullEffect) StartCoroutine(DistortionPulse());
    }

    IEnumerator RotateShake()
    {
        Quaternion originalRot = transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float strength = animationCurve != null ? animationCurve.Evaluate(elapsed / duration) : 1f;

            float rotX = Random.Range(-1f, 1f) * magnitude * strength;
            float rotY = Random.Range(-1f, 1f) * magnitude * strength;
            float rotZ = Random.Range(-1f, 1f) * magnitude * strength;

            transform.rotation = originalRot * Quaternion.Euler(rotX, rotY, rotZ);
            yield return null;
        }

        transform.rotation = originalRot;
        isActive = false;
    }

    IEnumerator DistortionPulse()
    {
        if (volume == null || !volume.profile.TryGet(out lensDistortion))
        {
            Debug.LogWarning("DistortionPulse: LensDistortion not found on Volume.");
            yield break;
        }

        float originalIntensity = lensDistortion.intensity.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float strength = animationCurve != null ? animationCurve.Evaluate(elapsed / duration) : 1f;

            // Apply distortion pulse
            lensDistortion.intensity.value = originalIntensity + (magnitude * strength) / 2f;

            yield return null;
        }

        // Restore original distortion intensity
        lensDistortion.intensity.value = originalIntensity;
        isActive = false;
    }
}

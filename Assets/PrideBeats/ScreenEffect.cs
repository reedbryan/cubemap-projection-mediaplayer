using UnityEngine;
using System.Collections;

public class ScreenEffect : MonoBehaviour
{
    public char keybind;
    
    public float duration = 0.5f;
    public float magnitude = 1f;
    public AnimationCurve animationCurve;

    private bool isActive = false;

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
                    Debug.Log("SHAKING");
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
        if (fullEffect) StartCoroutine(FOVPulse());
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

    IEnumerator FOVPulse()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogWarning("FOVPulse: No Camera component found on GameObject.");
            yield break;
        }

        float originalFOV = cam.fieldOfView;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float strength = animationCurve != null ? animationCurve.Evaluate(elapsed / duration) : 1f;

            // Increase FOV based on magnitude and curve
            float targetFOV = originalFOV + (magnitude * strength) * 10f;
            cam.fieldOfView = targetFOV;

            yield return null;
        }

        // Restore original FOV
        cam.fieldOfView = originalFOV;
        isActive = false;
    }


}

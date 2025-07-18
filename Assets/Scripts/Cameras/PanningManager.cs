using UnityEngine;
using UnityEngine.UI;

public class PanningManager : MonoBehaviour
{
    public Slider slider;
    public Toggle passivePanningToggle;

    public Transform cameraParent;
    private Vector3 initialRotation;

    public float passivePanningSpeed = 10f; // degrees per second

    void Start()
    {
        initialRotation = cameraParent.eulerAngles;

        // Optional: Set up the slider
        /*
        slider.minValue = 0f;
        slider.maxValue = 360f;
        slider.value = 0f;
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        */
    }

    void Update()
    {
        // Passive panning
        if (passivePanningToggle != null && passivePanningToggle.isOn)
        {
            RotateCamera(passivePanningSpeed * Time.deltaTime);
        }

        // Manual input
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RotateCamera(-1);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            RotateCamera(1);
        }
    }

    void OnSliderValueChanged(float value)
    {
        RotateCamera(value);
    }

    void RotateCamera(float angle)
    {
        float newY = cameraParent.eulerAngles.y + angle;
        cameraParent.rotation = Quaternion.Euler(initialRotation.x, newY, initialRotation.z);
    }
}
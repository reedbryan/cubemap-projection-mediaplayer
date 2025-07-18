using UnityEngine;
using UnityEngine.UI;

public class PanningManager : MonoBehaviour
{

    public Slider slider;

    public Transform cameraParent;   
    private Vector3 initialRoation; 

    void Start()
    {
        initialRoation = cameraParent.eulerAngles;
        /*    
        slider.minValue = 0f;
        slider.maxValue = 360f;
        slider.value = 0f;

        slider.onValueChanged.AddListener(OnSliderValueChanged);
        */
    }

    void Update()
    {
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
        float newRotation = cameraParent.eulerAngles.y + angle;
        cameraParent.rotation = Quaternion.Euler(initialRoation.x, newRotation, initialRoation.z);
    }
}
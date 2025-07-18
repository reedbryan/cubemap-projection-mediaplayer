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

/*write a readme for my unity project. the project takes 360 videos (dropped into the streaming assets folder) and outputs a cubemapped equirectangular projection of that footage into 3 displays. it is ment to be used in a space with 3 projectors, projecting onto evenly sized walls so that it looks like you are surrounded by the footage. there are features to edit the post processing of each display (brightness, saturation, contrast, etc) to account for any inconsistancy in the different projectors. also the tilt of each displays camera is editable (to fix any alignment issues at runntime). all these settings can be saved presets and reloaded accross multiple sessions. you can also rotate the cameras to get a different viewpoint or activate a rotate setting that will slowly pan the camera on its own. 

when writing make references to code (i've attached the repo below) and if something is better demonstrated with an image then leave a placeholder and I take add that later.

Github: https://github.com/reedbryan/cubemap-projection-mediaplayer 

*/
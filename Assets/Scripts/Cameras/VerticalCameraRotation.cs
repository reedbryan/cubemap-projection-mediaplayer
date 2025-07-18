using UnityEngine;
using UnityEngine.UI;

public class VerticalCameraRotation : MonoBehaviour
{
    public Button lookUpButton;
    public Button lookDownButton;
    public Vector3 axisOfRotation = new Vector3(1,0,0);

    public bool invertRotation = false;

    public float rotationSpeed; // Degrees per second

    void Start()
    {
        // Add event listeners for button press and release
        lookUpButton.onClick.AddListener(incrmentUp);
        lookDownButton.onClick.AddListener(incrmentDown);

        // Optional: Hook into button release if you're using EventTrigger or custom methods
    }

    public void incrmentUp()
    {
        transform.eulerAngles += axisOfRotation * rotationSpeed * (invertRotation ? -1 : 1);
        Debug.Log(gameObject.name + " Rotating upwards");
    }

    public void incrmentDown()
    {
        transform.eulerAngles -= axisOfRotation * rotationSpeed * (invertRotation ? -1 : 1);
        Debug.Log(gameObject.name + " Rotating downwards");
    }
}
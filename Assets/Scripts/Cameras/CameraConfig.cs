using UnityEngine;
using TMPro;

public class CameraConfig : MonoBehaviour
{
    public Camera[] cameras; // Make sure this is size-matched with available displays
    public TextMeshProUGUI displayLogText; // Or TextMeshPro depending on your setup

    void Start()
    {
        Debug.Log("Displays connected: " + Display.displays.Length);
        displayLogText.text += "Activated display: 1\n";

        // Fullscreen on the main display
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.fullScreen = true;

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();

            if (i < cameras.Length)
            {
                cameras[i].targetDisplay = i; // Use zero-based index
                Debug.Log("Activated display: " + (i+1));
                if (displayLogText != null)
                {
                    displayLogText.text += "Activated display: " + (i+1) + "\n";
                }
            }
            else
            {
                Debug.LogWarning("No camera assigned for display: " + i);
            }
        }
    }
}

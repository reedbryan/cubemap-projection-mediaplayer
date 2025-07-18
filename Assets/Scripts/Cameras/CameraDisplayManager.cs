using UnityEngine;
using TMPro;

public class CameraDisplayManager : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown dropdown1;
    public TMP_Dropdown dropdown2;
    public TMP_Dropdown dropdown3;

    [Header("Cameras")]
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;

    // Keep track of previous values to handle swaps
    [SerializeField]private int[] prevValues = new int[3];

    private void Start()
    {
        // Initialize dropdown values
        SetDropdownValue(0, 0);
        SetDropdownValue(1, 1);
        SetDropdownValue(2, 2);
       
        // Initialize previous values
        prevValues[0] = dropdown1.value;
        prevValues[1] = dropdown2.value;
        prevValues[2] = dropdown3.value;

        // Assign initial camera displays
        ApplyDisplay(0, prevValues[0]);
        ApplyDisplay(1, prevValues[1]);
        ApplyDisplay(2, prevValues[2]);

        // Add listeners
        dropdown1.onValueChanged.AddListener(idx => OnDropdownChanged(0, idx));
        dropdown2.onValueChanged.AddListener(idx => OnDropdownChanged(1, idx));
        dropdown3.onValueChanged.AddListener(idx => OnDropdownChanged(2, idx));
    }

    private void OnDropdownChanged(int camIndex, int newValue)
    {
        int oldValue = prevValues[camIndex];
        if (newValue == oldValue)
            return;

        // Apply new display to this camera
        ApplyDisplay(camIndex, newValue);

        // Check for conflicts with other cameras and swap if necessary
        for (int i = 0; i < 3; i++)
        {
            if (i == camIndex) continue;

            // If another camera has the same display
            if (GetDisplayValue(i) == newValue)
            {
                // Swap it back to the old value
                ApplyDisplay(i, oldValue);

                // Update the other dropdown without triggering its listener
                SetDropdownValue(i, oldValue);

                // Update its prevValues entry
                prevValues[i] = oldValue;

                break; // Only one conflict possible
            }
        }

        // Update prev value for this camera
        prevValues[camIndex] = newValue;
    }

    private void ApplyDisplay(int camIndex, int dropdownValue)
    {
        Camera cam = GetCamera(camIndex);
        if (cam == null) return;

        // Assuming dropdown options are named "Display 1", "Display 2", "Display 3",
        // and correspond to targetDisplay indices 0,1,2 respectively.
        cam.targetDisplay = dropdownValue;
    }

    private int GetDisplayValue(int camIndex)
    {
        Camera cam = GetCamera(camIndex);
        return cam.targetDisplay;
    }

    private Camera GetCamera(int index)
    {
        switch (index)
        {
            case 0: return cam1;
            case 1: return cam2;
            case 2: return cam3;
            default: return null;
        }
    }

    private void SetDropdownValue(int camIndex, int value)
    {
        Debug.Log("SetDropdownValue, cam: " + (camIndex+1) + " to display: " + value);
        
        // Temporarily remove listener to avoid recursion
        switch (camIndex)
        {
            case 0:
                dropdown1.onValueChanged.RemoveAllListeners();
                dropdown1.value = value;
                dropdown1.onValueChanged.AddListener(idx => OnDropdownChanged(0, idx));
                break;
            case 1:
                dropdown2.onValueChanged.RemoveAllListeners();
                dropdown2.value = value;
                dropdown2.onValueChanged.AddListener(idx => OnDropdownChanged(1, idx));
                break;
            case 2:
                dropdown3.onValueChanged.RemoveAllListeners();
                dropdown3.value = value;
                dropdown3.onValueChanged.AddListener(idx => OnDropdownChanged(2, idx));
                break;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class TogglePBUI : MonoBehaviour
{
    [Header("UI Setup")]
    public Button prideBeatsButton;
    public Button backButton;
    public GameObject PrideBeatsUI;
    public GameObject VideoPlayerUI;

    void Start()
    {
        // Start with VideoPlayerUI active
        PrideBeatsUI.SetActive(false);
        VideoPlayerUI.SetActive(true);

        // Hook up buttons
        backButton.onClick.AddListener(ToggleUI);
        prideBeatsButton.onClick.AddListener(ToggleUI);
    }

    void ToggleUI()
    {
        if (PrideBeatsUI.activeSelf)
        {
            PrideBeatsUI.SetActive(false);
            VideoPlayerUI.SetActive(true);
        }
        else if (VideoPlayerUI.activeSelf)
        {
            VideoPlayerUI.SetActive(false);
            PrideBeatsUI.SetActive(true);
        }

        Debug.Log("[TogglePBUI] Switched UI objects.");
    }
}

using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class ConfigVideoSource : MonoBehaviour
{
    public RenderTexture renderTexture;
    public Material skyboxMaterial;
    public Material blackSkyboxMaterial;
    public TMP_Dropdown videoDropdown;

    private VideoPlayer videoPlayer;
    private List<string> videoPaths = new();

    public GameObject canvas;

    void Start()
    {
        // Find all video files in StreamingAssets
        string folderPath = Application.streamingAssetsPath;
        string[] extensions = { ".mp4", ".mov", ".webm", ".avi", ".m4v" };

        videoPaths = Directory.GetFiles(folderPath)
            .Where(path => extensions.Contains(Path.GetExtension(path).ToLower()))
            .ToList();

        if (videoPaths.Count == 0)
        {
            Debug.LogError("No video files found in StreamingAssets.");
            return;
        }

        // Populate dropdown with filenames
        videoDropdown.ClearOptions();
        List<string> fileNames = new List<string> { "Black Skybox" }; // Add custom option
        fileNames.AddRange(videoPaths.Select(Path.GetFileName));      // Add video files
        videoDropdown.AddOptions(fileNames);

        videoDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Prepare video player
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.source = VideoSource.Url;

        // Set skybox
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = blackSkyboxMaterial; // start with black skybox
            DynamicGI.UpdateEnvironment();
        }
    }

    void OnDropdownValueChanged(int index)
    {
        if (index == 0)
        {
            // Black skybox
            RenderSettings.skybox = blackSkyboxMaterial;
            DynamicGI.UpdateEnvironment();
            RenderTexture.active = null;
            videoPlayer.Stop();
            return;
        }

        int videoIndex = index - 1; // Offset by 1 because of "Black Skybox"
        if (videoIndex >= 0 && videoIndex < videoPaths.Count)
        {
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
            PlayVideo(videoPaths[videoIndex]);
        }
    }


    void PlayVideo(string fullPath)
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        string url = "file://" + fullPath;
        #elif UNITY_ANDROID
        string url = "jar:file://" + fullPath;
        #else
        string url = fullPath;
        #endif

        videoPlayer.url = url;
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (vp) => vp.Play();
    }
}

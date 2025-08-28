using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
    public OSCMessaging oscMessaging;

    // Game States - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
    private enum GameState
    {
        SessionClosed,
        SessionOpen,
        Calibrating,
        GameStarting,
        GameOn
    }
    [Header("Current Game State")]
    [SerializeField] private GameState currentState = GameState.SessionClosed;
    // read-only public accessor
    private GameState CurrentState => currentState;
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    // UI - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
    [Header("Start Session Button")]
    public Button startSessionButton;
    [Header("Launch Game Button")]
    public Button lauchGameButton;
    [Header("Countdown")]
    public TextMeshProUGUI countdownText;
    public GameObject countdownObject;
    [Header("Player List")]
    public TextMeshProUGUI playerListText;
    public GameObject playerListObject;
    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    // Effects - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
    public List<ScreenEffects> ScreenEffectss = new List<ScreenEffects>();
    private Dictionary<string, ScreenEffects> IPs = new Dictionary<string, ScreenEffects>();
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    // Drum Beat Handling - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
    public List<float> startSequence = new List<float>();
    [SerializeField] private Dictionary<string, int> PlayerCalibration = new Dictionary<string, int>();
    //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    void Start()
    {   
        // Configure UI
        startSessionButton.gameObject.SetActive(true);
        lauchGameButton.gameObject.SetActive(false);
        playerListObject.gameObject.SetActive(false);
        countdownObject.gameObject.SetActive(false);
        
        // Set state
        currentState = GameState.SessionClosed;

        // Assign listeners
        startSessionButton.onClick.AddListener(StartSession);
        lauchGameButton.onClick.AddListener(LauchGame);
    }


    public void StartSession()
    {
        // Configure UI
        startSessionButton.gameObject.SetActive(false);
        lauchGameButton.gameObject.SetActive(true);
        playerListObject.gameObject.SetActive(true);
        countdownObject.gameObject.SetActive(false);
        
        // Set state
        currentState = GameState.SessionOpen;
        
        // Send OSC
        oscMessaging.Send_SessionOpen();
    }

    public void LauchGame()
    {
        // Configure UI
        startSessionButton.gameObject.SetActive(false);
        lauchGameButton.gameObject.SetActive(false);
        playerListObject.gameObject.SetActive(false);
        countdownObject.gameObject.SetActive(false);
        
        // Set state
        currentState = GameState.Calibrating;
        
        // Send OSC
        oscMessaging.Send_Calibrate();
    }

    public void StartGame()
    {
        // Configure UI
        startSessionButton.gameObject.SetActive(false);
        lauchGameButton.gameObject.SetActive(false);
        playerListObject.gameObject.SetActive(false);
        countdownObject.gameObject.SetActive(false);
        
        // Set state
        currentState = GameState.GameOn;
        
        oscMessaging.Send_StartGame(startSequence);

        // Play video
        string fileName = "Test2_360_200Mbps.mp4";
        string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
        GameObject.Find("VideoSkyboxPlayer").GetComponent<ConfigVideoSource>().PlayVideo(fullPath);

        Debug.Log("[GameManager] Started pridebeats video");
    }

    public void DrumHit(string IP, string content)
    {
        if (currentState == GameState.SessionOpen)
        {
            JoinSessionBeat(IP, content);
        }
        if (currentState == GameState.Calibrating)
        {
            CalibrationBeat(IP, content);
        }
        if (currentState == GameState.GameOn)
        {
            GameOnBeat(IP, content);
        }
    }

    private void JoinSessionBeat(string IP, string content)
    {
        // If this IP hasn't been seen before, add it with a starting value of 0
        if (!PlayerCalibration.ContainsKey(IP))
        {
            PlayerCalibration[IP] = 0;
            Debug.Log($"new player: {IP} Joined the session.");
        }

        // Build the player list string
        string playerListDisplay = "";
        foreach (var kvp in PlayerCalibration)
        {
            playerListDisplay += $"- Headset: {kvp.Key} : {kvp.Value} -\n";
        }

        // Update UI text
        playerListText.text = playerListDisplay;
    }


    private void GameOnBeat(string IP, string content)
    {
        // Assign ScreenEffects if not already mapped
        if (!IPs.TryGetValue(IP, out ScreenEffects ScreenEffects))
        {
            foreach (var effect in ScreenEffectss)
            {
                if (!IPs.ContainsValue(effect))
                {
                    ScreenEffects = effect;
                    IPs.Add(IP, ScreenEffects);
                    ScreenEffects.Init();
                    Debug.Log($"[GameManager] Assigned new IP {IP} to a ScreenEffects.");
                    break;
                }
            }

            if (ScreenEffects == null)
            {
                Debug.LogWarning($"[GameManager] No available ScreenEffects left to assign for IP: {IP}");
                return;
            }
        }

        // Handle sync messages
        if (content.Contains("in sync"))
        {
            Debug.Log(IP + " in sync");
            ScreenEffects.ActivateEffects(true);
        }
        else if (content.Contains("out of sync"))
        {
            Debug.Log(IP + " out of sync");
            ScreenEffects.ActivateEffects(false);
        }
        else
        {
            Debug.Log(IP + " invalid message content");
        }
    }

    private void CalibrationBeat(string IP, string content)
    {
        // If this IP hasn't been seen before, add it with a starting value of 0
        if (!PlayerCalibration.ContainsKey(IP))
        {
            PlayerCalibration[IP] = 0;
            Debug.Log($"[Calibration] Added new player {IP} to calibration list.");
        }

        // Increment calibration count for this IP
        if (PlayerCalibration[IP] < 3)
            PlayerCalibration[IP]++;
        
        // Check for all players calibrated
        bool allReady = true;
        foreach (var kvp in PlayerCalibration)
        {
            if (kvp.Value < 3)
            {
                allReady = false;
                break;
            }
        } // if all players calibrated --> start the game
        if (allReady) {
            Debug.Log("all players calibrated, GAME ON");
            StartGame();
        }

        Debug.Log($"[Calibration] {IP} beat count: {PlayerCalibration[IP]}");
    }
}
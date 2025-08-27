using UnityEngine;
using extOSC;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class GameManager : MonoBehaviour
{
    public int questPort;
    public int localPort;

    public ConfigVideoSource configVideoSource;
    private OSCTransmitter transmitter;

    public List<ScreenEffects> ScreenEffectss = new List<ScreenEffects>();
    private Dictionary<string, ScreenEffects> IPs = new Dictionary<string, ScreenEffects>();

    // Store local IP for reuse
    private string localIPAddress;

    void Awake()
    {
        // Get and store local IP once
        localIPAddress = GetLocalIPAddress();
        Debug.Log("[GameManager] Local IP: " + localIPAddress);

        // Setup transmitter to broadcast
        transmitter = gameObject.AddComponent<OSCTransmitter>();
        string broadcastIP = NetworkUtils.GetBroadcastAddress();
        Debug.Log("Broadcast IP: " + broadcastIP);
        transmitter.RemoteHost = broadcastIP;
        transmitter.RemotePort = 8100; // Port that Quests are listening on

        Debug.Log("[GameManager] Ready to broadcast to all Quests.");
    }

    private bool GameOn = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
            GameOn = true;
            Debug.Log("Starting game");
        }
    }

    public List<float> startSequence = new List<float> { 0.5f, 1.0f, 0.75f }; // Example pattern
    public void StartGame()
    {
        var startMsg = new OSCMessage("/StartGame");

        // Add the float sequence to the message
        foreach (float value in startSequence)
        {
            startMsg.AddValue(OSCValue.Float(value));
        }

        // ✅ Add the local IP as a string argument
        startMsg.AddValue(OSCValue.String(localIPAddress));

        transmitter.Send(startMsg);
        Debug.Log("[GameManager] Broadcasted /StartGame with " + startSequence.Count + " intervals + IP " + localIPAddress);

        // Play video
        string fileName = "Test2_360_200Mbps.mp4";
        string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
        GameObject.Find("VideoSkyboxPlayer").GetComponent<ConfigVideoSource>().PlayVideo(fullPath);

        Debug.Log("[GameManager] Started pridebeats video");
    }

    public void DrumHit(string IP, string content)
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

    // 🔹 Utility to get local IPv4 address
    private string GetLocalIPAddress()
    {
        string ipAddress = "0.0.0.0";
        try
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                ipAddress = endPoint.Address.ToString();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[GameManager] Could not determine local IP: {ex.Message}");
        }
        return ipAddress;
    }
}

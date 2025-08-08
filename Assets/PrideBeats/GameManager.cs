

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

    void Awake()
    {
        // Transmitter to broadcast address
        transmitter = gameObject.AddComponent<OSCTransmitter>();
        string broadcastIP = NetworkUtils.GetBroadcastAddress();
        Debug.Log("broadcastIP"  + broadcastIP);
        transmitter.RemoteHost = broadcastIP;
        transmitter.RemotePort = 8100; // Or whatever port your Quest devices are listening on


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

        transmitter.Send(startMsg);
        Debug.Log("[GameManager] Broadcasted /StartGame with " + startSequence.Count + " intervals");

        string fileName = "Test2_360_200Mbps.mp4";
        string fullPath = Path.Combine(Application.streamingAssetsPath, fileName);
        GameObject.Find("VideoSkyboxPlayer").GetComponent<ConfigVideoSource>().PlayVideo(fullPath);

        Debug.Log("[GameManager] Started pridebeats video");
    }


    public void DrumHit(string IP, string content)
    {
        // 1. Assign a ScreenEffects to the IP if not already assigned - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        if (!IPs.TryGetValue(IP, out ScreenEffects ScreenEffects))
        {
            // Assign the next unassigned ScreenEffects
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

        // Log message content
        if (content.Contains("in sync"))
        {
            Debug.Log(IP + " in sync");
            // Activate the effect
            ScreenEffects.ActivateEffects(true);
        }
        else if (content.Contains("out of sync"))
        {
            Debug.Log(IP + " out of sync");
            // Activate the effect
            ScreenEffects.ActivateEffects(false);
        }
        else
        {
            Debug.Log(IP + " invalid message content");
        }
    }

}

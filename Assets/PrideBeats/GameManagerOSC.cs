

using UnityEngine;
using extOSC;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class GameManagerOSC : MonoBehaviour
{
    public int questPort;
    public int localPort;

    private OSCTransmitter transmitter;

    public List<ScreenEffect> ScreenEffects = new List<ScreenEffect>();
    private Dictionary<string, ScreenEffect> IPs = new Dictionary<string, ScreenEffect>();

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

    public void StartGame()
    {
        var startMsg = new OSCMessage("/StartGame");
        transmitter.Send(startMsg);
        Debug.Log("[GameManager] Broadcasted /StartGame");
    }

    public void DrumHit(string IP, string content)
    {
        if (!IPs.TryGetValue(IP, out ScreenEffect screenEffect))
        {
            // Assign the next unassigned ScreenEffect
            foreach (var effect in ScreenEffects)
            {
                if (!IPs.ContainsValue(effect))
                {
                    screenEffect = effect;
                    IPs.Add(IP, screenEffect);
                    Debug.Log($"[GameManager] Assigned new IP {IP} to a ScreenEffect.");
                    break;
                }
            }

            if (screenEffect == null)
            {
                Debug.LogWarning($"[GameManager] No available ScreenEffect left to assign for IP: {IP}");
                return;
            }
        }

        // Log message content
        if (content.Contains("in sync"))
        {
            Debug.Log(IP + " in sync");
            // Activate the effect
            screenEffect.ActivateEffects(true);
        }
        else if (content.Contains("out of sync"))
        {
            Debug.Log(IP + " out of sync");
            // Activate the effect
            screenEffect.ActivateEffects(false);
        }
        else
        {
            Debug.Log(IP + " invalid message content");
        }
    }

}

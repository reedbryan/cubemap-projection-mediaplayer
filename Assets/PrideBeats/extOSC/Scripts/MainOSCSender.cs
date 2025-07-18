using UnityEngine;
using extOSC;

public class GameManagerOSC : MonoBehaviour
{
    [Header("Client IPs")]
    public string[] questIPs = { "192.168.1.101", "192.168.1.102", "192.168.1.103" }; // Example Quest IPs
    public int questPort = 8100;  // Assuming all listen on same port

    public int localPort = 8000;

    private OSCReceiver receiver;
    private OSCTransmitter[] transmitters;

    void Start()
    {
        // Setup receiver
        receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = localPort;
        receiver.Bind("/PlayerAction", OnReceivePlayerAction);

        // Setup transmitters for each Quest
        transmitters = new OSCTransmitter[questIPs.Length];
        for (int i = 0; i < questIPs.Length; i++)
        {
            var transmitter = gameObject.AddComponent<OSCTransmitter>();
            transmitter.RemoteHost = questIPs[i];
            transmitter.RemotePort = questPort;
            transmitters[i] = transmitter;
        }

        Debug.Log("[GameManager] Ready. Listening for player actions.");
    }

    public void StartGame()
    {
        var startMsg = new OSCMessage("/StartGame");

        foreach (var tx in transmitters)
        {
            tx.Send(startMsg);
        }

        Debug.Log("[GameManager] → Sent /StartGame to all Quests.");
    }

    void OnReceivePlayerAction(OSCMessage message)
    {
        Debug.Log($"[GameManager] ← Player Action: {message.Values[0].StringValue}");
        // Handle actions from players
    }
}

using UnityEngine;
using extOSC;
using System.Net;
using System.Net.Sockets;

public class OSCMessaging : MonoBehaviour
{
    private OSCTransmitter transmitter;
    private string localIPAddress;

    public int remotePort = 8100; // Port that Quests are listening on

    void Awake()
    {
        // Get and store local IP once
        localIPAddress = GetLocalIPAddress();
        Debug.Log("[OSCMessaging] Local IP: " + localIPAddress);

        // Setup transmitter to broadcast
        transmitter = gameObject.AddComponent<OSCTransmitter>();
        string broadcastIP = NetworkUtils.GetBroadcastAddress();
        Debug.Log("[OSCMessaging] Broadcast IP: " + broadcastIP);
        transmitter.RemoteHost = broadcastIP;
        transmitter.RemotePort = remotePort;

        Debug.Log("[OSCMessaging] Ready to broadcast.");
    }

    public void Send_SessionOpen()
    {
        var msg = new OSCMessage("/SessionOpen");
        msg.AddValue(OSCValue.String(localIPAddress));
        transmitter.Send(msg);
    }

    public void Send_Calibrate()
    {
        var msg = new OSCMessage("/Calibrate");
        transmitter.Send(msg);
    }

    public void Send_StartGame(System.Collections.Generic.List<float> sequence)
    {
        var msg = new OSCMessage("/StartGame");

        foreach (float value in sequence)
        {
            msg.AddValue(OSCValue.Float(value));
        }

        msg.AddValue(OSCValue.String(localIPAddress));
        transmitter.Send(msg);

        Debug.Log("[OSCMessaging] Broadcasted /StartGame with " + sequence.Count + " intervals + IP " + localIPAddress);
    }

    // Utility to get local IPv4 address
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
            Debug.LogWarning($"[OSCMessaging] Could not determine local IP: {ex.Message}");
        }
        return ipAddress;
    }
}

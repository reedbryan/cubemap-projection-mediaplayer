using UnityEngine;
using extOSC;
using System.Collections.Generic;

public class OSCReceiverAll : MonoBehaviour
{
    [Header("OSC Settings")]
    public int port = 8000;

    private OSCReceiver receiver;
    public GameManager GM;

    void Start()
    {
        receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = port;

        // This binds to all addresses starting with "/"
        receiver.Bind("*", OnAnyMessage);  // Use "*" instead of "BindAll"
        receiver.Bind("/DrumHit", OnDrumHit); 
        receiver.Bind("/Ready", NewPlayerJoined); 

        Debug.Log($"OSC Receiver started on port {port}, listening to all addresses");
    }

    private void OnAnyMessage(OSCMessage message)
    {
        Debug.Log($"Received message: {message.Address}");

        if (message.Values.Count >= 2)
        {
            string content = message.Values[0].StringValue;
            string ip = message.Values[1].StringValue;

            Debug.Log($"  Content: {content}");            
        }
        else
        {
            //Debug.LogWarning("Received message does not contain expected number of values.");
        }
    }

    private void NewPlayerJoined(OSCMessage message)
    {
        Debug.Log($"PLAYER JOINED");
    }

    private void OnDrumHit(OSCMessage message)
    {
        string content = message.Values[0].StringValue;
        string ip = message.Values[1].StringValue;

        GM.DrumHit(ip, content);

        Debug.Log("HUUUUUUUUHHHHHHHHHHHH");
    }
}

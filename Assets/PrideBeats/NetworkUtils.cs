using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

public class NetworkUtils : MonoBehaviour
{
    public static string GetBroadcastAddress()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != OperationalStatus.Up || ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                continue;

            foreach (UnicastIPAddressInformation ua in ni.GetIPProperties().UnicastAddresses)
            {
                if (ua.Address.AddressFamily == AddressFamily.InterNetwork) // IPv4 only
                {
                    IPAddress ip = ua.Address;
                    IPAddress mask = ua.IPv4Mask;
                    if (ip == null || mask == null) continue;

                    byte[] ipBytes = ip.GetAddressBytes();
                    byte[] maskBytes = mask.GetAddressBytes();
                    byte[] broadcastBytes = new byte[4];

                    for (int i = 0; i < 4; i++)
                        broadcastBytes[i] = (byte)(ipBytes[i] | (maskBytes[i] ^ 255));

                    IPAddress broadcast = new IPAddress(broadcastBytes);
                    Debug.Log($"[Broadcast] Local IP: {ip} | Mask: {mask} | Broadcast: {broadcast}");
                    return broadcast.ToString();
                }
            }
        }

        Debug.LogWarning("No suitable network interface found.");
        return "255.255.255.255"; // Fallback
    }
}

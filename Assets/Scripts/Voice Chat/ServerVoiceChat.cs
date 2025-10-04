using System;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;

/// <summary>
/// Should be applied to the single [Don'tDestroyOnLoad] Server Object.
/// </summary>
public class ServerVoiceChat : NetworkBehaviour
{

    void Start()
    {
        NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("VoiceData", OnReceiveFromClient);
    }

    void OnReceiveFromClient(ulong senderClientId, FastBufferReader reader)
    {
        print("(I am the server) -> Received VoiceData from client!!!");

        // retrieving the voice data. I'm hoping Unity handles race conditions.
        reader.ReadValueSafe(out int byteCount);
        byte[] pcmBytes = new byte[byteCount];
        reader.ReadBytesSafe(ref pcmBytes, byteCount, 0);

        // create the new message
        int headerSize = sizeof(int);
        FastBufferWriter writer = new FastBufferWriter(headerSize + byteCount, Allocator.Temp);
        writer.TryBeginWrite(sizeof(int) + byteCount);
        writer.WriteValueSafe<int>(byteCount);
        writer.WriteBytes(pcmBytes, byteCount, 0);

        // send the message to all players, except for 0 and the sender.
        var targets = NetworkManager.Singleton.ConnectedClientsIds.Where(id => id != senderClientId && id != NetworkManager.ServerClientId).ToList();
        //var targets = NetworkManager.Singleton.ConnectedClientsIds.ToList();
        if (targets.Count > 0)
        {
            print("(I am the server) -> Sending voice data to clients!");
            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("VoiceData", targets, writer, NetworkDelivery.UnreliableSequenced);
        }
    }

    private void OnDestroy()
    {
        NetworkManager.CustomMessagingManager.UnregisterNamedMessageHandler("VoiceData");
    }
}

using UnityEngine;
using Unity.Netcode;
using System.Runtime.CompilerServices;
using System.Collections;
using UnityEditor;
using Unity.Collections;
using System.Collections.Generic;
public class ServerPlayerManagement : MonoBehaviour
{

    private Dictionary<ulong, FixedString64Bytes> playerNames = new Dictionary<ulong, FixedString64Bytes>();

    public void ClientConnectedApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // we should probably have some sort of validation here...
        byte[] connectionDataBytes = request.Payload;
        FixedString64Bytes playerName = System.Text.UTF8Encoding.UTF8.GetString(connectionDataBytes);

        playerNames.Add(request.ClientNetworkId, playerName);

        response.Approved = true;
        response.CreatePlayerObject = true;
        Debug.Log("ClientConnection Approved - > " + playerName + " " + request.ClientNetworkId);

        
    }

    public void OnClientConnectedCallback(ulong clientID)
    {
        Debug.Log("client connected -> " + clientID);
        //NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject;
        //playerObject.GetComponent<BotcPlayer>().SetName(currentPlayerName);
        //print("Player connected - > " + currentPlayerName);
        //currentPlayerName = "";
    }

    public void OnEnable()
    {
        if (NetworkManager.Singleton == null)
        {
            print("Waiting for the NetworkManager Singleton to become available!");
            StartCoroutine(addCallbacks());
        }
        else
        {
            print("NetworkManager Singleton is available right away!");
            NetworkManager.Singleton.ConnectionApprovalCallback += ClientConnectedApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        }
    }

    public void OnDisable()
    {
        //NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.ConnectionApprovalCallback -= ClientConnectedApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
    }

    IEnumerator addCallbacks()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null);
        NetworkManager.Singleton.ConnectionApprovalCallback += ClientConnectedApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
    }

    public FixedString64Bytes getPlayerName(ulong clientId)
    {
        Debug.Log("Player Name requested");

        if (playerNames.TryGetValue(clientId, out FixedString64Bytes name))
        {
            Debug.Log("getPlayerName: returning " + name);
            return name;
        }
        else
        {
            return "error getting name";
        }
    }
}

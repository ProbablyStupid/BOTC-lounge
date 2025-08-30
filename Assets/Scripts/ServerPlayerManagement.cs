using UnityEngine;
using Unity.Netcode;
using System.Runtime.CompilerServices;
using System.Collections;
using UnityEditor;
public class ServerPlayerManagement : MonoBehaviour
{
    private string currentPlayerName;

    public void ClientConnectedApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        byte[] connectionDataBytes = request.Payload;
        string playerName = System.Text.UTF8Encoding.UTF8.GetString(connectionDataBytes);

        currentPlayerName = playerName;

        response.Approved = true;
        print("ClientConnection Approved - > " + playerName);
    }

    public void OnClientConnectedCallback(ulong clientID)
    {
        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject;
        playerObject.GetComponent<BotcPlayer>().SetName(currentPlayerName);
        print("Player connected - > " + currentPlayerName);
        currentPlayerName = "";
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
}

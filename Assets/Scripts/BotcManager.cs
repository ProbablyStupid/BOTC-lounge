using Unity.Netcode;
using UnityEngine;

/// <summary>
/// The BotcManager is NOT a network object. Every instance of the game has one.
/// 
/// If the instance is the server (logic in the Start method), it will spawn a BotcMaster, which is a network object.
/// </summary>
public class BotcManager:NetworkBehaviour
{
    public void Awake()
    {
        // TODO: implement condition for detecting server here!
        bool isServer = false;
        if (isServer)
        {
            // TODO: spawn the BotcMaster here!
        }
    }
}

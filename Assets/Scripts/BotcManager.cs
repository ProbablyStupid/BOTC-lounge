using Unity.Netcode;
using UnityEngine;

/// <summary>
/// The BotcManager is NOT a network object. Every instance of the game has one.
/// 
/// If the instance is the server (logic in the Start method), it will spawn a BotcMaster, which is a network object.
/// </summary>
public class BotcManager:NetworkBehaviour
{
    [SerializeField] private bool created = false;

    [SerializeField] GameObject masterPrefab;

    public void Start()
    {
        if (IsServer)
        {
            Debug.Log("Running on Server! Instantiating master!");
            Instantiate(masterPrefab);
            created = true;
        } else
        {
            Debug.Log("Running on not server! Not creating BotcMaster!");
        }
    }

    public void Update()
    {
        if (IsServer && !created)
        {
            Debug.Log("Running on Server! Instantiating master!");
            Instantiate(masterPrefab);
            created = true;
        }
    }
}

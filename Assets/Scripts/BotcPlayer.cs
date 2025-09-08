using UnityEngine;
using Unity.Netcode;
using System;
using TMPro;
using Unity.Collections;
using System.Security.Cryptography;

public class BotcPlayer : NetworkBehaviour
{

    [SerializeField] TextMeshPro playerNameTag;

    /// <summary>
    /// This is here and not in a seperate file because uhhh, reducing complexity?
    /// 
    /// Note:
    /// NetworkVariables in Unity Netcode need a string of constant size. Since the
    /// default 'string' is changeable, it cannot be used inside a network variable.
    /// 
    /// </summary>
    [SerializeField] NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>("goofy");
    string localName = "goofy";


    /// <summary>
    /// Specifies which type of Player the Player is:
    /// 
    /// 0 -> null; not part of the game
    /// 1 -> Player
    /// 2 -> Storyteller
    /// 3 -> Observer [to come]
    /// 
    /// </summary>
    [SerializeField] public NetworkVariable<int> BOTC_PlayerType = new NetworkVariable<int>(1);

    /// <summary>
    /// In case the player is a Storyteller, the RoleName is irrelevant, but is ideally set to "na".
    /// </summary>
    [SerializeField] public NetworkVariable<FixedString64Bytes> BOTC_RoleName = new NetworkVariable<FixedString64Bytes>("unassigned");

    // The server does not do logic for this! It is the job of the storyteller to manage this variable.
    [SerializeField] public NetworkVariable<bool> BOTC_PlayerAlive = new NetworkVariable<bool>(true);

    // this is relevant so the player knows which round to report to
    // this is not used anywhere :( ... yet!
    public BOTCRound ParentRound = null;

    public BOTCSeat mySeat = null;

    public void AssignSeat(BOTCSeat seat)
    {
        if (mySeat != null) 
            return;
        
        mySeat = seat;
    }

    public void ClearSeat()
    {
        mySeat = null;
    }

    public void Revive()
    {
        BOTC_PlayerAlive.Value = true;
    }

    public void Kill()
    {
        BOTC_PlayerAlive.Value = false;
    }

    public bool GetAlive() { return BOTC_PlayerAlive.Value; }

    public void SetName(FixedString64Bytes name)
    {
        Debug.Log("Set name of player to " + name + " at " + playerNameTag);

        // this is stupid, but Unity is forcing us to do this ToString() nonsense...
        //playerNameTag.text = name.ToString();
        playerNameTag.SetText(name.ToString());
        
        // this step is not necessary because the change is triggered through the network variable.
        //  playerName.Value = name;
        
        
        localName = name.ToString();
    }

    public bool CompareName(FixedString64Bytes name)
    {
        return name.ToString().Equals(playerName.Value.ToString());
    }

    void OnPlayerNameValueChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        Debug.Log("OnPlayerNameValueChanged -> called! newValue " + newValue);
        SetName(newValue);
    }

    void OnRoleValueChanged(int previousValue, int newValue)
    {
        // something?
        if (newValue == 2)
        {
            Debug.Log("You are the storyteller!");
        } else if (newValue == 1)
        {
            Debug.Log("You are a player");
        }
    }

    public void Awake()
    {
        Debug.Log("BOTC player is awake " + OwnerClientId);
        playerName.OnValueChanged += OnPlayerNameValueChanged;
        BOTC_PlayerType.OnValueChanged += OnRoleValueChanged;
    }

    public void Start()
    {
        Debug.Log("Starting BotcPlayer");

        // hideously inefficient, but it'll work for now
        StorytellerUI storytellerUI = FindFirstObjectByType<StorytellerUI>();
        Debug.Log("StorytellerUI Found -> " + storytellerUI.name);
        storytellerUI.AssignPlayer(this);
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("On Spawn triggered!");
        if (IsOwner)
        {
            Debug.Log("Setting name of player.");
            requestMyNameServerRpc(OwnerClientId);

            // set us as the clocktower's desired transform
            // this may be a stupid way to find the clocktower, but it works for now
            GameObject clocktower = GameObject.FindGameObjectWithTag("Clocktower");
            Timer timer = clocktower.GetComponent<Timer>();
            timer.RegisterPlayer(transform);
        }
    }

    public void SetPlayerType(int type)
    {
        BOTC_PlayerType.Value = type;
    }

    [ServerRpc]
    void requestMyNameServerRpc(ulong clientId)
    {
        Debug.Log("Running requestMyNameServerRpc");
        var playerManager = FindFirstObjectByType<ServerPlayerManagement>();
        FixedString64Bytes name = playerManager.getPlayerName(clientId);
        playerName.Value = name;
        Debug.Log("Updated playerName " + name);
    }
}

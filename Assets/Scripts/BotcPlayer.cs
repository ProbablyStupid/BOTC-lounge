using UnityEngine;
using Unity.Netcode;
using System;
public class BotcPlayer : NetworkBehaviour
{
    /// <summary>
    /// This is here and not in a seperate file because uhhh, reducing complexity?
    /// </summary>
    [SerializeField] NetworkVariable<string> playerName = new NetworkVariable<string>("goofy");


    /// <summary>
    /// Specifies which type of Player the Player is:
    /// 
    /// 0 -> null; not part of the game
    /// 1 -> Player
    /// 2 -> Storyteller
    /// 3 -> Observer [to come]
    /// 
    /// </summary>

    [SerializeField] NetworkVariable<int> BOTC_PlayerType = new NetworkVariable<int>(1);

    /// <summary>
    /// In case the player is a Storyteller, the RoleName is irrelevant, but is ideally set to "na".
    /// </summary>
    [SerializeField] NetworkVariable<string> BOTC_RoleName = new NetworkVariable<string>("unassigned");

    // The server does not do logic for this! It is the job of the storyteller to manage this variable.
    [SerializeField] NetworkVariable<bool> BOTC_PlayerAlive = new NetworkVariable<bool>(true);


    public void Revive()
    {
        BOTC_PlayerAlive.Value = true;
    }

    public void Kill()
    {
        BOTC_PlayerAlive.Value = false;
    }

    public bool GetAlive() { return BOTC_PlayerAlive.Value; }

}

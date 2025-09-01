using UnityEngine;
using Unity.Netcode;


/// <summary>
/// 
///     The BotcMaster is responsible for storing and managing attributes that
///     globally affect players.
///     This includes the time / stage of the day.
///     There should only EVER be one BotcMaster in any game. Otherwise we'll have a big problem!
///     
///     Important Note: This architecture does not allow for a new game to start in the same world.
///                     New players and a new BotcMaster would need to be generated.
/// 
/// </summary>
public class BotcMaster : NetworkBehaviour
{
    /// <summary>
    /// The stage of the day in the BOTC game.
    /// 
    /// 1 - Generic day
    /// 2 - Collaborative
    /// 3 - Voting
    /// 4 - Night
    /// </summary>
    NetworkVariable<int> BOTC_time = new NetworkVariable<int>(1);
}

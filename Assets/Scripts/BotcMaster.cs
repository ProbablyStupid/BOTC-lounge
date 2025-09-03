using UnityEngine;
using Unity.Netcode;


// this class should only exist on the server
public class BotcMaster : NetworkBehaviour
{
    [SerializeField] NetworkVariable<bool> inRound = new NetworkVariable<bool>(false);

    // BOTCRound is not yet serializable...
    NetworkVariable<BOTCRound> currentRound = new NetworkVariable<BOTCRound>(null);

    // control elements -----------------------------------------------------------------------------
    [SerializeField] Controlable start_round;

    /// <summary>
    /// Call before RegisterPlayers!
    /// </summary>
    public void NewRound()
    {
        currentRound.Value = new BOTCRound();
    }

    // TODO: implement this now!
    public void RegisterPlayers()
    {
        // do we simply take all players and force them into the next round?

        BotcPlayer[] players = GetComponents<BotcPlayer>();
        currentRound.Value.SetPlayers(players);
    }

    public void StartRound()
    {
        currentRound.Value.Initialize();
    }

    public BOTCRound CurrentRound()
    {
        return currentRound.Value;
    }
}

using UnityEngine;
using Unity.Netcode;


// this class should only exist on the server
public class BotcMaster : NetworkBehaviour
{
    [SerializeField] NetworkVariable<bool> inRound = new NetworkVariable<bool>(false);

    // BOTCRound is not yet serializable...
    [SerializeField] BOTCRound currentRound = null;

    // control elements -----------------------------------------------------------------------------
    [SerializeField] Controlable start_round;

    [SerializeField] BOTCCommonArea commonArea;

    public void Start()
    {
        commonArea = FindFirstObjectByType<BOTCCommonArea>();
        Debug.Log("Detected BOTC Common area " + commonArea.name);


    }

    /// <summary>
    /// Call before RegisterPlayers!
    /// </summary>
    public void NewRound()
    {
        // it is important that we do not use the new keyword since that is not allowed by Unity.
        currentRound = gameObject.AddComponent<BOTCRound>();
    }

    // TODO: implement this now!
    public void RegisterPlayers()
    {
        // do we simply take all players and force them into the next round?

        BotcPlayer[] players = GetComponents<BotcPlayer>();
        Debug.Log("found players");
        foreach (var player in players)
        {
            Debug.Log(player.name);
        }
        currentRound.SetPlayers(players);
    }

    public void StartRound()
    {
        currentRound.Initialize();
    }

    public BOTCRound CurrentRound()
    {
        return currentRound;
    }

    public BOTCCommonArea GetCommonArea()
    {
        return commonArea;
    }
}

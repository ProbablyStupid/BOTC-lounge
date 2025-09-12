using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class StorytellerUI : NetworkBehaviour
{
    [SerializeField] BOTCCommonArea commonArea;

    [SerializeField] GameObject storyTellerUI;

    [SerializeField] private bool enabled = false;

    [SerializeField] BotcPlayer ownedPlayer = null;

    [SerializeField] PlayerMovement playerMovement = null;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnStorytellerUIButtonPressed();
        }
    }

    public void OnStorytellerUIButtonPressed()
    {
        if (ownedPlayer == null)
        {
            Debug.Log("No BOTCPlayer present!");
            return;
        }

        if (ownedPlayer.BOTC_PlayerType.Value != 2)
        {
            Debug.Log("BOTCPlayer is not storyteller!");
            return;
        }

        enabled = !enabled;
        storyTellerUI.SetActive(enabled);

        if (enabled)
        {
            playerMovement.Unlock();
        } else 
        {
            playerMovement.Lock();
        }
    }

    public void AssignPlayer(BotcPlayer player)
    {
        ownedPlayer = player;
        playerMovement = player.gameObject.GetComponent<PlayerMovement>();
    }

    public void OnGameStart()
    {
        Debug.Log("Game Start!");
        StartGameServerRpc();
    }

    public void OnGameEnd()
    {
        Debug.Log("Game End!");
    }

    public void OnTimerStart()
    {
        Debug.Log("Timer Start!");
    }

    public void OnTimerStop()
    {
        Debug.Log("Timer End!");
    }

    public void OnTeleportAllBack()
    {
        Debug.Log("Teleport all back!");
    }

    [ServerRpc]
    public void StartGameServerRpc()
    {
        // the server will look through all currently active players
        // generate the seats in the common area

        Debug.Log("Starting game " + IsServer);

        // but really, this is the job of the BOTCMaster
        BotcMaster master = FindFirstObjectByType<BotcMaster>();

        Debug.Log(master);

        if (master.CurrentRound() == null)
        {
            master.NewRound();
        }

        master.RegisterPlayers();
        master.StartRound();
        master.GetCommonArea().Regenerate(master.CurrentRound().GetPlayers().Length);
    }
}
